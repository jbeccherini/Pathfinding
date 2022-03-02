using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterPathfinding : MonoBehaviour
{
    private GameObject startNode;
    private GameObject goalNode;
    private List<ClusterConnection> OpenList;
    private List<ClusterConnection> ClosedList;

    public List<ClusterConnection> PathList;

    private int maxClusterID = 8;

    private int startClusterId;
    private int goalClusterId;

    private bool finishedExecution = false;
    private bool finishedExecution2 = false;

    public static bool doneExecuting = false;

    // Start is called before the first frame update
    void Start()
    {

        if (OpenList == null)
        {
            Setup();
        }
        

        //StartCoroutine(FindShortestClusterPath());
    }

    public void Setup()
    {
        OpenList = new List<ClusterConnection>();
        ClosedList = new List<ClusterConnection>();
        PathList = new List<ClusterConnection>();
    }

    //IEnumerator FindShortestClusterPath()
    //{
    //    //while (!Pathfinding.nodesFound) { }

    //    while (!CreateTileMap.doneLoading) { }// || !Pathfinding.nodesFound) { }

    //    startNode = gameObject.GetComponent<Pathfinding>().startNode;
    //    goalNode = gameObject.GetComponent<Pathfinding>().goalNode;
    //    startClusterId = startNode.GetComponent<TileController>().clusterID;
    //    goalClusterId = goalNode.GetComponent<TileController>().clusterID;

    //    ClusterConnection connection = new ClusterConnection(startClusterId, 0, null);

    //    OpenList.Add(connection);

    //    while (!finishedExecution)
    //    {
    //        ClusterConnection currentCluster = GetLowestCost();
    //        for (int i = 0; i <= maxClusterID; i++)
    //        {
    //            int cost = GetCost(currentCluster.clusterID, i);
    //            if (cost != -1)
    //            {
    //                ClusterConnection clusterConnection = new ClusterConnection(i, currentCluster.costSoFar + cost, currentCluster);
    //                AddOrUpdateCluster(clusterConnection);
    //                if (clusterConnection.clusterID == goalClusterId)
    //                {
    //                    finishedExecution = true;
    //                }
    //            }
    //        }

    //        OpenList.Remove(currentCluster);
    //        ClosedList.Add(currentCluster);
    //    }

    //    foreach (var clusterConnection in OpenList)
    //    {
    //        if (clusterConnection.clusterID == goalClusterId)
    //        {
    //            connection = clusterConnection;
    //            break;
    //        }
    //    }
    //    PathList.Add(connection);

    //    while (connection.connection != null)
    //    {
    //        connection = connection.connection;
    //        PathList.Add(connection);
    //    }

    //    foreach (var item in PathList)
    //    {
    //        Debug.Log(item.ToString());
    //    }

    //    doneExecuting = true;
    //    yield return null;
    //}
    
    public List<int> FindShortestClusterPathIds()
    {
        startNode = gameObject.GetComponent<Pathfinding>().startNode;
        goalNode = gameObject.GetComponent<Pathfinding>().goalNode;
        startClusterId = startNode.GetComponent<TileController>().clusterID;
        goalClusterId = goalNode.GetComponent<TileController>().clusterID;

        ClusterConnection connection = new ClusterConnection(startClusterId, 0, null);
        finishedExecution2 = false;

        if(OpenList == null || OpenList.Count == 0) 
        {
            Setup();
        }

        OpenList.Add(connection);

        while (!finishedExecution2)
        {
            ClusterConnection currentCluster = GetLowestCost();
            for (int i = 0; i <= maxClusterID; i++)
            {
                int cost = GetCost(currentCluster.clusterID, i);
                if (cost != -1)
                {
                    ClusterConnection clusterConnection = new ClusterConnection(i, currentCluster.costSoFar + cost, currentCluster);
                    AddOrUpdateCluster(clusterConnection);
                    if (clusterConnection.clusterID == goalClusterId)
                    {
                        finishedExecution2 = true;
                    }
                }
            }

            OpenList.Remove(currentCluster);
            ClosedList.Add(currentCluster);
        }

        foreach (var clusterConnection in OpenList)
        {
            if (clusterConnection.clusterID == goalClusterId)
            {
                connection = clusterConnection;
                break;
            }
        }
        PathList.Add(connection);

        while (connection.connection != null)
        {
            connection = connection.connection;
            PathList.Add(connection);
        }

        List<int> clusterIds = new List<int>();

        foreach (var item in PathList)
        {
            clusterIds.Add(item.clusterID);
        }

        doneExecuting = true;
        return clusterIds;
    }

    public ClusterConnection GetLowestCost()
    {
        ClusterConnection nextCluster = null;

        foreach (var clusterConnection in OpenList)
        {
            if (nextCluster == null)
            {
                nextCluster = clusterConnection;
            }
            else
            {
                if (clusterConnection.costSoFar < nextCluster.costSoFar)
                {
                    nextCluster = clusterConnection;
                }
            }
        }

        return nextCluster;
    }

    public void AddOrUpdateCluster(ClusterConnection cluster)
    {
        foreach (var clusterConnection in ClosedList)
        {
            if (clusterConnection.clusterID == cluster.clusterID)
            {
                if (cluster.costSoFar < clusterConnection.costSoFar)
                {
                    ClosedList.Remove(clusterConnection);
                    AddOrUpdateCluster(cluster);
                }
                return;
            }
        }
        foreach (var clusterConnection in OpenList)
        {
            if (clusterConnection.clusterID == cluster.clusterID)
            {
                if (cluster.costSoFar < clusterConnection.costSoFar)
                {
                    OpenList[OpenList.IndexOf(clusterConnection)] = cluster;
                }
                return;
            }
        }

        OpenList.Add(cluster);
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
    private int GetCost(int cluster1, int cluster2)
    {
        int clusterid1 = Mathf.Min(cluster1, cluster2);
        int clusterid2 = Mathf.Max(cluster1, cluster2);

        if (clusterid1 == 0 && clusterid2 == 1) { return 8; }
        else if (clusterid1 == 0 && clusterid2 == 7) { return 14; }
        else if (clusterid1 == 1 && clusterid2 == 2) { return 5; } //only path
        else if (clusterid1 == 1 && clusterid2 == 3) { return 10; }
        else if (clusterid1 == 3 && clusterid2 == 4) { return 9; }
        else if (clusterid1 == 4 && clusterid2 == 5) { return 5; } //only path
        else if (clusterid1 == 4 && clusterid2 == 6) { return 6; }
        else if (clusterid1 == 6 && clusterid2 == 7) { return 9; }
        else if (clusterid1 == 7 && clusterid2 == 8) { return 5; } //only path
        else { return -1; }
    }
}

public class ClusterConnection
{
    public int clusterID { get; set; }
    public float costSoFar { get; set; }
    public ClusterConnection connection { get; set; }

    public ClusterConnection(int cluster, float costSoFar, ClusterConnection connection)
    {
        this.clusterID = cluster;
        this.costSoFar = costSoFar;
        this.connection = connection;
    }

    public override string ToString()
    {
        return "ClusterID: " + clusterID + ", cost so far: " + costSoFar;
    }

}


