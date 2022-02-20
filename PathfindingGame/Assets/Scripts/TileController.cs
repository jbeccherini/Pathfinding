using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public List<Connection> connections;
    public static bool debug = true;

    private float distance = CreateTileMap.space;

    public int clusterID;

    [SerializeField] public GameObject center;

    // Start is called before the first frame update
    void Start()
    {
        if (connections == null) 
        {
            InstantiateConnectionList();
        }
    }

    // Update is called once per frame
    private void InstantiateConnectionList()
    {
        connections = new List<Connection>();
    }

    public void FindConnections() 
    {
        if (connections == null)
        {
            InstantiateConnectionList();
        }

        Collider[] orthogonalColliders = Physics.OverlapSphere(transform.position, .1f);
        Collider[] diagonalColliders = Physics.OverlapSphere(transform.position, .2f);
        foreach (var hitCollider in diagonalColliders)
        {
            if (hitCollider.gameObject.CompareTag("Tile") && hitCollider.gameObject != this.gameObject) 
            {
                Connection connection = new Connection(hitCollider.gameObject, distance*2);

                connections.Add(connection);
            }
        }foreach (var hitCollider in orthogonalColliders)
        {
            if (hitCollider.gameObject.CompareTag("Tile") && hitCollider.gameObject != this.gameObject)
            {
                foreach (var connection in connections)
                {
                    if (connection.node == hitCollider.gameObject) 
                    {
                        connection.cost = distance;
                    }
                }
            }
        }
    }

    public void DrawRay(GameObject connection)
    {
        var startPos = center.transform.position;
        var endPos = connection.GetComponent<TileController>().center.transform.position;
        var direction = (endPos - startPos).normalized;

        Debug.DrawRay(startPos, direction, Color.white, (endPos - startPos).magnitude);
    }
}

public class Connection
{
    public GameObject node { get; set; }
    public float cost { get; set; }

    public Connection(GameObject node, float cost)
    {
        this.node = node;
        this.cost = cost;
    }

    override
    public string ToString() 
    {
        return node.name +" "+ cost;
    }

}


