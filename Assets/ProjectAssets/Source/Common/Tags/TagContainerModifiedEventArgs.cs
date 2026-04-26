
namespace Common.Tags
{
    public readonly struct TagContainerModifiedEventArgs
    {
        public readonly Tag Tag;
        public readonly bool WasAdded;

        public TagContainerModifiedEventArgs(in Tag tag, bool wasAdded)
        {
            Tag = tag;
            WasAdded = wasAdded;
        }
    }
}
