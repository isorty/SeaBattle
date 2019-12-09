using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Shapes;

namespace TestPages
{
    public class Ship : Button
    {
        public int Length { get; set; }
        public Orientation Orientation { get; set; }
        public int Health { get; set; }

        public int Y;
        public int X;

        public Image image;

        public bool canMove = false;

        public Ship()
        {
        //    Background = System.Windows.Media.Brushes.Transparent;
        //    BorderBrush = System.Windows.Media.Brushes.Blue;
        //    BorderThickness = new Thickness(4);

            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            PreviewMouseDown += Ship_PreviewMouseDown;
            PreviewMouseUp += Ship_PreviewMouseUp;

            Image correct = new Image()
            {
                Source = new BitmapImage(new Uri(@"F:\VS\BattleShipsTrainings\Ship4Correct.png", UriKind.Relative))
            };      
        }

        public Ship(int length, Orientation o, int x =0 , int y = 0) : this()
        {
            this.Length = length;
            this.X = x;
            this.Y = y;
            this.Orientation = o;
        }

        public Ship(int length) : this()
        {
            Length = length;
        }


        private void Ship_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            canMove = false;
        }

        private void Ship_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            canMove = true;
            DataObject data = new DataObject();
            data.SetData("Ship", this);
            DragDrop.DoDragDrop(this, data, DragDropEffects.All);
        }
    }
}
