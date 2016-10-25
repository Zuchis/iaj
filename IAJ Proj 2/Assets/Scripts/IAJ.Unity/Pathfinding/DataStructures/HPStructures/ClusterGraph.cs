using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures
{
    public class ClusterGraph : ScriptableObject
    {
        public List<Cluster> clusters;
        public List<Gateway> gateways;
        //public GatewayDistanceTableRow[] gatewayDistanceTable;
        public List<GatewayDistanceTableRow> gatewayDistanceTable = new List<GatewayDistanceTableRow>();

        public ClusterGraph()
        {
            this.clusters = new List<Cluster>();
            this.gateways = new List<Gateway>();
        }

        public Cluster Quantize(NavigationGraphNode node)
        {
            //TODO maybe implement with hastables
            foreach(var c in clusters){
                if(node.Position.x <= c.max.x && node.Position.x >= c.min.x && node.Position.z <= c.max.z && node.Position.z >= c.min.z)
                {
                    return c;
                }
            }
            return null;
        }

        public void SaveToAssetDatabase()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + typeof(ClusterGraph).Name.ToString() + ".asset");

            AssetDatabase.CreateAsset(this, assetPathAndName);
            EditorUtility.SetDirty(this);
            
            //save the clusters
            foreach(var cluster in this.clusters)
            {
                AssetDatabase.AddObjectToAsset(cluster, assetPathAndName);
            }

            //save the gateways
            foreach (var gateway in this.gateways)
            {
                AssetDatabase.AddObjectToAsset(gateway, assetPathAndName);
            }

            //save the gatewayTableRows and tableEntries
            foreach(var tableRow in this.gatewayDistanceTable)
            {
                AssetDatabase.AddObjectToAsset(tableRow, assetPathAndName);
                foreach(var tableEntry in tableRow.entries)
                {
                    AssetDatabase.AddObjectToAsset(tableEntry, assetPathAndName);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = this;
        }
    }
}
/*from cluster center to node???
            Cluster c = new Cluster();
            c.center.x = Mathf.Floor(node.Position.x / tilesize(clustersize??);
            TileX = Mathf.Floor(node.Position.x / tileSize);
            TileZ = Mathf.Floor(node.Position.z / tileSize);*/
