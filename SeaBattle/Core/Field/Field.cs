using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Battleships.Extensions;

namespace Battleships.Core
{
    public class UpdateableType<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UpdateableType(T i)
        {
            val = i;
        }

        private T val;
        public T Value
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged("Value");
            }
        }

        void OnPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propName));
        }
    }

    public abstract class Field : Grid, IField
    {
        public int ShipsLeft = 10;
        protected Cell[,] cellField;

        public List<IShip> settedShips = new List<IShip>();

        public ObservableCollection<UpdateableType<int>> UnsettedShipsByLength { get; set; } = new ObservableCollection<UpdateableType<int>>{
                new UpdateableType<int>(4),
                new UpdateableType<int>(3),
                new UpdateableType<int>(2),
                new UpdateableType<int>(1) };

        public Field()
        {
            Style TextBoxStyle = Application.Current.Resources["StandartTextBoxStyle"] as Style;

            //A...Z Row
            for (int i = 0; i < 10; i++)
            {
                TextBox t = new TextBox()
                {
                    IsReadOnly = true,
                    Style = TextBoxStyle,
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = ((char)('A' + i)).ToString()
                };

                SetRow(t, 0);
                SetColumn(t, i + 1);
                Children.Add(t);
            }

            //1..10 Row
            for (int i = 0; i < 10; i++)
            {
                TextBox t = new TextBox()
                {
                    IsReadOnly = true,
                    Style = TextBoxStyle,
                    FontSize = 20,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Text = (i + 1).ToString()
                };

                SetRow(t, i + 1);
                SetColumn(t, 0);
                Children.Add(t);
            }

            //Add game field
            cellField = new Cell[10, 10];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j] = new Cell(i, j);
                    SetRow(cellField[i, j], i + 1);
                    SetColumn(cellField[i, j], j + 1);
                    Children.Add(cellField[i, j]);
                }
        }

        public bool AllowDrop
        {
            set
            {
                for (int i = 0; i < 10; i++)
                    for (int j = 0; j < 10; j++)
                        cellField[i, j].AllowDrop = value;
            }
        }

        public async void ShipDestroyed(IShip s)
        {
            ShipsLeft--;
            SetBorderAfterDestroy(s);

            if (ShipsLeft == 0)
            {
                Game.Info.Result = (Game.Info.IsMyTurn) ? GameResult.Win : GameResult.Loss;
                Game.Info.Status = GameState.End;

                await Game.opponent.SendGameMessage(GameMessageType.End);   

                PageHelper.GoToPage(PageType.FightResult);
            }
        }

        public void SetBorderAfterDestroy(IShip s)
        {
            int mini, maxi, minj, maxj;

            mini = Math.Max(0, s.I - 1);
            minj = Math.Max(0, s.J - 1);
            maxi = (s.Orientation == Orientation.Horizontal) ? Math.Min(9, s.I + 1) : Math.Min(9, s.I + s.Length);
            maxj = (s.Orientation == Orientation.Vertical) ? Math.Min(9, s.J + 1) : Math.Min(9, s.J + s.Length);

            for (int i = mini; i <= maxi; i++)
                for (int j = minj; j <= maxj; j++)
                    cellField[i, j].Explored = true;
        }

        public Cell this[int i, int j]
        {
            get { return cellField[i, j]; }
            set { cellField[i, j] = value; }
        }

        public bool CanSetShip(IShip ship)
        {
            if (UnsettedShipsByLength[ship.Length - 1].Value <= 0) return false;

            if (ship.Orientation == Orientation.Horizontal) if (ship.J + ship.Length > 10) return false;
            if (ship.Orientation == Orientation.Vertical) if (ship.I + ship.Length > 10) return false;

            foreach (IShip s in settedShips)
                if (ship.IsIntersectWith(s)) return false;

            return true;
        }
    }

    public class MyField : Field
    {
        private IShip SelectedShip = null;

        //+
        public MyField()
        {
            Action<Cell> setDefaultEventHandlersToCell = delegate (Cell c)
            {
                c.DragEnter += MyField_DragEnter;
                c.DragLeave += MyField_DragLeave;
                c.Drop += MyField_Drop;
                c.Click += Cell_Click;
            };

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    setDefaultEventHandlersToCell(cellField[i, j]);
        }

        public void DeleteSelectedShip()
        {
            if (SelectedShip == null) return;

            switch (SelectedShip.Orientation)
            {
                case Orientation.Horizontal:
                    {
                        for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
                        {
                            this[SelectedShip.I, j].State = false;
                            this[SelectedShip.I, j].Background = Brushes.Transparent;
                            //    this[SelectedShip.I, j].Selected = false;
                        }
                        break;
                    }
                case Orientation.Vertical:
                    {
                        for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                        {
                            this[i, SelectedShip.J].State = false;
                            this[i, SelectedShip.J].Background = Brushes.Transparent;
                            //     this[i, SelectedShip.J].Selected = false;
                        }
                        break;
                    }
            }
            
            settedShips.Remove(SelectedShip);
            UnsettedShipsByLength[SelectedShip.Length - 1].Value++;

            SelectedShip = null;
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Cell c = sender as Cell;

            if (SelectedShip != null)
            {
                switch (SelectedShip.Orientation)
                {
                    case Orientation.Horizontal:
                        {
                            for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
                                this[SelectedShip.I, j].Background = Brushes.Transparent;
                            break;
                        }
                    case Orientation.Vertical:
                        {
                            for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                                this[i, SelectedShip.J].Background = Brushes.Transparent;
                            break;
                        }
                }
            }

            SelectedShip = GetShipByIJCoordinates(c.I, c.J);  
            
            if (SelectedShip != null)
            {
                switch (SelectedShip.Orientation)
                {
                    case Orientation.Horizontal:
                        {
                            for (int j = SelectedShip.J ; j < SelectedShip.J + SelectedShip.Length; j++)
                                this[SelectedShip.I, j].Background = Brushes.Green;
                            break;
                        }
                    case Orientation.Vertical:
                        {
                            for (int i = SelectedShip.I; i < SelectedShip.I + SelectedShip.Length; i++)
                                this[i, SelectedShip.J].Background = Brushes.Green;
                            break;
                        }
                }
            }
        }

        private void MyField_Drop(object sender, DragEventArgs e)
        {
            Cell c = sender as Cell;
            ShipVisual s = e.Data.GetData("Ship") as ShipVisual;

            TrySetShip(s, c.I, c.J);

            MyField_DragLeave(sender, e);
        }

        private void MyField_DragLeave(object sender, DragEventArgs e)
        {
            Cell c = sender as Cell;
            ShipVisual s = e.Data.GetData("Ship") as ShipVisual;

            if (s.Orientation == Orientation.Horizontal)
            {
                for (int j = c.J; j < c.J + s.Length; j++)
                    if (j >= 0 && j < 10) cellField[c.I, j].Background = Brushes.Transparent;
            }
            else
            {
                for (int i = c.I; i < c.I + s.Length; i++)
                    if (i >= 0 && i < 10) cellField[i, c.J].Background = Brushes.Transparent;
            }
        }

        private void MyField_DragEnter(object sender, DragEventArgs e)
        {
            Cell c = sender as Cell;
            ShipVisual s = e.Data.GetData("Ship") as ShipVisual;

            s.I = c.I;
            s.J = c.J;
            bool res = CanSetShip(s);

            if (s.Orientation == Orientation.Horizontal)
            {
                for (int j = c.J; j < c.J + s.Length; j++)
                    if (j >= 0 && j < 10) cellField[c.I, j].Background = (res) ? Brushes.GreenYellow : Brushes.IndianRed;
            }
            else
            {
                for (int i = c.I; i < c.I + s.Length; i++)
                    if (i >= 0 && i < 10) cellField[i, c.J].Background = (res) ? Brushes.GreenYellow : Brushes.IndianRed;
            }
        }

        protected IShip GetShipByIJCoordinates(int i, int j)
        {
            foreach (IShip s in settedShips)
            {
                if (s.I == i && s.Orientation == Orientation.Horizontal && (s.J <= j && j < s.J + s.Length)) return s;
                if (s.J == j && s.Orientation == Orientation.Vertical && (s.I <= i && i < s.I + s.Length)) return s;
            }
            return null;
        }

        public void CopyInfo(MyField f)
        {
            for (int i=0;i<10;i++)
                for (int j = 0; j< 10; j++)
                    cellField[i, j].State = f[i, j].State;
            settedShips = f.settedShips;
        }

        public AttackMessage GetAttackOnFieldResult(int i, int j)
        {
            cellField[i, j].Explored = true;

            IShip ship = GetShipByIJCoordinates(i, j);

            if (ship == null) return new AttackMessage(AttackResult.Miss, i, j);
            else
            {
                if (ship.Attacked() == AttackResult.Destroy)
                {
                    ShipDestroyed(ship);
                    return new AttackMessage(AttackResult.Destroy, i, j, ship);
                }
                else
                    return new AttackMessage(AttackResult.Hit, i, j);
            }
        }

        public bool TrySetShip(IShip ship, int i,int j)
        {
            ship.I = i;
            ship.J = j;

            if (!CanSetShip(ship)) return false;

            settedShips.Add(new ShipModel(ship.Length, ship.Orientation, ship.I, ship.J));

            switch (ship.Orientation)
            { 
                case Orientation.Horizontal:
                    {
                        for (int _j = j; _j < j + ship.Length; _j++)
                            cellField[i, _j].State = true;
                        break;
                    }
                case Orientation.Vertical:
                    {
                        for (int _i = i; _i < i + ship.Length; _i++)
                            cellField[_i, j].State = true;
                        break;
                    }
            }

            UnsettedShipsByLength[ship.Length - 1].Value--;
            return true;
        }

        public void Reset()
        {
            UnsettedShipsByLength[0].Value = 4;
            UnsettedShipsByLength[1].Value = 3;
            UnsettedShipsByLength[2].Value = 2;
            UnsettedShipsByLength[3].Value = 1;


            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    cellField[i, j].State = false;
                    cellField[i, j].Explored = false;
                    cellField[i, j].Background = Brushes.Transparent;
                }

            settedShips.Clear();
        }

        public void SetRandom()
        {
            this.Reset();

            for (int ii = 3; ii >= 0; ii--)
                while (UnsettedShipsByLength[ii].Value > 0)
                {
                    Random rnd = new Random();
                    int i, j, o;
                    Orientation or;

                    IShip ship;

                    do
                    {
                        i = rnd.Next(10);
                        j = rnd.Next(10);
                        o = rnd.Next(2);

                        if (o == 0) or = Orientation.Horizontal;
                        else or = Orientation.Vertical;


                        ship = new ShipModel(ii + 1);
                        ship.I = i;
                        ship.J = j;
                        ship.Orientation = or;

                    } while (!TrySetShip(ship, i, j));
                }
        }
    }

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
                case AttackResult.Miss:    { c.State = false; break; }
                case AttackResult.Hit:     { c.State = true;  break; }
                case AttackResult.Destroy: { c.State = true;
                                             UnsettedShipsByLength[atckMsg.Ship.Length-1].Value--;
                                             ShipDestroyed(atckMsg.Ship);
                                             break; }
            }
        }

        private async void EnemyField_Click(object sender, RoutedEventArgs e)
        {
            Cell c = sender as Cell;
            if (!Game.Info.IsMyTurn || c.Explored || !Game.Info.IsOpponentReady) return;

            await Game.opponent.SendAttackRequest(c.I, c.J);
        }
    }
}
