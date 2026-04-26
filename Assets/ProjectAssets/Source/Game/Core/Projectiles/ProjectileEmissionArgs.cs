using System.Numerics;

namespace Game.Core.Projectiles
{
    public record struct ProjectileEmissionArgs(Vector2 Position, Vector2 Direction, float DamageOverride = 0f, int ProjectileGeneration = 1);
}