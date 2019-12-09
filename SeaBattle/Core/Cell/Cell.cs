using System;
using System.Windows;
using System.Windows.Controls;

namespace Battleships.Core
{
    public class Cell : Button, ICell
    {
        public Boolean State
        {
            get { return (Boolean)GetValue(CellState); }
            set { SetValue(CellState, value); }
        }

        public static bool GetCellState(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellState);
        }

        public static void SetCellState(DependencyObject obj, bool value)
        {
            obj.SetValue(CellState, value);
        }

        public static readonly DependencyProperty CellState = DependencyProperty.Register("CellState", typeof(Boolean), typeof(Cell), new PropertyMetadata(false));
       
        //

        public Boolean Explored
        {
            get { return (Boolean)GetValue(CellExplored); }
            set { SetValue(CellExplored, value); }
        }

        public static bool GetCellExplored(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellExplored);
        }

        public static void SetCellExplored(DependencyObject obj, bool value)
        {
            obj.SetValue(CellExplored, value);
        }

        public static readonly DependencyProperty CellExplored = DependencyProperty.Register("CellExplored", typeof(Boolean), typeof(Cell), new PropertyMetadata(false));
        
        //

        public Boolean Selected
        {
            get { return (Boolean)GetValue(CellSelected); }
            set { SetValue(CellSelected, value); }
        }

        public static bool GetCellSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellSelected);
        }

        public static void SetCellSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(CellSelected, value);
        }

        public static readonly DependencyProperty CellSelected = DependencyProperty.Register("CellSelected", typeof(Boolean), typeof(Cell), new PropertyMetadata(false));

        public int I { get; set; }
        public int J { get; set; }

        public Cell()
        {
            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0.8);
            BorderBrush = System.Windows.Media.Brushes.Gray;
            Style = Application.Current.Resources["CellStyle"] as Style;
        }

        public Cell(int i, int j) : this()
        {
            this.I = i;
            this.J = j;
        }
    }
}
