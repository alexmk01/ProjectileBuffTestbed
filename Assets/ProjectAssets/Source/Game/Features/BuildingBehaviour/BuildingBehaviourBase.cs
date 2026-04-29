using Game.Core.BuildingBehaviour;
using Game.Core.Buildings;

namespace Game.Features.BuildingBehaviour
{
    public abstract class BuildingBehaviourBase<TData> : IBuildingBehaviour 
        where TData : IBuildingBehaviourData
    {
        IBuildingBehaviourData IBuildingBehaviour.BehaviourData => BehaviourData;
        public bool IsActive { get; set; }
        
        protected readonly TData BehaviourData;
        protected readonly IBuilding Building;

        protected BuildingBehaviourBase(TData data, IBuilding building)
        {
            BehaviourData = data;
            Building = building;
            IsActive = true;
        }

        public virtual void Initialize() { }
    }
}