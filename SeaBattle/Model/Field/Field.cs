using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Battleships.Extensions;

namespace Battleships.Model
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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

        public async void ShipDestroyed(IShip ship)
        {
            ShipsLeft--;
            SetBorderAfterDestroy(ship);

            if (ShipsLeft == 0)
            {
                Battle.Info.Result = (Battle.Info.IsMyTurn) ? BattleResult.Victory : BattleResult.Defeat;
                Battle.Info.Status = BattleState.End;

                await Battle.Enemy.SendGameMessage(GameMessageType.End);   

                PageHelper.GoToPage(PageType.FightResult);
            }
        }

        public void SetBorderAfterDestroy(IShip ship)
        {
            int mini, maxi, minj, maxj;

            mini = Math.Max(0, ship.I - 1);
            minj = Math.Max(0, ship.J - 1);
            maxi = (ship.Orientation == Orientation.Horizontal) ? Math.Min(9, ship.I + 1) : Math.Min(9, ship.I + ship.Length);
            maxj = (ship.Orientation == Orientation.Vertical) ? Math.Min(9, ship.J + 1) : Math.Min(9, ship.J + ship.Length);

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

}
