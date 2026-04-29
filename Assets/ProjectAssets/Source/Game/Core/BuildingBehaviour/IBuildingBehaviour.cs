
namespace Game.Core.BuildingBehaviour
{
    public interface IBuildingBehaviour
    {
        IBuildingBehaviourData BehaviourData { get; }
        bool IsActive { get; set; }

        void Initialize();
    }
}