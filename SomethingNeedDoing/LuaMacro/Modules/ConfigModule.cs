using NLua;
using SomethingNeedDoing.Core.Interfaces;

namespace SomethingNeedDoing.LuaMacro.Modules;

/// <summary>
/// Lua module for macro configuration management.
/// </summary>
public class ConfigModule(IMacro macro) : LuaModuleBase
{
    public override string ModuleName => "Config";

    [LuaFunction]
    public object Get(string name, object? defaultValue = null)
    {
        if (string.IsNullOrEmpty(name))
            return defaultValue ?? string.Empty;

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
            return ValidateValue(configItem, configItem.Value);

        return defaultValue ?? string.Empty;
    }

    [LuaFunction]
    public int GetInt(string name, object? defaultValue = null) => Get(name, defaultValue) switch
    {
        int i => i,
        string s when int.TryParse(s, out var parsed) => parsed,
        double d => (int)d,
        float f => (int)f,
        _ => throw new InvalidCastException($"Config value {name} cannot be converted to int. Current value: {Get(name, defaultValue)} (type: {Get(name, defaultValue)?.GetType().Name})")
    };

    [LuaFunction]
    public float GetFloat(string name, object? defaultValue = null) => Get(name, defaultValue) switch
    {
        float f => f,
        double d => (float)d,
        int i => i,
        string s when float.TryParse(s, out var parsed) => parsed,
        _ => throw new InvalidCastException($"Config value {name} cannot be converted to float. Current value: {Get(name, defaultValue)} (type: {Get(name, defaultValue)?.GetType().Name})")
    };

    [LuaFunction]
    public bool GetBool(string name, object? defaultValue = null) => Get(name, defaultValue) switch
    {
        bool b => b,
        string s when bool.TryParse(s, out var parsed) => parsed,
        int i => i != 0,
        _ => throw new InvalidCastException($"Config value {name} cannot be converted to bool. Current value: {Get(name, defaultValue)} (type: {Get(name, defaultValue)?.GetType().Name})")
    };

    [LuaFunction]
    public string GetString(string name, object? defaultValue = null) => Get(name, defaultValue)?.ToString() ?? string.Empty;

    [LuaFunction]
    public void Set(string name, object value)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
        {
            var validatedValue = ValidateValue(configItem, value);
            configItem.Value = validatedValue;

            if (macro is ConfigMacro)
                C.Save();
        }
    }

    [LuaFunction]
    public object GetDefault(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
            return configItem.DefaultValue;

        return string.Empty;
    }

    [LuaFunction]
    public void Reset(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
        {
            configItem.Value = configItem.DefaultValue;
            if (macro is ConfigMacro)
                C.Save();
        }
    }

    [LuaFunction]
    public string GetDescription(string name)
    {
        if (string.IsNullOrEmpty(name))
            return string.Empty;

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
            return configItem.Description;

        return string.Empty;
    }

    [LuaFunction]
    public string GetType(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "string";

        if (macro.Metadata.Configs.TryGetValue(name, out var configItem))
            return configItem.TypeName;

        return "string";
    }

    [LuaFunction]
    public bool Exists(string name) => !string.IsNullOrEmpty(name) && macro.Metadata.Configs.ContainsKey(name);

    [LuaFunction]
    public string[] GetAllNames() => [.. macro.Metadata.Configs.Keys];

    private object ValidateValue(MacroConfigItem configItem, object value)
    {
        try
        {
            switch (configItem.Type)
            {
                case var t when t == typeof(int):
                    if (int.TryParse(value.ToString(), out var intValue))
                    {
                        if (configItem.MinValue != null && int.TryParse(configItem.MinValue.ToString(), out var min) && intValue < min)
                            return min;
                        if (configItem.MaxValue != null && int.TryParse(configItem.MaxValue.ToString(), out var max) && intValue > max)
                            return max;
                        return intValue;
                    }
                    return configItem.DefaultValue;

                case var t when t == typeof(float) || t == typeof(double):
                    if (double.TryParse(value.ToString(), out var doubleValue))
                    {
                        if (configItem.MinValue != null && double.TryParse(configItem.MinValue.ToString(), out var min) && doubleValue < min)
                            return min;
                        if (configItem.MaxValue != null && double.TryParse(configItem.MaxValue.ToString(), out var max) && doubleValue > max)
                            return max;
                        return doubleValue;
                    }
                    return configItem.DefaultValue;

                case var t when t == typeof(bool):
                    if (bool.TryParse(value.ToString(), out var boolValue))
                        return boolValue;
                    return configItem.DefaultValue;

                case var t when t == typeof(List<string>):
                    if (configItem.IsChoice)
                    {
                        var choice = value?.ToString() ?? string.Empty;
                        return configItem.Choices.Contains(choice) ? choice : configItem.Choices.FirstOrDefault() ?? string.Empty;
                    }
                    else
                        return value is List<string> list ? list : [];

                case var t when t == typeof(string):
                    if (configItem.IsChoice)
                    {
                        var choice = value?.ToString() ?? string.Empty;

                        // 动态选项由 IPC 提供，本地没有候选表时不做限定
                        if (configItem.HasDynamicChoices || configItem.Choices.Count == 0)
                            return choice;

                        return configItem.Choices.Contains(choice) ? choice : configItem.Choices.FirstOrDefault() ?? string.Empty;
                    }
                    else
                    {
                        if (value.ToString() is { } str && !string.IsNullOrEmpty(configItem.ValidationPattern))
                        {
                            try
                            {
                                var regex = new System.Text.RegularExpressions.Regex(configItem.ValidationPattern);
                                if (!regex.IsMatch(str))
                                {
                                    FrameworkLogger.Warning($"Config validation failed for '{configItem.Description}': {configItem.ValidationMessage ?? "Value does not match pattern"}");
                                    return configItem.DefaultValue;
                                }
                            }
                            catch (Exception ex)
                            {
                                FrameworkLogger.Error(ex, $"Invalid validation pattern for config: {configItem.ValidationPattern}");
                            }
                        }

                        return value.ToString() ?? string.Empty;
                    }

                default:
                    return value.ToString() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            FrameworkLogger.Error(ex, $"Failed to validate config value for {configItem.Description}");
            return configItem.DefaultValue;
        }
    }
}
