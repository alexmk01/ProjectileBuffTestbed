using System;
using Game.Core.Map.Services;
using Game.Core.Projectiles;
using Game.Core.Projectiles.Events;
using MessagePipe;
using VContainer;

namespace Game.Features.Projectiles.Behaviours
{
    [Serializable]
    public sealed class ProjectilesCellChangeEventBehaviour : IProjectilesBehaviour
    {
        private IGameMapService gameMapService;
        private IPublisher<ProjectileCellChangedMessage> cellChangedMessagePublisher;
        private int[] projectileCellIndices;
        private ProjectileEmitter projectileEmitter;

        [Inject]
        private void Construct(IGameMapService gameMapService, IPublisher<ProjectileCellChangedMessage> cellChangedMessagePublisher)
        {
            this.gameMapService = gameMapService;
            this.cellChangedMessagePublisher = cellChangedMessagePublisher;
        }
        
        public object Clone() => new ProjectilesCellChangeEventBehaviour();

        public void Initialize(ProjectileEmitter emitter)
        {
            projectileEmitter = emitter;
            projectileCellIndices = new int[64];
        }

        public void ModifyProjectiles(Span<Projectile> projectiles, float time, float deltaTime)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                ref Projectile projectile = ref projectiles[i];
                ref int cellIndex = ref projectileCellIndices[i];
                int newCellIndex = gameMapService.GetCellIndex(projectile.Position);

                if (cellIndex != newCellIndex)
                {
                    cellChangedMessagePublisher.Publish(new ProjectileCellChangedMessage(projectileEmitter, i, cellIndex, newCellIndex));
                    cellIndex = newCellIndex;
                }
            }
        }
        //TODO: implement additional data synchronizator as IProjectilesBehaviour
        public void OnNewProjectilesLaunched(int startIndex, int endIndex, Span<Projectile> projectiles)
        {
            if (projectiles.Length > projectileCellIndices.Length)
            {
                Array.Resize(ref projectileCellIndices, (int)(projectiles.Length * 1.5f));
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                projectileCellIndices[i] = -1;
            }
        }

        public void OnProjectileIndexChanged(int lastIndex, int newIndex, Span<Projectile> projectiles)
        {
            projectileCellIndices[newIndex] = projectileCellIndices[lastIndex];
        }
        
        public void OnProjectileDestroyed(int projectileIndex, Span<Projectile> projectiles)
        {
        }
    }
}