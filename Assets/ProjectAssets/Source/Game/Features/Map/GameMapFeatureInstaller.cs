using Game.Core.Map;
using Game.Core.Map.Requests;
using Game.Core.Map.Services;
using Game.Features.Map.Services;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Features.Map
{
    public sealed class GameMapFeatureInstaller : IInstaller
    {
        private readonly IGameMapLoader mapLoader;
        private readonly MessagePipeOptions messagePipeOptions;

        public GameMapFeatureInstaller(IGameMapLoader mapLoader, MessagePipeOptions messagePipeOptions)
        {
            this.mapLoader = mapLoader;
            this.messagePipeOptions = messagePipeOptions;
        }

        public void Install(IContainerBuilder builder)
        {
            mapLoader.LoadMap(out IGameMapGrid mapGrid, out GameMapArea[] entityPlacementAreas, out Transform[] mapEntityRoots);
            builder.Register<GameMapService>(Lifetime.Singleton).AsImplementedInterfaces()
                .WithParameter(mapGrid)
                .WithParameter(entityPlacementAreas);
            builder.RegisterRequestHandler<MapEntityPlacementRequest, MapEntityPlacementResponse, MapEntityPlacementRequestHandler>(messagePipeOptions);
            
            builder.RegisterBuildCallback(resolver =>
            {
                //TODO: fix
                resolver.Resolve<IGameMapService>();

                for (int i = 0; i < mapEntityRoots.Length; i++)
                {
                    resolver.InjectGameObject(mapEntityRoots[i].gameObject);
                }
            });
        }
    }
}