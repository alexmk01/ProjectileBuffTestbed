using Common.Unity;
using Common.Tags;
using Common.Unity.Tags.Hierarchical;
using Game.Core.Buildings;

namespace Game.Infrastructure
{
    public static class IdExtensions
    {
        public static bool IsValid(this BuildingId buildingId) => buildingId.ToTag().HasValue();
        public static BuildingId ToBuildingId(this int value) => new(value);
        public static BuildingId ToBuildingId(this Tag tag) => ((int)tag).ToBuildingId();
        public static BuildingId ToBuildingId(this SerializableGUID id) => ((int)new HierarchicalTagReference(id).GetValue()).ToBuildingId();
        public static Tag ToTag(this BuildingId buildingId) => new(buildingId.Value);
        public static SerializableGUID ToGuid(this BuildingId buildingId) => buildingId.ToTag().ToGuid();
    }
}