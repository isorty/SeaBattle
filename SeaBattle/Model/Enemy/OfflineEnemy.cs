using System.Threading;
using System.Threading.Tasks;

namespace Battleships.Model
{
    public class OfflineEnemy : Enemy
    {
        Bot bot;

        public OfflineEnemy()
        {
            bot = new Bot();
        }


        public override async Task SendAttackRequest(int X, int Y)
        {
            AttackMessage atcRes = bot.myField.GetAttackOnFieldResult(X, Y);
            Battle.Player.EnemyField.ReactToAttackResult(atcRes);

            if (atcRes.Result == AttackResult.Miss) await Battle.Enemy.GiveTurn();

            if (!Battle.Info.IsMyTurn) ThreadPool.QueueUserWorkItem(new WaitCallback(bot.AttackAsync));
        }
    }
}
