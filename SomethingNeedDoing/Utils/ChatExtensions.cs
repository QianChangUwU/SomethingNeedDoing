using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using ECommons.ChatMethods;

namespace SomethingNeedDoing.Utils;

public static class ChatExtensions
{
    private const ushort PREFIX_COLOR = (ushort)UIColor.Red;

    public static void PrintMessage(this IChatGui chat, string message)
        => chat.Print(new XivChatEntry
        {
            Type    = C.ChatType,
            Message = BuildMessage(message)
        });

    public static void PrintErrorMsg(this IChatGui chat, string message)
        => chat.Print(new XivChatEntry
        {
            Type    = C.ErrorChatType,
            Message = BuildMessage(message)
        });

    public static void PrintColor(this IChatGui chat, string message, UIColor color)
        => chat.Print(new XivChatEntry
        {
            Type = C.ChatType,
            Message = new SeStringBuilder()
                      .Add(BuildMessage(string.Empty).Payloads)
                      .AddUiForeground((ushort)color)
                      .AddText(message)
                      .AddUiForegroundOff()
                      .Build()
        });

    public static SeString BuildMessage(string message) =>
        new SeStringBuilder()
            .AddUiForeground(PREFIX_COLOR)
            .AddText(SeIconChar.BoxedLetterS.ToIconString())
            .AddText(SeIconChar.BoxedLetterN.ToIconString())
            .AddText(SeIconChar.BoxedLetterD.ToIconString())
            .AddUiForegroundOff()
            .AddText(message.Length == 0 ? string.Empty : $" {message}")
            .Build();
}
