using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using System.Collections;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;

internal class Hash_Closed : IClosedSet
{
    private Dictionary<NodeRecord, NodeRecord> NodeRecords { get; set; }

    public Hash_Closed()
    {
        this.NodeRecords = new Dictionary<NodeRecord, NodeRecord>();
    }

    public void AddToClosed(NodeRecord nodeRecord)
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

    public void RemoveFromClosed(NodeRecord nodeRecord)
    {
        this.NodeRecords.Remove(nodeRecord);
    }

    public NodeRecord SearchInClosed(NodeRecord nodeRecord)
    {
        if (NodeRecords.ContainsKey(nodeRecord))
        {
            return nodeRecord;
        }
            return null;
    }
}