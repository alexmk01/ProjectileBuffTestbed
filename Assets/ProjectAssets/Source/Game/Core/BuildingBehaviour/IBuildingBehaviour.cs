
namespace Game.Core.BuildingBehaviour
{
    public interface IBuildingBehaviour
    {
        object BehaviourData { get; }
        bool IsActive { get; set; }

        void Initialize();
    }
}