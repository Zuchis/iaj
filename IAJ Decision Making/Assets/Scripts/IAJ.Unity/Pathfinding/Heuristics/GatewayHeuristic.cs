using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class GatewayHeuristic : IHeuristic
    {
        private ClusterGraph ClusterGraph { get; set; }
        private SerializedDictionaryNodeCluster nodeClusterDictionary;

        public GatewayHeuristic(ClusterGraph clusterGraph, List<NavigationGraphNode> nodes)
        {
            this.ClusterGraph = clusterGraph;
            this.nodeClusterDictionary = new SerializedDictionaryNodeCluster();
            this.InitializeDictionary(nodes);
        }

        private void InitializeDictionary(List<NavigationGraphNode> nodes)
        {
            bool flag;
            float smallestDistance;
            float distance;
            Cluster closestCluster = null;
            // Creating the dictionary of clusters and nodes

            foreach (var node in nodes)
            {
                flag = false;
                smallestDistance = 9999999.0f;
                foreach (var c in ClusterGraph.clusters)
                {
                    distance = Vector3.Distance(node.LocalPosition, c.center);
                    if (distance <= smallestDistance)
                    {
                        smallestDistance = distance;
                        closestCluster = c;
                    }
                    if (MathHelper.PointInsideBoundingBox(node.Position, c.min, c.max)) // found the cluster for the node
                    {
                        if (!this.nodeClusterDictionary.ContainsKey(node))
                        {
                            this.nodeClusterDictionary.Add(node, c);
                            flag = true;
                        }
                    }
                }
                if (flag == false) // if no cluster was found, get the closest one
                {
                    if (!this.nodeClusterDictionary.ContainsKey(node))
                    {
                        this.nodeClusterDictionary.Add(node, closestCluster);
                    }
                }
            }
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            float minCost = 9999999;
            float cost;
            Cluster node_cluster = this.nodeClusterDictionary[node];
            Cluster goalNode_cluster = this.nodeClusterDictionary[goalNode];

            //ignore null's returned from Quantize method and return euclidean distance
            if (object.ReferenceEquals(null, node_cluster) || object.ReferenceEquals(null, goalNode_cluster) || object.ReferenceEquals(node_cluster, goalNode_cluster)) { 
                return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            }
            else
            {
                foreach(Gateway g1 in node_cluster.gateways)
                {
                    foreach (Gateway g2 in goalNode_cluster.gateways)
                    {
                        cost = EuclideanDistance(node.LocalPosition, g1.center) + this.ClusterGraph.gatewayDistanceTable[g1.id].entries[g2.id].shortestDistance + EuclideanDistance(g2.center, goalNode.LocalPosition);
                        if (cost < minCost) minCost = cost;
                    }
                }       
            }
            return minCost;
        }

        public float EuclideanDistance(Vector3 startPosition, Vector3 endPosition)
        {
            return (endPosition - startPosition).magnitude;
        }
    }
}
