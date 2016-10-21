using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public interface IClosedSet
    {
        void Initialize();
        void Add(NodeRecord nodeRecord);
        void Remove(NodeRecord nodeRecord);
        //should return null if the node is not found
        NodeRecord Search(NodeRecord nodeRecord);
        ICollection<NodeRecord> All();
    }
}
