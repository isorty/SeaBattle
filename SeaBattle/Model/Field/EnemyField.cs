using System.Windows;

namespace Battleships.Model
{
    public class EnemyField : Field
    {
        public EnemyField()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    cellField[i, j].Click += EnemyField_Click;
        }

        public async void ReactToAttackResult(AttackMessage atckMsg)
        {
            ICell c = cellField[atckMsg.X, atckMsg.Y];
            c.Explored = true;

            switch (atckMsg.Result)
            {
                case AttackResult.Miss: { c.State = false; break; }
                case AttackResult.Hit: { c.State = true; break; }
                case AttackResult.Destroy:
                    {
                        c.State = true;
                        UnsettedShipsByLength[atckMsg.Ship.Length - 1].Value--;
                        ShipDestroyed(atckMsg.Ship);
                        break;
                    }
            }
        }

        private async void EnemyField_Click(object sender, RoutedEventArgs e)
        {
            Cell c = sender as Cell;
            if (!Battle.Info.IsMyTurn || c.Explored || !Battle.Info.IsOpponentReady) return;

            await Battle.Enemy.SendAttackRequest(c.I, c.J);
        }
    }
}
