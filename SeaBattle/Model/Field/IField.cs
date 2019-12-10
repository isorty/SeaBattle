namespace Battleships.Model
{
    public interface IField
    {
        void ShipDestroyed(IShip s);

        void SetBorderAfterDestroy(IShip s);
    }
}
