using Game.Core.Buildings;

namespace Game.Core.BuildingBehaviour.Factory
{
    public record struct BuildingBehaviourFactoryArgs(IBuilding Building, object BehaviourData);
}