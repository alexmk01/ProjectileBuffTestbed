using System;

namespace Game.Core.Data
{
    [Serializable]
    public sealed class GameConfig
    {
        public int PlayerBuildingsLayer;
        public int EnemyBuildingsLayer;
        public float ProjectilesTimeSpeedUpMultiplier = 2f;
    }
}