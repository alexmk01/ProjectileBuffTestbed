using Common.Unity.Editor;
using Game.Core.Data;
using UnityEngine;

namespace Game.Infrastructure.Data
{
    [CreateAssetMenu]
    public sealed class GameConfigAsset : ScriptableObject
    {
        [LayerField]
        public int PlayerBuildingsLayer;
        [LayerField]
        public int EnemyBuildingsLayer;

        public GameConfig CreateConfig()
        {
            return new()
            {
                PlayerBuildingsLayer = PlayerBuildingsLayer,
                EnemyBuildingsLayer = EnemyBuildingsLayer
            };
        }
    }
}