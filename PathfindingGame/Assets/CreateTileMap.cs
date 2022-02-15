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

    public float space;

    // Start is called before the first frame update
    void Start()
    {
        for (float x = minX; x <= maxX; x += space)
        {
            for (float y = maxY; y > minY; y -= space)
            {
                Instantiate(Tile, new Vector3((float)x, 0.15f, (float)y), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
