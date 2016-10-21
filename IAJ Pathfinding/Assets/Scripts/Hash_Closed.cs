using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using System.Collections;

internal class Hash_Closed : IClosedSet
{
    private Dictionary<NodeRecord, NodeRecord> NodeRecords { get; set; }

    public Hash_Closed()
    {
        this.NodeRecords = new Dictionary<NodeRecord, NodeRecord>();
    }

    public void Add(NodeRecord nodeRecord)
    {
        this.NodeRecords.Add(nodeRecord, nodeRecord);
    }

    public ICollection<NodeRecord> All()
    {
        return this.NodeRecords.Values;
    }

    public void Initialize()
    {
        this.NodeRecords.Clear();
    }

    public void Remove(NodeRecord nodeRecord)
    {
        this.NodeRecords.Remove(nodeRecord);
    }

    public NodeRecord Search(NodeRecord nodeRecord)
    {
        if (NodeRecords.ContainsKey(nodeRecord))
        {
            return nodeRecord;
        }
            return null;
    }
}