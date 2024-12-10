public interface IShipOwner
{
    FactionType Faction { get; }
    void AddShip(Ship ship);
    void RemoveShip(Ship ship);
    void SelectShip(Ship ship);
}