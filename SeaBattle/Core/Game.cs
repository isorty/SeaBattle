using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Battleships.Core
{
    public enum GameType
    {
        VsHuman,
        VsAI
    }


    public class Me
    {
        public MyField myField { get; set; }
        public EnemyField enemyField { get; set; }
    }

    public class Game
    {
        //+-
        public static Me me;
        public static Opponent opponent;
        public static ConnectionManager ConnectionManager;
        public static GameInfo Info;

        public Game(GameType type)
        {
            me = new Me();
            Info = new GameInfo();
            ConnectionManager = new ConnectionManager();

            switch (type)
            {
                case GameType.VsHuman:
                    {
                        opponent = new NetOpponent();
                        break;
                    }
                case GameType.VsAI:
                    {
                        Game.Info.IsHost = true;
                        Game.Info.IsMyTurn = true;

                        opponent = new AIOppenent();
                        break;
                    }
            }
        }
    }

    public abstract class Opponent
    {
        public abstract Task SendAttackRequest(int X, int Y);
        public virtual void SendAttackResult(AttackMessage msg) { }
        public virtual void SendChatMessage(string message) { }
        public virtual async Task SendGameMessage(GameMessageType message) { }
        public virtual async Task GiveTurn() { Game.Info.ReverseTurn(); }
    }

    public class NetOpponent : Opponent
    {
        public override async Task SendAttackRequest(int X, int Y)
        {
            await Game.ConnectionManager.SendMessage(MessageType.AttackRequest, new Point(X, Y));
        }

        public override async void SendAttackResult(AttackMessage message)
        {
            await Game.ConnectionManager.SendMessage(MessageType.AttackResult, message);
        }

        public override async void SendChatMessage(string message)
        {
            await Game.ConnectionManager.SendMessage(MessageType.Chat, message);
        }

        public async override Task SendGameMessage(GameMessageType message)
        {
            await Game.ConnectionManager.SendMessage(MessageType.Game, message);
        }

        public async override Task GiveTurn()
        {
            await SendGameMessage(GameMessageType.YourTurn);
            await base.GiveTurn();
        }
    }

    public class AIOppenent : Opponent
    {
        Bot bot;

        public AIOppenent()
        {
            bot = new Bot();
        }


        public override async Task SendAttackRequest(int X, int Y)
        {
            AttackMessage atcRes = bot.myField.GetAttackOnFieldResult(X, Y);
            Game.me.enemyField.ReactToAttackResult(atcRes);

            if (atcRes.Result == AttackResult.Miss) await Game.opponent.GiveTurn();

            if (!Game.Info.IsMyTurn) ThreadPool.QueueUserWorkItem(new WaitCallback(bot.AttackAsync));
        }
    }

    public class Bot
    {
        public MyField myField;
        private EnemyField enemyField;

        List<Point> pointList = new List<Point>();

        //+
        public Bot()
        {
            myField = new MyField();
            enemyField = new EnemyField();
            myField.SetRandom();
            Game.Info.IsOpponentReady = true;
        }

        private void RemovePoints()
        {
            if (enemyField.UnsettedShipsByLength[0].Value > 0) return;

            int length = 2;
            for (; length < 4; length++)
                if (enemyField.UnsettedShipsByLength[length - 1].Value > 0) break;

            byte[,] map = new byte[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    if (enemyField.UnsettedShipsByLength[length-1].Value <= 0) continue;

                    if (enemyField.CanSetShip(new ShipModel(length, Orientation.Horizontal, i, j)))
                    {
                        for (int _j = j; _j < j + length; _j++)
                            map[i, _j] = 1;
                    }

                    if (enemyField.CanSetShip(new ShipModel(length, Orientation.Vertical, i, j)))
                    {
                        for (int _i = i; _i < i + length; _i++)
                            map[_i, j] = 1;
                    }
                }

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j] == 0) enemyField[i, j].Explored = true;
                }
        }


        private Point GuessPointSmart()
        {
            Point p = new Point();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    if (enemyField[i, j].Explored)
                    {
                        if (enemyField[Math.Min(i + 1, 9), j].Explored &&
                            enemyField[Math.Max(0, i - 1), j].Explored &&
                            enemyField[i, Math.Min(j + 1, 9)].Explored &&
                            enemyField[i, Math.Max(0, j - 1)].Explored) continue;


                        if (enemyField[i, j].State)
                        {
                            if (i + 1 < 10 && !enemyField[i + 1, j].Explored) p = new Point(i + 1, j);
                            if (i - 1 >= 0 && !enemyField[i - 1, j].Explored) p = new Point(i - 1, j);
                            if (j + 1 < 10 && !enemyField[i, j + 1].Explored) p = new Point(i, j + 1);
                            if (j - 1 >= 0 && !enemyField[i, j - 1].Explored) p = new Point(i, j - 1);


                            if (i + 1 < 10 && !enemyField[i + 1, j].Explored && i - 1 >= 0 && enemyField[i - 1, j].State) return new Point(i + 1, j);
                            if (i - 1 >= 0 && !enemyField[i - 1, j].Explored && i + 1 < 10 && enemyField[i + 1, j].State) return new Point(i - 1, j);
                            if (j + 1 < 10 && !enemyField[i, j + 1].Explored && j - 1 >= 0 && enemyField[i, j - 1].State) return new Point(i, j + 1);
                            if (j - 1 >= 0 && !enemyField[i, j - 1].Explored && j + 1 < 10 && enemyField[i, j + 1].State) return new Point(i, j - 1);
                        }
                    }
                }

            RemovePoints();

            if (enemyField[(int)p.X, (int)p.Y].Explored)
            {
                int x, y;
                Random rnd = new Random();
                do
                {
                    x = rnd.Next(10);
                    y = rnd.Next(10);
                    p.X = x;
                    p.Y = y;
                } while (enemyField[x, y].Explored);
            }

            return p;
        }

        public void AttackAsync(object o)
        {
            while (!Game.Info.IsMyTurn && Game.Info.Status != GameState.End)
            {
                Thread.Sleep(350);

                int X, Y;

                Point p = new Point();
                Application.Current.Dispatcher.Invoke(new Action(() => { p = GuessPointSmart(); }));

                X = (int)p.X; Y = (int)p.Y;

                bool IfExplored = false;
                Application.Current.Dispatcher.Invoke(new Action(() => { IfExplored = enemyField[X, Y].Explored; }));
                if (IfExplored == true) continue;

                AttackMessage res;
                res.Result = AttackResult.Miss;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    res = Game.me.myField.GetAttackOnFieldResult(X, Y);
                    enemyField.ReactToAttackResult(res);
                }
                ));

                if (res.Result == AttackResult.Miss) Game.Info.ReverseTurn();
            }
        }
    }
}
