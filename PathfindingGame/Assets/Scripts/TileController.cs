using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public List<Connection> connections;
    public static bool debug = true;

    private float distance = CreateTileMap.space;

    // Start is called before the first frame update
    void Start()
    {
        if (Physics.CheckSphere(this.transform.position + new Vector3(0, 0.1f, 0), 0.03f)) 
        {
            //Destroy(this.gameObject);
            
        }

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

        if (this.gameObject.name.Equals("tile77")) 
        {
            foreach (var connection in connections)
            {
                Debug.Log(connection.ToString());
            }
            debug = false;
        }

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


