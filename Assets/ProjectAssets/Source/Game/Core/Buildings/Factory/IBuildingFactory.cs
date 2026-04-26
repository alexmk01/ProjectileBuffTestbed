
namespace Game.Core.Buildings.Factory
{
    public interface IBuildingFactory
    {
        IBuilding CreateBuilding(in BuildingFactoryArgs args);
    }
}