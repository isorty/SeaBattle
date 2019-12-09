namespace Battleships.Core
{
    public interface IField
    {
        void ShipDestroyed(IShip s);

        void SetBorderAfterDestroy(IShip s);
    }
}
