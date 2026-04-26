using Game.Infrastructure;
using Game.Infrastructure.Entities.Factory;
using Common;
using Game.Core.Buildings;

namespace Game.Features.Buildings.Factory
{
    public sealed class BuildingDescriptionComponent : EntityDescriptionComponent, IIdentifiable<BuildingId>
    {
        BuildingId IIdentifiable<BuildingId>.Id => Id.ToBuildingId();
        
        public bool IsEnemyBuilding;
    }
}