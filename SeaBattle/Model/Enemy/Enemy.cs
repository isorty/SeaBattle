using System.Threading.Tasks;

namespace Battleships.Model
{
    public abstract class Enemy
    {
        public abstract Task SendAttackRequest(int X, int Y);
        public virtual void SendAttackResult(AttackMessage msg) { }
        public virtual void SendChatMessage(string message) { }
        public virtual async Task SendGameMessage(GameMessageType message) { }
        public virtual async Task GiveTurn() { Battle.Info.ReverseTurn(); }
    }
}