using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanHeuristic : IHeuristic
    {
        
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
         /* float sub_x = goalNode.Position.x - node.Position.x;
            float sub_z = goalNode.Position.z - node.Position.z;
            float x_value = Mathf.Pow(sub_x, 2);
            float z_value = Mathf.Pow(sub_z, 2);
            return Mathf.Sqrt(x_value + z_value);
         */

            return Vector3.Distance(goalNode.Position, node.Position);

           // return Vector3.Magnitude(goalNode.Position - node.Position);
        }
    }
}
