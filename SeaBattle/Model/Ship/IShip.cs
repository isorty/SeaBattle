namespace Battleships.Model
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }


    public interface IShip
    {
        int Length { get; set; }
        int LifesLeft { get; set; }
        Orientation Orientation { get; set; }

        int I { get; set; }
        int J { get; set; }

        AttackResult Attacked();
        bool IsIntersectWith(IShip b);

    }
}
