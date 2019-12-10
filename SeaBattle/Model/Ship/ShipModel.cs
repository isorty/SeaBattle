namespace Battleships.Model
{
    public class ShipModel : IShip
    {
        public int Length { get; set; }
        public int LifesLeft { get; set; }
        public Orientation Orientation { get; set; }
        public int I { get; set; }
        public int J { get; set; }

        public ShipModel(int length, Orientation o, int i = 0, int j = 0) 
        {
            this.Length = length;
            this.I = i;
            this.J = j;
            this.Orientation = o;
            LifesLeft = Length;
        }

        public ShipModel(int length) 
        {
            Length = length;
            LifesLeft = length;
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