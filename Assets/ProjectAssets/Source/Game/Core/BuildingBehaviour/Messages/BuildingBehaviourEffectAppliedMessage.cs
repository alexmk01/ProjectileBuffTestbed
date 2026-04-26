
using Game.Core.Buildings;

namespace Game.Core.BuildingBehaviour.Messages
{
    public record struct BuildingBehaviourEffectAppliedMessage(IBuilding Building, IBuildingBehaviour Behaviour, object EffectTarget, float EffectAmount);
}