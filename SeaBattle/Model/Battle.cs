namespace Battleships.Model
{
    public class Battle
    {     
        public static Player Player;

        public static Enemy Enemy;

        public static ServerManager ServerManager;

        public static BattleInfo Info;

        public Battle(BattleType type)
        {
            Player = new Player();
            Info = new BattleInfo();
            ServerManager = new ServerManager();

            switch (type)
            {
                case BattleType.Online:
                    {
                        Enemy = new OnlineEnemy();
                        break;
                    }
                case BattleType.Offline:
                    {
                        Info.IsHost = true;
                        Info.IsMyTurn = true;
                        Enemy = new OfflineEnemy();
                        break;
                    }
            }
        }
    }

    //типы игры (против игрока, против компьютера)
    public enum BattleType { Online, Offline }
}