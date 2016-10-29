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
                //if (!previousPosition.Equals(this.PathPositions[i]))
                if(Vector3.Distance(this.PathPositions[i], previousPosition) > 0.3f)
                {
                    this.LocalPaths.Add(new LineSegmentPath(previousPosition, this.PathPositions[i]));
                    previousPosition = this.PathPositions[i];
                }
            }
        }



        public override float GetParam(Vector3 position, float previousParam)
        {
            int index = Mathf.FloorToInt(previousParam);
            //Debug.Log("INDEX = " + index);
            if (index == 0) index = 1;
            float param = LocalPaths[index].GetParam(position, 0);
            while(param >= 0.90f)
            {
                if(index + 1 >= LocalPaths.Count)    
                {
                    return index + param;
                }
               
                param = LocalPaths[index++].GetParam(position, 0);
            }
            //Debug.Log("RESULT = " + (index + param));
            return index + param;
        }

        public override Vector3 GetPosition(float param)
        {
            int roundParam = Mathf.FloorToInt(param);   
            float diff = param - roundParam;
            while(roundParam >= LocalPaths.Count)
            {
                roundParam--;
            }
            return LocalPaths[roundParam].GetPosition(diff);
        }

        public override bool PathEnd(float param)
        {
            return LocalPaths.Count - param <= 0.5f;
        }
    }
}
