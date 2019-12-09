using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Battleships.Core
{
    public class ShipVisual : Button, IShip
    {
        public int Length { get; set; }
        public int LifesLeft { get; set; }
        public Orientation Orientation { get; set; }

        public int I { get; set; }
        public int J { get; set; }


        public Image image;

      
        public ShipVisual()
        {
            PreviewMouseDown += Ship_PreviewMouseDown;
        }

        public ShipVisual(int length, Orientation o, int i = 0 , int j = 0) : this()
        {
            this.Length = length;
            this.I = i;
            this.J = j;
            this.Orientation = o;
            LifesLeft = Length;
        }

        public ShipVisual(int length) : this()
        {
            Length = length;
            LifesLeft = length;
        }

        private void Ship_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject();
            data.SetData("Ship", this);
            DragDrop.DoDragDrop(this, data, DragDropEffects.All);
        }

        public AttackResult Attacked()
        {
            return (--LifesLeft == 0) ? AttackResult.Destroy : AttackResult.Hit;
        }

        //+
        public bool IsIntersectWith(IShip b)
        {
            int a_mini = this.I - 1;
            int a_minj = this.J - 1;
            int a_maxi = (this.Orientation == Orientation.Vertical) ? this.I + this.Length : this.I + 1;
            int a_maxj = (this.Orientation == Orientation.Horizontal) ? this.J + this.Length : this.J + 1;

            int b_mini = b.I;
            int b_minj = b.J;
            int b_maxi = (b.Orientation == Orientation.Vertical) ? b.I + b.Length - 1 : b.I;
            int b_maxj = (b.Orientation == Orientation.Horizontal) ? b.J + b.Length - 1 : b.J;

            for (int ai = a_mini; ai <= a_maxi; ai++)
                for (int aj = a_minj; aj <= a_maxj; aj++)
                    for (int bi = b_mini; bi <= b_maxi; bi++)
                        for (int bj = b_minj; bj <= b_maxj; bj++)
                            if (ai == bi && aj == bj) return true;

            return false;
        }
    }
}