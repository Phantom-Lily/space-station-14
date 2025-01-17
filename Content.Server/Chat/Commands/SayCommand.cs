using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Server.Ghost.Components;
using Content.Server.Players;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.IoC;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal class SayCommand : IConsoleCommand
    {
        public string Command => "say";
        public string Description => "Send chat messages to the local channel or a specified radio channel.";
        public string Help => "say <text>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player as IPlayerSession;
            if (player == null)
            {
                shell.WriteLine("This command cannot be run from the server.");
                return;
            }

            if (player.Status != SessionStatus.InGame || !player.AttachedEntityUid.HasValue)
                return;

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            var chat = IoCManager.Resolve<IChatManager>();
            var chatSanitizer = IoCManager.Resolve<IChatSanitizationManager>();
            var playerEntity = player.AttachedEntity;

            if (playerEntity == null)
            {
                shell.WriteLine("You don't have an entity!");
                return;
            }

            if (playerEntity.HasComponent<GhostComponent>())
                chat.SendDeadChat(player, message);
            else
            {
                var mindComponent = player.ContentData()?.Mind;

                if (mindComponent == null)
                {
                    shell.WriteError("You don't have a mind!");
                    return;
                }

                if (mindComponent.OwnedEntity == null)
                {
                    shell.WriteError("You don't have an entity!");
                    return;
                }

                var emote = chatSanitizer.TrySanitizeOutSmilies(message, mindComponent.OwnedEntity, out var sanitized, out var emoteStr);
                if (sanitized.Length != 0)
                    chat.EntitySay(mindComponent.OwnedEntity, sanitized);
                if (emote)
                    chat.EntityMe(mindComponent.OwnedEntity, emoteStr!);
            }

        }
    }
}
