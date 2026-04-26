
namespace Game.Core.BuildingBehaviour.Factory
{
    public interface IBuildingBehaviourFactory
    {
        IBuildingBehaviour CreateBehaviour(in BuildingBehaviourFactoryArgs args);
    }
}