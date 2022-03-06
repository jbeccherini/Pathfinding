using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTileMap : MonoBehaviour
{
    public GameObject Tile;

    public float minX = 0;
    public float maxX = 0;
    public float minY = 0;
    public float maxY = 0;

    public static float space;

    public int count = 0;

    public static bool doneLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        space = Tile.transform.localScale.x;
        int clusterId = 0;

        for (float x = minX; x <= maxX; x += space)
        {
            for (float y = maxY; y > minY; y -= space)
            {
                bool obstacle = false;
                Collider[] colliders = Physics.OverlapSphere(new Vector3((float)x, 0.15f, (float)y) + new Vector3(0, 0.1f, 0), 0.03f);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Room")) 
                    {
                        obstacle = true; break;
                    }
                    if (collider.gameObject.CompareTag("Cluster"))
                    {
                        string name = collider.gameObject.name;
                        clusterId = name[name.Length - 1] - '0';
                    }
                }
                if (obstacle) 
                {
                    continue;
                }

                GameObject tile = Instantiate(Tile, new Vector3((float)x, 0.15f, (float)y), Quaternion.identity) as GameObject;
                tile.transform.parent = GameObject.Find("Tiles"+clusterId.ToString()).transform;
                tile.name = "tile" + count;
                tile.GetComponent<TileController>().clusterID = clusterId;
                count++;
            }
        }

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (var tile in tiles)
        {
            tile.GetComponent<TileController>().FindConnections();
        }
        
        doneLoading = true;
    }
}
