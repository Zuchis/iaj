using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public class GlobalPath : Path
    {
        public List<NavigationGraphNode> PathNodes { get; protected set; }
        public List<Vector3> PathPositions { get; protected set; } 
        public bool IsPartial { get; set; }
        public float Length { get; set; }
        public List<LocalPath> LocalPaths { get; protected set; } 


        public GlobalPath()
        {
            this.PathNodes = new List<NavigationGraphNode>();
            this.PathPositions = new List<Vector3>();
            this.LocalPaths = new List<LocalPath>();
        }

        public void CalculateLocalPathsFromPathPositions(Vector3 initialPosition)
        {
            Vector3 previousPosition = initialPosition;
            for (int i = 0; i < this.PathPositions.Count; i++)
            {
                if (!previousPosition.Equals(this.PathPositions[i]))
                {
                    this.LocalPaths.Add(new LineSegmentPath(previousPosition, this.PathPositions[i]));
                    previousPosition = this.PathPositions[i];
                }
            }
        }

        public override float GetParam(Vector3 position, float previousParam)
        {
            int index = Mathf.FloorToInt(previousParam);
            
            float param = LocalPaths[index].GetParam(position, 0);
            while(param == 1)
            {
                param = LocalPaths[index++].GetParam(position, 0);
            }
            //if (param == 1 && index + 1 < LocalPaths.Count) index++;  //recursividade, quando nao der 1 tamos no segmento certo
           
            
            return LocalPaths[index].GetParam(position, 0);
        }

        public override Vector3 GetPosition(float param)
        {
            int roundParam = Mathf.FloorToInt(param);   
            float diff = param - roundParam;
            return LocalPaths[roundParam].GetPosition(diff);
        }

        public override bool PathEnd(float param)
        {
            return param >= (LocalPaths.Count - 0.05);
        }
    }
}
