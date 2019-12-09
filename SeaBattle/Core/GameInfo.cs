using System.ComponentModel;

namespace Battleships.Core
{
    public enum GameState
    {
        NotStarted,
        Ready,
        Running,
        End
    }

    public enum GameResult
    {
        Win,
        Loss
    }

    public enum AttackResult
    {
        Hit,
        Miss,
        Destroy,
        Loss,
        Win
    }

    public class GameInfo: INotifyPropertyChanged 
    {
        public GameInfo()
        {
            this.Reset();
        }

        void OnPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propName));
        }

        private bool isHost;
        public bool IsHost { get { IsMyTurn = true; return isHost; } set { isHost = value; } }

        private bool isIReady;
        public bool IsIReady
        {
            get
            {
                return isIReady;
            }
            set
            {
                isIReady = value;
                OnPropertyChanged("IsIReady");
            }
        }

        private bool isOpponentReady;
        public bool IsOpponentReady
        {
            get
            {
                return isOpponentReady;
            }
            set
            {
                isOpponentReady = value;
                OnPropertyChanged("IsOpponentReady");
            }
        }

        public void ReverseTurn()
        {
            IsMyTurn = (IsMyTurn == true) ? false : true;
        }

        private bool myTurn = false;
        public bool IsMyTurn
        {
            get
            {
                return myTurn;
            }
            set
            {
                myTurn = value;
                OnPropertyChanged("IsMyTurn");
            }

        }

        private GameState status;
        public GameState Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private GameResult result;
        public GameResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Reset()
        {
            Status = GameState.NotStarted;
            IsIReady = false;
            IsOpponentReady = false;
        }
    }
}