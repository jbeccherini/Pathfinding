using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("Tile"))
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        break;
                    }
                }
            }
        }
    }
}
