using Common;
using Common.Unity;
using Game.Core.Buildings;
using Game.Core.Construction.Services;
using Game.Infrastructure;
using UnityEngine;
using VContainer;

namespace Game.Features.Construction
{
    public sealed class BuildingsConstructionRootComponent : MonoBehaviour
    {
        private IBuildingConstructionService constructionService;
        
        [Inject]
        private void Construct(IBuildingConstructionService constructionService)
        {
            this.constructionService = constructionService;
        }
        
        private void Start()
        {
            int childCount = transform.childCount;
            
            for (int i = 0; i < childCount; i++)
            {
                GameObject spawnPivot = transform.GetChild(i).gameObject;
                if (!spawnPivot.activeSelf) continue;
                
                if (spawnPivot.TryGetComponent(out IIdentifiable<BuildingId> idProvider))
                {
                    //TODO: pass values from description component (max hp etc)
                    if (constructionService.TryConstructBuilding(idProvider.Id, spawnPivot.transform.position.ToNumericsVector2()))
                    {
                        Destroy(spawnPivot);
                    }
                    else
                    {
                        Common.DebugUtils.LogError($"Failed to construct building {idProvider.Id.ToTag()}");
                    }
                }
            }
        }
    }
}