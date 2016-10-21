using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using UnityEngine;
using RAIN.Navigation;
using RAIN.Navigation.NavMesh;
using RAIN.Navigation.Graph;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;

public class PathfindingManager : MonoBehaviour {

	//public fields to be set in Unity Editor
	public GameObject startDebugSphere;
	public GameObject endDebugSphere;
	public Camera camera;

	//private fields for internal use only
	private Vector3 startPosition;
	private Vector3 endPosition;
	private NavMeshPathGraph navMesh;
	private ushort currentClickNumber;

    private AStarPathfinding aStarPathFinding;
    private Path currentSolution;

    private bool draw;

    private DynamicCharacter character;        //character to follow the final path

    // Use this for initialization
    void Start ()
	{
	    this.draw = false;
		this.currentClickNumber = 1;
		this.navMesh = NavigationManager.Instance.NavMeshGraphs [0];

        this.aStarPathFinding = new AStarPathfinding(this.navMesh, new SimpleUnorderedNodeList(), new Hash_Closed() /*new SimpleUnorderedNodeList()*/, new EuclideanHeuristic());//new ZeroHeuristic());

        this.character = new DynamicCharacter(this.startDebugSphere);  //initialize new character
        this.character.Drag = 0.5f;
    }
	
	// Update is called once per frame
	void Update () 
    {
		Vector3 position;
		NavigationGraphNode node;

		if (Input.GetMouseButtonDown(0)) 
		{
			//if there is a valid position
			if(this.MouseClickPosition(out position))
			{
				
				//if this is the first click we're setting the start point
				if(this.currentClickNumber == 1)
				{
					//show the start sphere, hide the end one
					//this is just a small adjustment to better see the debug sphere
					this.startDebugSphere.transform.position = position + Vector3.up;
					this.startDebugSphere.SetActive(true);
					this.endDebugSphere.SetActive(false);
					this.currentClickNumber = 2;
					this.startPosition = position;
				    this.currentSolution = null;
				    this.draw = false;
				}
				else 
				{
					//we're setting the end point
					//this is just a small adjustment to better see the debug sphere
					this.endDebugSphere.transform.position = position + Vector3.up;
					this.endDebugSphere.SetActive(true);
					this.currentClickNumber = 1;
					this.endPosition = position;
				    this.draw = true;
                    //initialize the search algorithm
                    this.aStarPathFinding.InitializePathfindingSearch(this.startPosition,this.endPosition);
				}
			}
		}

        //call the pathfinding method if the user specified a new goal
        if (this.aStarPathFinding.InProgress)
        {
            //var finished = this.aStarPathFinding.Search(out this.currentSolution);                //for full path
            var finished = this.aStarPathFinding.Search(out this.currentSolution, true);            //for partial path  lab3 question 2.c
            if (finished && this.currentSolution != null)
            {
                //here I would make a character follow the path   
               

            }
        }
}

    public void OnGUI()
    {
        if (this.currentSolution != null)
        {
            var time = this.aStarPathFinding.TotalProcessingTime*1000;
            float timePerNode;
            if (this.aStarPathFinding.TotalProcessedNodes > 0)
            {
                timePerNode = time/this.aStarPathFinding.TotalProcessedNodes;
            }
            else
            {
                timePerNode = 0;
            }
            var text = "Nodes Visited: " + this.aStarPathFinding.TotalProcessedNodes
                       + "\nMaximum Open Size: " + this.aStarPathFinding.MaxOpenNodes
                       + "\nProcessing time (ms): " + time
                       + "\nTime per Node (ms):" + timePerNode;
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10,10,200,100),text);
        }
    }

    public void OnDrawGizmos()
    {
        if (this.draw)
        {
            //draw the current Solution Path if any (for debug purposes)
            if (this.currentSolution != null)
            {
                var previousPosition = this.startPosition;
                foreach (var pathPosition in this.currentSolution.PathPositions)
                {
                    Debug.DrawLine(previousPosition, pathPosition, Color.red);
                    previousPosition = pathPosition;
                }
            }

            //draw the nodes in Open and Closed Sets
            if (this.aStarPathFinding != null)
            {
                Gizmos.color = Color.cyan;

                if (this.aStarPathFinding.Open != null)
                {
                    foreach (var nodeRecord in this.aStarPathFinding.Open.All())
                    {
                        Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }

                Gizmos.color = Color.blue;

                if (this.aStarPathFinding.Closed != null)
                {
                    foreach (var nodeRecord in this.aStarPathFinding.Closed.All())
                    {
                        Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                    }
                }
            }
        }
    }

	private bool MouseClickPosition(out Vector3 position)
	{
		RaycastHit hit;

		var ray = this.camera.ScreenPointToRay (Input.mousePosition);
		//test intersection with objects in the scene
		if (Physics.Raycast (ray, out hit)) 
		{
			//if there is a collision, we will get the collision point
			position = hit.point;
			return true;
		}

		position = Vector3.zero;
		//if not the point is not valid
		return false;
	}
}
