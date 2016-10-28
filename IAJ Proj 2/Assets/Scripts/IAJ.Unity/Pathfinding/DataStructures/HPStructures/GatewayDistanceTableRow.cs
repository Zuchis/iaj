using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class GatewayDistanceTableRow : ScriptableObject
    {
        public GatewayDistanceTableEntry[] entries;
        //public List<GatewayDistanceTableEntry> entries = new List<GatewayDistanceTableEntry>();
    }
}
