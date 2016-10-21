using JetBrains.Annotations;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public interface IOpenSet : IClosedSet
    {
        void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace);
        NodeRecord GetBestAndRemove();
        NodeRecord PeekBest();
        int Count();
    }
}
