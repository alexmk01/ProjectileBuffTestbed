using Game.Infrastructure.Entities;
using Game.Core.Buildings;
using Common;
using Common.Tags;
using Game.Infrastructure;

namespace Game.Features.Buildings
{
    public sealed class BuildingComponent : EntityMainComponent, IBuilding, IIdentifiable<Tag>
    {
        Tag IIdentifiable<Tag>.Id => Id.ToTag();
        
        public BuildingId Id { get; internal set; }
    }
}
