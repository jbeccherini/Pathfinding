using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private GameObject startNode;
    private GameObject goalNode;
    private List<NodeConnection> OpenList;
    private List<NodeConnection> ClosedList;

    public Material StartMaterial;
    public Material GoalMaterial;
    public Material ClosedMaterial;
    public Material OpenMaterial;


    // Start is called before the first frame update
    void Start()
    {
        startNode = GameObject.Find("tile421");
        goalNode = GameObject.Find("tile257");
    }

    // Update is called once per frame
    void Update()
    {
        if (CreateTileMap.doneLoading)
        {
            CreateTileMap.doneLoading = false;

            List<GameObject> gameObjects = FindShortestPath(startNode, goalNode);
        }
    }


    public List<GameObject> FindShortestPath(GameObject startNode, GameObject goalNode)
    {
        startNode.GetComponent<MeshRenderer>().material = StartMaterial;
        goalNode.GetComponent<MeshRenderer>().material = GoalMaterial;


        OpenList = new List<NodeConnection>();
        ClosedList = new List<NodeConnection>();
        bool finishedExecution = false;
        int count = 0;

        NodeConnection connection = new NodeConnection(startNode, GetHeuristic(startNode, goalNode), 0, null);

        OpenList.Add(connection);

        while (true)
        {
            NodeConnection currentNode = GetLowestEstimatedCost();

            foreach (var tileConnection in currentNode.node.GetComponent<TileController>().connections)
            {
                GameObject node = tileConnection.node;
                NodeConnection nodeConnection = new NodeConnection(node, GetHeuristic(node, goalNode), connection.costSoFar + tileConnection.cost, null);
                AddOrUpdateNode(nodeConnection);
                if (nodeConnection.node == goalNode)
                {
                    finishedExecution = true;
                }
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if (currentNode.node != goalNode && currentNode.node != startNode)
            {
                currentNode.node.gameObject.GetComponent<MeshRenderer>().material = ClosedMaterial;
            }

            if (finishedExecution)
            {
                break;
            }
        }

        return null;
    }

    public float GetHeuristic(GameObject startNode, GameObject goalNode, bool UseManhattan = true)
    {
        if (UseManhattan)
        {
            float xDelta = Mathf.Abs(goalNode.transform.position.x - startNode.transform.position.x);
            float yDelta = Mathf.Abs(goalNode.transform.position.z - startNode.transform.position.z);
            return xDelta + yDelta;
        }


        return 0;
    }

    public NodeConnection GetLowestEstimatedCost()
    {
        NodeConnection nextNode = null;

        foreach (var nodeConnection in OpenList)
        {
            if (nextNode == null)
            {
                nextNode = nodeConnection;
            }
            else
            {
                if (nodeConnection.estimatedTotalCost < nextNode.estimatedTotalCost)
                {
                    nextNode = nodeConnection;
                }
            }
        }

        return nextNode;
    }

    public void AddOrUpdateNode(NodeConnection node) 
    {
        foreach (var nodeConnection in ClosedList)
        {
            if (nodeConnection.node == node.node)
            {
                //IMPLEMENT REOPENING AND UPDATING CLOSED NODES
                return;
            }
        }
        foreach (var nodeConnection in OpenList)
        {
            if (nodeConnection.node == node.node) 
            {
                if (node.costSoFar < nodeConnection.costSoFar) 
                {
                    OpenList[OpenList.IndexOf(nodeConnection)] = node;
                }
                return;
            }
        }

        OpenList.Add(node);
        if (node.node != goalNode && node.node != startNode) 
        {
            node.node.gameObject.GetComponent<MeshRenderer>().material = OpenMaterial;
        }
        
        
    }

}

public class NodeConnection
{
    public GameObject node { get; set; }
    public float heuristic { get; set; }
    public float costSoFar { get; set; }
    public ConnectionPair connection { get; set; }
    public float estimatedTotalCost { get; set; }

    public NodeConnection(GameObject node, float heuristic, float costSoFar, ConnectionPair connection)
    {
        this.node = node;
        this.heuristic = heuristic;
        this.costSoFar = costSoFar;
        this.connection = connection;
        this.estimatedTotalCost = costSoFar + heuristic;
    }

    public override string ToString()
    {
        return node.name + " " + heuristic + " " + costSoFar + " " + connection.ToString() + " " + estimatedTotalCost;
    }

}

public class ConnectionPair
{
    public GameObject node1 { get; set; }
    public GameObject node2 { get; set; }

    public ConnectionPair(GameObject node1, GameObject node2)
    {
        this.node1 = node1;
        this.node2 = node2;
    }


    public override string ToString()
    {
        return node1.name + " " + node2.name;
    }

}
