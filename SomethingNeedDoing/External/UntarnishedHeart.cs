using ECommons.EzIpcManager;
using SomethingNeedDoing.Core.Interfaces;

namespace SomethingNeedDoing.External;

public class UntarnishedHeart : IPC
{
    public override string Name => "UntarnishedHeart";
    public override string Repo => Repos.FirstParty;

    [EzIPC]
    [LuaFunction(description: "Checks if a preset or route is currently running")]
    public readonly Func<bool> IsRunning = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the current execution mode", examples: ["Preset", "Route"])]
    public readonly Func<string> GetMode = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the name of the running preset or route")]
    public readonly Func<string> GetName = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the current round of the running preset")]
    public readonly Func<int> GetRound = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the max round of the running preset, -1 means infinite")]
    public readonly Func<int> GetMaxRound = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the current step of the running route")]
    public readonly Func<int> GetStep = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the total step count of the running route")]
    public readonly Func<int> GetStepCount = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the running message of the current executor")]
    public readonly Func<string> GetMessage = null!;

    [EzIPC]
    [LuaFunction(description: "Checks if stop-after-duty-completion has been requested")]
    public readonly Func<bool> IsStopAfterDutyCompletionRequested = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the preset count")]
    public readonly Func<int> GetPresetCount = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the preset name at the given index", parameterDescriptions: ["index"])]
    public readonly Func<int, string> GetPresetNameAt = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the route count")]
    public readonly Func<int> GetRouteCount = null!;

    [EzIPC]
    [LuaFunction(description: "Gets the route name at the given index", parameterDescriptions: ["index"])]
    public readonly Func<int, string> GetRouteNameAt = null!;

    [EzIPC]
    [LuaFunction(
        description: "Starts the preset with the given name, returns an empty string on success or the failure reason",
        parameterDescriptions: ["name", "rounds"])]
    public readonly Func<string, int, string> StartPreset = null!;

    [EzIPC]
    [LuaFunction(
        description: "Starts the route with the given name, returns an empty string on success or the failure reason",
        parameterDescriptions: ["name"])]
    public readonly Func<string, string> StartRoute = null!;

    [EzIPC]
    [LuaFunction(description: "Stops the current execution")]
    public readonly Func<bool> Stop = null!;

    [EzIPC]
    [LuaFunction(description: "Requests stopping after the current or next duty completes")]
    public readonly Func<bool> RequestStopAfterDutyCompletion = null!;

    [EzIPC]
    [LuaFunction(description: "Cancels a pending stop-after-duty-completion request")]
    public readonly Func<bool> CancelStopAfterDutyCompletion = null!;

    [EzIPC]
    [LuaFunction(description: "Gets a human readable status line of the current execution")]
    public readonly Func<string> GetStatus = null!;
}
