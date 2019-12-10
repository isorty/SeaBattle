using System;
using System.Windows;
using System.Windows.Media;

namespace Battleships.Model
{
    public class PlayerField : Field
    {
        private IShip SelectedShip = null;

        public PlayerField()
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
                            for (int j = SelectedShip.J; j < SelectedShip.J + SelectedShip.Length; j++)
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

        public void CopyInfo(PlayerField f)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
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

        public bool TrySetShip(IShip ship, int i, int j)
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
}
