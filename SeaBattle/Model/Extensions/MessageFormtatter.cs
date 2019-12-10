using System;
using Battleships.Model;

namespace Battleships.Extensions
{
    public class MessageFormtatter
    {
        public static object GetInfo(MessageType msgType, object msg)
        {
            switch (msgType)
            {
                case MessageType.AttackRequest:
                    {
                        string[] mesg = (msg as string).Split(' ');
 
                        int x = Int32.Parse(mesg[0]);
                        int y = Int32.Parse(mesg[1]);

                        return new System.Windows.Point(x, y);
                    }
                case MessageType.AttackResult:
                    {
                        string[] mesg = (msg as string).Split(' ');

                        AttackResult attackResult;

                        int I = Int32.Parse(mesg[1]);
                        int J = Int32.Parse(mesg[2]);

                        switch (mesg[0])
                        {
                            case "hit"    : { return new AttackMessage(AttackResult.Hit, I, J);}
                            case "miss"   : { return new AttackMessage(AttackResult.Miss, I, J); }
                            case "destroy": { return new AttackMessage(AttackResult.Destroy, I, J,
                                                     new ShipModel( Int32.Parse(mesg[3]),
                                                                    (mesg[4] == "h")? Orientation.Horizontal : Orientation.Vertical,
                                                                    Int32.Parse(mesg[5]),
                                                                    Int32.Parse(mesg[6]))); }
                            default       : { throw new Exception("Wrong message format!"); }
                        }
                    }
                case MessageType.Game:
                    {
                        string[] mesg = (msg as string).Split(' ');
                        switch (mesg[0])
                        {
                            case "end": { return GameMessageType.End; }
                            case "ready"   : { return GameMessageType.Ready;    }
                            case "yourturn": { return GameMessageType.YourTurn; }
                            default:
                                {
                                    throw new Exception("Unknow game message");
                                }
                        }
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public static MessageType GetType(string msg)
        {
            string type = (msg as String).Split(' ')[0];

            switch (type)
            {
                case "/attack_result":  { return MessageType.AttackResult;  }
                case "/attack_request": { return MessageType.AttackRequest; }
                case "/chat":           { return MessageType.Chat;          }
                case "/game":           { return MessageType.Game;          }
                default:
                    {
                        throw new Exception("Unknow message type");
                    }
            }

        }

        public static string Concat(MessageType msgType, object msgInfo)
        {
            string msg = "";
            //switch (msgType)
            //{
            //    case MessageType.AttackRequest:
            //        {
            //            var m = (System.Windows.Point)msgInfo;
            //            msg = ($"/attack_request {m.X} {m.Y}");
            //            break;
            //        }
            //    case MessageType.AttackResult:
            //        {
            //            var m = (AttackMessage)msgInfo;

            //            switch (m.Result)
            //            {
            //                case AttackResult.Miss:    { msg = ($"/attack_result {"miss"} {m.X} {m.Y}"); break; }
            //                case AttackResult.Hit:     { msg = ($"/attack_result {"hit"} {m.X} {m.Y}"); break; }
            //                case AttackResult.Destroy: {
            //                        string orientationStr = (m.Ship.Orientation == Orientation.Horizontal) ? "h" : "v";
            //                        msg = ($"/attack_result {"destroy"} {m.X} {m.Y}" +
            //                            $" {m.Ship.Length} {orientationStr} {m.Ship.I} {m.Ship.J}"); break; }
            //            }
            //            break;
            //        }
            //    case MessageType.Chat:
            //        {
            //            msg = ($"/chat {(string)msgInfo}");
            //            break;
            //        }
            //    case MessageType.Game:
            //        {
            //            switch (msgInfo)
            //            {
            //                case GameMessageType.End: { msg = ($"/game {"end"}"); break; }
            //                case GameMessageType.Ready: { msg = ($"/game {"ready"}"); break; }
            //                case GameMessageType.YourTurn: { msg = ($"/game {"yourturn"}"); break; }
            //            }
            //            break;
            //        }
            //}
            return msg;
        }
    }
}
