using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleships.Extensions;
using Battleships.Windows;

namespace Battleships.Core
{
    public enum MessageType
    {
        Chat,
        AttackResult,
        AttackRequest,
        Game
    }

    public enum GameMessageType
    {
        Ready,
        YourTurn,
        End
    }

    public struct AttackMessage
    {
        public AttackResult Result;
        public int X;
        public int Y;
        public IShip Ship;

        public AttackMessage(AttackResult res, int x, int y)
        {
            Result = res;
            X = x;
            Y = y;

            Ship = null;
        }

        public AttackMessage(AttackResult res, int i, int j, IShip ship)
        {
            Result = res;

            X = i;
            Y = j;

            Ship = ship;
        }
    }

    public class ConnectionManager
    {
        //~ConnectionManager()
        //{
        //    tcpClient.Dispose();
        //    tcpListener.Stop();
        //    tcpListener = null;
        //}

        private TcpClient tcpClient;
        public Socket Client
        {
            get
            {
                return tcpClient.Client;
            }
        }

        private TcpListener tcpListener;
        public Socket Listener
        {
            get
            {
                return tcpClient.Client;
            }
        }

        private NetworkStream NtStream;

        public RichTextBox ChatStream;

        #region Recivers

        private async Task<string> ReciveAsync()
        {
            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
            int bytesReceive = await NtStream.ReadAsync(buffer, 0, tcpClient.ReceiveBufferSize);
            string recive = Encoding.Default.GetString(buffer, 0, bytesReceive);

            return recive;
        }

        public async Task RunRecive()
        {
            while (tcpClient.Connected && Game.Info.Status != GameState.End)
            {
                try
                {
                    string recive = await ReciveAsync();

                    string type = recive.Split(' ')[0];
                    recive = recive.Replace(type, "").Remove(0, 1);

                    switch (MessageFormtatter.GetType(type))
                    {
                        case MessageType.AttackResult : { await AttackResultMessageRecived(recive);  break; }
                        case MessageType.AttackRequest: { await AttackRequestMessageRecived(recive); break; }
                        case MessageType.Chat         : { await ChatMessageRecived(recive);          break; }
                        case MessageType.Game         : { await GameMessageRecieved(recive);         break; }
                        default                       : { throw new Exception("Unknown Message type!");     }
                    }
                }
                catch (Exception ex)
                {
                    Game.Info.Status = GameState.End;
                    Notification n = new Notification("Lost connection...");
                    n.ShowDialog();

                    CloseAllConnections();
                    PageHelper.GoToPage(PageType.Login);
                    return;
                }
            }
        }

        private async Task AttackRequestMessageRecived(string message)
        {
            Point p = (Point)MessageFormtatter.GetInfo(MessageType.AttackRequest, message);
            int i = (int)p.X;
            int j = (int)p.Y;

            await SendMessage(MessageType.AttackResult, Game.me.myField.GetAttackOnFieldResult(i,j));
        }

        private async Task AttackResultMessageRecived(string message)
        {
            AttackMessage atckMsg = (AttackMessage)MessageFormtatter.GetInfo(MessageType.AttackResult, message);

            if (atckMsg.Result == AttackResult.Miss) await Game.opponent.GiveTurn();

            Game.me.enemyField.ReactToAttackResult(atckMsg);
        }
    
        private async Task GameMessageRecieved(string msg)
        {
            var gmsgType = MessageFormtatter.GetInfo(MessageType.Game, msg);

            //switch (gmsgType)
            //{
            //    case GameMessageType.Ready   : { Game.Info.IsOpponentReady = true; break; }
            //    case GameMessageType.YourTurn: { Game.Info.IsMyTurn = true;        break; }
            //    case GameMessageType.End     : {
            //            Game.Info.Status = GameState.End;
            //            CloseAllConnections();
            //            PageHelper.GoToPage(PageType.FightResult);
            //            break;
            //        }
            //}
        }

        private async Task ChatMessageRecived(string message)
        {
            ChatStream.AppendText("Enemy: " + message, Brushes.Red);
        }

        #endregion

        #region Senders 

        private async Task SendAsync(string message)
        {
            byte[] buffer = Encoding.Default.GetBytes(message);
            await NtStream.WriteAsync(buffer, 0, message.Length);
        }

        public async Task SendMessage(MessageType msgType, object msgInfo = null)
        {
            if (tcpClient.Connected)
            {
                switch (msgType)
                {
                    case MessageType.AttackRequest: { await SendAttackRequestMessage((Point)msgInfo);        break; }
                    case MessageType.AttackResult:  { await SendAttackResultMessage((AttackMessage)msgInfo); break; }
                    case MessageType.Chat:          { await SendChatMessage((string)msgInfo);                break; }
                    case MessageType.Game:          { await SendGameMessage((GameMessageType)msgInfo);       break; }
                }
            }
        }

        private async Task SendAttackResultMessage(AttackMessage msg)
        {
            await SendAsync(MessageFormtatter.Concat(MessageType.AttackResult, msg));
        }

        private async Task SendAttackRequestMessage(Point p)
        { 
            await SendAsync(MessageFormtatter.Concat(MessageType.AttackRequest, p));
        }

        private async Task SendGameMessage(GameMessageType msg)
        {
            await SendAsync(MessageFormtatter.Concat(MessageType.Game, msg));

            if (msg == GameMessageType.End)
            {
                CloseAllConnections();
                Game.Info.Status = GameState.End;
            }
        }

        private async Task SendChatMessage(string msg)
        {
            await SendAsync(MessageFormtatter.Concat(MessageType.Chat, msg));
        }

        #endregion


        public void CloseAllConnections()
        {
            if (tcpClient != null) tcpClient.Close();
            if (tcpListener != null) tcpListener.Stop();
        }

        public async Task<bool> StartServer(IPAddress addr ,string ServerPort)
        {
            try
            {
                tcpListener = new TcpListener(addr, Convert.ToInt32(ServerPort));
                tcpListener.Start();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot start server at this IP/ Port");
            }
        }

        public async Task WaitServerForUsers()
        {
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            NtStream = tcpClient.GetStream();
            tcpListener.Stop();
        }

        public async Task ConnectTo(IPAddress ipAddr, int Port)
        {
            tcpClient = new TcpClient();
            try
            {
                await tcpClient.ConnectAsync(ipAddr, Port);

                if (tcpClient.Connected) NtStream = tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot connect to server!");
            }
        }   
    }
}
