using Dalamud.Plugin.Ipc;

namespace SomethingNeedDoing.Core;

/// <summary>
/// Resolves config choices at runtime from another plugin's IPC.
/// </summary>
public static class DynamicChoices
{
    private static readonly Dictionary<string, (DateTime FetchedAt, string[] Choices)> Cache = [];
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets the choices for the given config item, falling back to its static list.
    /// </summary>
    public static string[] Resolve(MacroConfigItem item)
    {
        if (!item.HasDynamicChoices)
            return [.. item.Choices];

        var key = $"{item.ChoicesCountIpc}|{item.ChoicesItemIpc}";

        if (Cache.TryGetValue(key, out var cached) && DateTime.UtcNow - cached.FetchedAt < CacheDuration)
            return cached.Choices;

        var choices = Fetch(item);

        if (choices.Length > 0)
            Cache[key] = (DateTime.UtcNow, choices);

        return choices.Length > 0 ? choices : [.. item.Choices];
    }

    private static string[] Fetch(MacroConfigItem item)
    {
        try
        {
            var countSubscriber = Svc.PluginInterface.GetIpcSubscriber<int>(item.ChoicesCountIpc!);
            if (!countSubscriber.HasFunction)
                return [];

            var count = countSubscriber.InvokeFunc();
            if (count <= 0)
                return [];

            var itemSubscriber = Svc.PluginInterface.GetIpcSubscriber<int, string>(item.ChoicesItemIpc!);
            if (!itemSubscriber.HasFunction)
                return [];

            var choices = new string[count];

            for (var index = 0; index < count; index++)
                choices[index] = itemSubscriber.InvokeFunc(index) ?? string.Empty;

            return [.. choices.Where(choice => !string.IsNullOrWhiteSpace(choice))];
        }
        catch (Exception ex)
        {
            FrameworkLogger.Debug($"Failed to resolve dynamic choices from IPC: {ex.Message}");
            return [];
        }
    }
}
