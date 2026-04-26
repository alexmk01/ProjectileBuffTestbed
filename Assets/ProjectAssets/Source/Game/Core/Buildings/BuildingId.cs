using System;

namespace Game.Core.Buildings
{
    public readonly struct BuildingId : IEquatable<BuildingId>
    {
        public readonly int Value;

        public BuildingId(int value) => Value = value;
        public readonly bool Equals(BuildingId other) => Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}