using Game.Core.BuildingBehaviour;
using Game.Core.BuildingBehaviour.Factory;
using Game.Features.BuildingBehaviour.Behaviours;
using VContainer.Unity;

namespace Game.Features.BuildingBehaviour.Factory
{
    public sealed class BuildingBehaviourFactory : IBuildingBehaviourFactory
    {
        private readonly LifetimeScope scope;

        public BuildingBehaviourFactory(LifetimeScope scope)
        {
            this.scope = scope;
        }

        public IBuildingBehaviour CreateBehaviour(in BuildingBehaviourFactoryArgs args)
        {
            IBuildingBehaviour behaviour = null;
            
            switch (args.BehaviourData)
            {
                case ShootingBuildingBehaviour.Data data:
                    behaviour = new ShootingBuildingBehaviour(data, args.Building);
                    break;

                case ModifyProjectileDamageBuildingBehaviour.Data data:
                    behaviour = new ModifyProjectileDamageBuildingBehaviour(data, args.Building);
                    break;
                
                case ModifyProjectileHitCountBuildingBehaviour.Data data:
                    behaviour = new ModifyProjectileHitCountBuildingBehaviour(data, args.Building);
                    break;
                
                case CloneProjectileBuildingBehaviour.Data data:
                    behaviour = new CloneProjectileBuildingBehaviour(data, args.Building);
                    break;
            }

            if (behaviour != null)
            {
                scope.Container.Inject(behaviour);
            }

            return behaviour;
        }
    }
}