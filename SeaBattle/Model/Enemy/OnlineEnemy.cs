using System.Threading.Tasks;
using System.Windows;

namespace Battleships.Model
{
    public class OnlineEnemy : Enemy
    {
        public override async Task SendAttackRequest(int X, int Y)
        {
            await Battle.ServerManager.SendMessage(MessageType.AttackRequest, new Point(X, Y));
        }

        public override async void SendAttackResult(AttackMessage message)
        {
            await Battle.ServerManager.SendMessage(MessageType.AttackResult, message);
        }

        public override async void SendChatMessage(string message)
        {
            await Battle.ServerManager.SendMessage(MessageType.Chat, message);
        }

        public async override Task SendGameMessage(GameMessageType message)
        {
            await Battle.ServerManager.SendMessage(MessageType.Game, message);
        }

        public async override Task GiveTurn()
        {
            await SendGameMessage(GameMessageType.YourTurn);
            await base.GiveTurn();
        }
    }
}