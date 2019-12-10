using System.ComponentModel;

namespace Battleships.Model
{
    public enum BattleState
    {
        NotStarted,
        Ready,
        Running,
        End
    }

    public enum BattleResult
    {
        Victory,
        Defeat
    }

    public enum AttackResult
    {
        Hit,
        Miss,
        Destroy,
        Loss,
        Win
    }

    public class BattleInfo: INotifyPropertyChanged 
    {
        public BattleInfo()
        {
            this.Reset();
        }

        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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
            //IsMyTurn = (IsMyTurn == true) ? false : true;
            IsMyTurn = !IsMyTurn;
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

        private BattleState status;

        public BattleState Status
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

        private BattleResult result;

        public BattleResult Result
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
            Status = BattleState.NotStarted;
            IsIReady = false;
            IsOpponentReady = false;
        }
    }
}