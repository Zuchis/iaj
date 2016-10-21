using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class AStarPathfinding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        //how many nodes do we process on each call to the search method
        public uint NodesPerSearch { get; set; }

        public uint TotalProcessedNodes { get; protected set; }
        public int MaxOpenNodes { get; protected set; }
        public float TotalProcessingTime { get; protected set; }
        public bool InProgress { get; protected set; }

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }

        public NavigationGraphNode GoalNode { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public Vector3 StartPosition { get; protected set; }
        public Vector3 GoalPosition { get; protected set; }

        //heuristic function
        public IHeuristic Heuristic { get; protected set; }

        public AStarPathfinding(NavMeshPathGraph graph, IOpenSet open, IClosedSet closed, IHeuristic heuristic)
        {
            this.NavMeshGraph = graph;
            this.Open = open;
            this.Closed = closed;
            //this.NodesPerSearch = uint.MaxValue; //by default we process all nodes in a single request
            this.NodesPerSearch = 15;
            this.InProgress = false;
            this.Heuristic = heuristic;
        }

        public void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.StartPosition = startPosition;
            this.GoalPosition = goalPosition;
            this.StartNode = this.Quantize(this.StartPosition);
            this.GoalNode = this.Quantize(this.GoalPosition);

            //if it is not possible to quantize the positions and find the corresponding nodes, then we cannot proceed
            if (this.StartNode == null || this.GoalNode == null) return;

            //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
            //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
            //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
            ((NavMeshPoly)this.StartNode).AddConnectedPoly(this.StartPosition);
            ((NavMeshPoly)this.GoalNode).AddConnectedPoly(this.GoalPosition);

            this.InProgress = true;
            this.TotalProcessedNodes = 0;
            this.TotalProcessingTime = 0.0f;
            this.MaxOpenNodes = 0;

            //another method to solve F.value ties
            float p = this.NavMeshGraph.MinEdgeCost / 2000;

            var initialNode = new NodeRecord
            {
                gValue = 0,
                //hValue = this.Heuristic.H(this.StartNode, this.GoalNode),
                hValue = (1 + p) * this.Heuristic.H(this.StartNode, this.GoalNode),
                node = this.StartNode
            };

            initialNode.fValue = AStarPathfinding.F(initialNode);

            this.Open.Initialize(); 
            this.Open.AddToOpen(initialNode);
            this.Closed.Initialize();
        }

        protected virtual void ProcessChildNode(NodeRecord bestNode, NavigationGraphEdge connectionEdge)
        {
            //this is where you process a child node 
            var childNode = GenerateChildNodeRecord(bestNode, connectionEdge);
            //TODO put the code from the previous LAB here

            NodeRecord nodeInOpen = Open.SearchInOpen(childNode);
            NodeRecord nodeInClosed = Closed.SearchInClosed(childNode);

            if (nodeInOpen == null && nodeInClosed == null)     //if child node is not in Open or Closed list, add to Open list
            {
                Open.AddToOpen(childNode);
            }
            else
            {       //the >= solves the F.value tie, easiest way to solve this
                if (nodeInOpen != null && nodeInOpen.fValue > childNode.fValue)     // if the same state is on Open list but with higher f-value, replace nodes
                {
                    Open.Replace(nodeInOpen, childNode);
                }
                else
                {
                    if (nodeInClosed != null && nodeInClosed.fValue > childNode.fValue)  // if the same state is on Closed list but with higher f-value, remove from Closed and add to Open list
                    {
                        Closed.RemoveFromClosed(nodeInClosed);
                        Open.AddToOpen(childNode);
                    }
                }
            }
        }

        //this method should return true if the Search process finished (i.e either because it found a solution or because there was no solution
        //it should return false if the search process didn't finish yet
        //when returning true, the solution parameter should be set to null if there is no solution. Otherwise it must contain the found solution
        //if the parameter returnPartialSolution is true, then the user wants to have a partial path to the best node so far even when the search has not finished searching
        public virtual bool Search(out Path solution, bool returnPartialSolution = false)
        {
            //TODO put the code from the previous LAB here
            //you will get compiler errors, because I change the method names in the IOpenSet and IClosedSet interfaces
            //sorry but I had to do it because if not, Unity profiler would consider the Search method in Open and Closed to be the same
            //and you would not be able to see the difference in performance searching the Open Set and in searching the closed set

            //so just replace this.Open.Search(...) by this.Open.SearchInOpen(...) and all other methods where you get the compilation errors
            float runningTime = Time.realtimeSinceStartup;
            int processedNode = 0;

            while (Open.CountOpen() != 0)
            {              //if EMPTY(open)?
                if (Open.CountOpen() > MaxOpenNodes)
                {
                    MaxOpenNodes = Open.CountOpen();
                }

                ++processedNode;
                ++TotalProcessedNodes;

                NodeRecord bestNode = Open.GetBestAndRemove();     //POP(open)

                if (GoalNode.Equals(bestNode.node))
                {        //if node is the goal return solution
                    this.CleanUp();
                    this.InProgress = false;
                    TotalProcessingTime += Time.realtimeSinceStartup - runningTime;
                    solution = CalculateSolution(bestNode, false);    //solution is not partial
                    return true;
                }
                else
                {
                    this.Closed.AddToClosed(bestNode);
                }

                int outConnections = bestNode.node.OutEdgeCount;
                for (int i = 0; i < outConnections; i++)
                {
                    ProcessChildNode(bestNode, bestNode.node.EdgeOut(i));
                }
                if (processedNode >= NodesPerSearch)
                {
                    TotalProcessingTime += Time.realtimeSinceStartup - runningTime;
                    if (returnPartialSolution)
                    {
                        solution = CalculateSolution(Open.PeekBest(), true);
                    }
                    else
                    {
                        solution = null;
                    }
                    return true;
                }
            }
            this.CleanUp();
            this.InProgress = false;
            TotalProcessingTime += Time.realtimeSinceStartup - runningTime;
            solution = null;
            return true;
        }

        protected NavigationGraphNode Quantize(Vector3 position)
        {
            return this.NavMeshGraph.QuantizeToNode(position, 1.0f);
        }

        protected void CleanUp()
        {
            //I need to remove the connections created in the initialization process
            if (this.StartNode != null)
            {
                ((NavMeshPoly)this.StartNode).RemoveConnectedPoly();
            }

            if (this.GoalNode != null)
            {
                ((NavMeshPoly)this.GoalNode).RemoveConnectedPoly();    
            }
        }

        protected virtual NodeRecord GenerateChildNodeRecord(NodeRecord parent, NavigationGraphEdge connectionEdge)
        {
            var childNode = connectionEdge.ToNode;
            var childNodeRecord = new NodeRecord
            {
                node = childNode,
                parent = parent,
                gValue = parent.gValue + connectionEdge.Cost,
                hValue = this.Heuristic.H(childNode, this.GoalNode)
            };

            childNodeRecord.fValue = F(childNodeRecord);

            return childNodeRecord;
        }

        protected Path CalculateSolution(NodeRecord node, bool partial)
        {
            var path = new Path
            {
                IsPartial = partial
            };
            var currentNode = node;

            path.PathPositions.Add(this.GoalPosition);

            //I need to remove the first Node and the last Node because they correspond to the dummy first and last Polygons that were created by the initialization.
            //And for instance I don't want to be forced to go to the center of the initial polygon before starting to move towards my destination.

            //skip the last node, but only if the solution is not partial (if the solution is partial, the last node does not correspond to the dummy goal polygon)
            if (!partial && currentNode.parent != null)
            {
                currentNode = currentNode.parent;
            }
            
            while (currentNode.parent != null)
            {
                path.PathNodes.Add(currentNode.node); //we need to reverse the list because this operator add elements to the end of the list
                path.PathPositions.Add(currentNode.node.LocalPosition);

                if (currentNode.parent.parent == null) break; //this skips the first node
                currentNode = currentNode.parent;
            }

            path.PathNodes.Reverse();
            path.PathPositions.Reverse();
            return path;
        }

        public static float F(NodeRecord node)
        {
            return F(node.gValue,node.hValue);
        }

        public static float F(float g, float h)
        {
            return g + h;
        }

    }
}
