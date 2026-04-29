using Game.Core.Map;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Map
{
    public sealed class GameMapFeatureInstaller : IInstaller
    {
        private readonly IGameMapLoader mapLoader;

        public GameMapFeatureInstaller(IGameMapLoader mapLoader)
        {
            this.mapLoader = mapLoader;
        }
        
        public void Install(IContainerBuilder builder)
        {
            mapLoader.LoadMap(out IGameMapGrid mapGrid, out GameMapArea[] entityPlacementAreas, out Transform[] mapEntityRoots);
            builder.Register<GameMap>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter(mapGrid)
                .WithParameter(entityPlacementAreas);
            
            builder.RegisterBuildCallback(resolver =>
            {
                //TODO: fix
                resolver.Resolve<IGameMap>();

                for (int i = 0; i < mapEntityRoots.Length; i++)
                {
                    resolver.InjectGameObject(mapEntityRoots[i].gameObject);
                }
            });
        }
    }
}