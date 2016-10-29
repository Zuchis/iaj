using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.HPStructures;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class GatewayHeuristic : IHeuristic
    {
        private ClusterGraph ClusterGraph { get; set; }

        public GatewayHeuristic(ClusterGraph clusterGraph)
        {
            this.ClusterGraph = clusterGraph;
        }

        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            //for now just returns the euclidean distance
            //return EuclideanDistance(node.LocalPosition, goalNode.LocalPosition);
            float minCost = 9999999;
            float cost;
            Cluster node_cluster = this.ClusterGraph.Quantize(node);
            Cluster goalNode_cluster = this.ClusterGraph.Quantize(goalNode);
            if (object.ReferenceEquals(null, node_cluster) || object.ReferenceEquals(null, goalNode_cluster) || object.ReferenceEquals(node_cluster, goalNode_cluster)) { 
                //Debug.Log("NODE NULL");
            //if (goalNode_cluster == null) Debug.Log("GOALNODE NULL");

            //if (node_cluster == goalNode_cluster)
            //{
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
