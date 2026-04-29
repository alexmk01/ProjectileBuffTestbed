using System;
using Game.Core.Map;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Events;
using MessagePipe;
using VContainer;

namespace Game.Features.Projectiles.Processors
{
    [Serializable]
    public sealed class ProjectileCellChangeEventProcessor : IProjectileProcessor
    {
        private IGameMap gameMap;
        private IPublisher<ProjectileCellChangedMessage> cellChangedMessagePublisher;
        private int[] projectileCellIndices;
        private ProjectileEmitter projectileEmitter;

        [Inject]
        private void Construct(IGameMap gameMap, IPublisher<ProjectileCellChangedMessage> cellChangedMessagePublisher)
        {
            this.gameMap = gameMap;
            this.cellChangedMessagePublisher = cellChangedMessagePublisher;
        }
        
        public object Clone() => new ProjectileCellChangeEventProcessor();

        public void Initialize(ProjectileEmitter emitter)
        {
            projectileEmitter = emitter;
            projectileCellIndices = new int[64];
            emitter.AddProjectileProcessor(new ProjectileAdditionalDataSyncProcessor<int>(projectileCellIndices, -1));
        }

        public void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                ref Projectile projectile = ref projectiles[i];
                ref int cellIndex = ref projectileCellIndices[i];
                int newCellIndex = gameMap.GetCellIndex(projectile.Position);

                if (cellIndex != newCellIndex)
                {
                    cellChangedMessagePublisher.Publish(new ProjectileCellChangedMessage(projectileEmitter, i, cellIndex, newCellIndex));
                    cellIndex = newCellIndex;
                }
            }
        }

        public void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles)
        {
        }
        
        public void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles)
        {
        }
        
        public void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles)
        {
        }
    }
}