using Game.Core.Map.Requests;
using Game.Core.Map.Services;
using MessagePipe;

namespace Game.Features.Map
{
    public sealed class MapEntityPlacementRequestHandler : IRequestHandler<MapEntityPlacementRequest, MapEntityPlacementResponse>
    {
        private readonly IGameMapService mapService;

        public MapEntityPlacementRequestHandler(IGameMapService mapService)
        {
            this.mapService = mapService;
        }
        
        public MapEntityPlacementResponse Invoke(MapEntityPlacementRequest request)
        {
            return new MapEntityPlacementResponse
            (
                mapService.IsEntityPlacementAllowed(request.Position),
                mapService.GetEntityPlacementPosition(request.Position)
            );
        }
    }
}