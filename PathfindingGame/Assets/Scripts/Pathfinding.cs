using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GameObject startNode;
    public GameObject goalNode;
    private List<NodeConnection> OpenList;
    private List<NodeConnection> ClosedList;
    public List<NodeConnection> PathList;

    public Material StartMaterial;
    public Material GoalMaterial;
    public Material ClosedMaterial;
    public Material OpenMaterial;
    public Material PathMaterial;
    public Material DefaultMaterial;

    private bool finishedExecution;

    public static bool nodesFound = false;

    public int start;
    public int goal;

    public float timeDelay = 0.1f;

    public bool useManhatten = false;

    private List<int> clusterPathIds = new List<int>();

    public static bool pathFound = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetUp());
    }

    public IEnumerator SetUp(String tileName = "") 
    {
        clusterPathIds = new List<int>();

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in tiles) 
        {
            SetMaterial(tile, DefaultMaterial);
        }

        List<int> smallClusterIds = new List<int>();
        smallClusterIds.Add(2);
        smallClusterIds.Add(5);
        smallClusterIds.Add(8);

        smallClusterIds.Remove(startNode.GetComponent<TileController>().clusterID);

        int goalIndex;

        if (tileName == "")
        {
            do
            {
                goalIndex = UnityEngine.Random.Range(0, 434);
                goalNode = GameObject.Find("tile" + goalIndex.ToString());
            } while (!smallClusterIds.Contains(goalNode.gameObject.GetComponent<TileController>().clusterID));
        }
        else 
        {
            goalNode = GameObject.Find(tileName);
        }

        nodesFound = true;

        SetMaterial(startNode, StartMaterial);
        SetMaterial(goalNode, GoalMaterial);

        OpenList = new List<NodeConnection>();
        ClosedList = new List<NodeConnection>();
        PathList = new List<NodeConnection>();

        StartCoroutine(FindShortestPath(timeDelay));
        yield return null;
    }

    IEnumerator FindShortestPath(float delay)
    {
        while (!CreateTileMap.doneLoading) 
        { }

        while (startNode == null) { }

        NodeConnection connection = new NodeConnection(startNode, GetHeuristic(startNode, goalNode), 0, null);

        OpenList.Add(connection);

        finishedExecution = false;

        while (!finishedExecution)
        {
            NodeConnection currentNode = GetLowestEstimatedCost();

            foreach (var tileConnection in currentNode.node.GetComponent<TileController>().connections)
            {
                GameObject node = tileConnection.node;
                NodeConnection nodeConnection = new NodeConnection(node, GetHeuristic(node, goalNode, useManhatten), currentNode.costSoFar + tileConnection.cost, currentNode);
                AddOrUpdateNode(nodeConnection);
                if (nodeConnection.node == goalNode)
                {
                    finishedExecution = true;
                    break;
                }
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if (currentNode.node != goalNode && currentNode.node != startNode)
            {
                SetMaterial(currentNode.node, ClosedMaterial);
            }

            yield return new WaitForSeconds(delay);
        }

        foreach (var nodeConnection in OpenList)
        {
            if (nodeConnection.node == goalNode)
            {
                connection = nodeConnection;
                break;
            }
        }

        while (connection.connection != null) 
        {
            PathList.Add(connection);
            //connection.node.GetComponent<TileController>().DrawRay(connection.connection.node);
            connection = connection.connection;
            if (connection.node == startNode) 
            {
                break;
            }
            SetMaterial(connection.node, PathMaterial);
            yield return new WaitForSeconds(delay);
        }

        PathList.Reverse();

        pathFound = true;

        startNode = null;

        yield return null;
    }

    

    public float GetHeuristic(GameObject startNode, GameObject goalNode, bool UseManhattan = true)
    {
        if (UseManhattan)
        {
            float xDelta = Mathf.Abs(goalNode.transform.position.x - startNode.transform.position.x);
            float yDelta = Mathf.Abs(goalNode.transform.position.z - startNode.transform.position.z);
            return xDelta + yDelta;
        }
        else 
        {
            float xDelta = Mathf.Abs(goalNode.transform.position.x - startNode.transform.position.x);
            float yDelta = Mathf.Abs(goalNode.transform.position.z - startNode.transform.position.z);


            if (clusterPathIds.Count == 0) 
            {
                clusterPathIds = this.GetComponent<ClusterPathfinding>().FindShortestClusterPathIds();
            }

            if (clusterPathIds.Contains(startNode.GetComponent<TileController>().clusterID))
            {
                return xDelta + yDelta;
            }
            else 
            {
            
            }

            return xDelta + yDelta + 1000000;
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
                if (node.costSoFar < nodeConnection.costSoFar) 
                {
                    ClosedList.Remove(nodeConnection);
                    AddOrUpdateNode(node);
                }

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
            SetMaterial(node.node.gameObject, OpenMaterial);
        }
    }

    private void SetMaterial(GameObject tile, Material material) 
    {
        tile.GetComponent<MeshRenderer>().material = material;
        return;
    }

    public List<NodeConnection> GetPath() 
    {
        return PathList;
    }

    public void SetStart(String tileName) 
    {
        startNode = GameObject.Find(tileName);
        return;
    }

}

public class NodeConnection
{
    public GameObject node { get; set; }
    public float heuristic { get; set; }
    public float costSoFar { get; set; }
    public NodeConnection connection { get; set; }
    public float estimatedTotalCost { get; set; }

    public NodeConnection(GameObject node, float heuristic, float costSoFar, NodeConnection connection)
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
