using Game.Core.Map;
using UnityEngine;

namespace Game.Features.Map
{
    public interface IGameMapLoader
    {
        void LoadMap(out IGameMapGrid mapGrid, out GameMapArea[] entityPlacementAreas, out Transform[] mapEntityRoots);
    }
}