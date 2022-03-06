using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MonoBehaviour
{
    public GameObject GameManager;
    private GameObject Tile;
    public GameObject body;

    [SerializeField] float MaxSpeed = 1.0f;
    [SerializeField] float arriveRadius = 0.02f;
    [SerializeField] float timeToTarget = 0.02f;
    [SerializeField] float delay = 0.025f;
    [SerializeField] float rotationSpeed = 1.0f;

    private float speed;

    private Animator animator;

    private bool startTileFound;

    // Start is called before the first frame update
    void Start()
    {
        animator = body.GetComponent<Animator>();

        StartCoroutine(FindTile());
        StartCoroutine(FollowPath());

        
    }


    // Update is called once per frame
    void Update()
    {
        if (speed > 0.05)
        {
            animator.SetBool("IsRunning", true);
        }
        else 
        {
            animator.SetBool("IsRunning", false);
        }
    }

    IEnumerator FindTile() 
    {
        while (!CreateTileMap.doneLoading) { }

        Collider[] tileColliders = Physics.OverlapSphere(transform.position, .1f);
        foreach (var tile in tileColliders)
        {
            if (tile.gameObject.CompareTag("Tile"))
            {
                if (Tile == null)
                {
                    Tile = tile.gameObject;
                }
                else if ((Tile.transform.position - transform.position).magnitude > (tile.transform.position - transform.position).magnitude)
                {
                    Tile = tile.gameObject;
                }
            }
        }

        Debug.Log("Closest Tile is " + Tile.name);

        if (GameManager.GetComponent<Pathfinding>().startNode == null) 
        {
            GameManager.GetComponent<Pathfinding>().SetStart(Tile.name);
        }

        transform.position = new Vector3(Tile.transform.position.x,transform.position.y,Tile.transform.position.z);

        startTileFound = true;

        yield return null;  
    }

    IEnumerator FollowPath() 
    {
        while (!Pathfinding.pathFound) { yield return null; }

        var path = GameManager.GetComponent<Pathfinding>().GetPath();

        path = PathSmoothing(path);

        int nodeCounter = 0;

        foreach (var node in path)
        {
            Vector3 movement;
            Vector3 target = node.node.transform.position;
            while((target - transform.position).magnitude > arriveRadius) 
            {
                movement = KinematicSeek(node.node.transform);

                transform.Translate(movement);
                yield return null;
            }
            nodeCounter++;
        }

        speed = 0;

        StartCoroutine(WaitForNewGoal());

        yield return null;
    }

    IEnumerator WaitForNewGoal()
    {
        startTileFound = false;

        StartCoroutine(FindTile());

        while (!startTileFound) { }

        bool newGoalSet = false;
        Pathfinding.pathFound = false;

        while (!newGoalSet)
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
                            StartCoroutine(GameManager.GetComponent<Pathfinding>().SetUp(hit.collider.gameObject.name));
                            newGoalSet = true;
                            break;
                        }
                    }
                }
            }

            yield return null;
        }

        StartCoroutine(FindTile());
        StartCoroutine(FollowPath());

        yield return null;
    }

    public Vector3 KinematicArrive(Transform target)
    {
        Vector3 direction = (target.position - transform.position);

        Quaternion goalRotation = Quaternion.LookRotation(direction);
        body.transform.rotation = Quaternion.LerpUnclamped(body.transform.rotation, goalRotation, rotationSpeed);

        float distance = direction.magnitude;
        speed = MaxSpeed;

        if (distance < arriveRadius)
        {
            //speed = 0;
        }
        else
        {
            speed = Mathf.Min(MaxSpeed, distance / (timeToTarget));
        }

        Vector3 movement = direction * speed * timeToTarget * Time.deltaTime;

        return movement;
    }

    public Vector3 KinematicSeek(Transform target)
    {
        speed = MaxSpeed;

        Vector3 direction = target.position - this.transform.position;

        Vector3 normalizedvelocityDirection = direction / (direction.magnitude);

        Vector3 movement = speed * normalizedvelocityDirection * Time.deltaTime;

        body.transform.rotation = Quaternion.LerpUnclamped(body.transform.rotation, Quaternion.LookRotation(movement), rotationSpeed);

        return movement;
    }

    public List<NodeConnection> PathSmoothing(List<NodeConnection> pathList) 
    {
        List<NodeConnection> smoothedList = new List<NodeConnection>();

        smoothedList.Add(pathList[0]);

        for (int i = 2; i < pathList.Count; i++)
        {
            Vector3 currentNode = smoothedList[smoothedList.Count - 1].node.transform.position;
            Vector3 goalNode = pathList[i].node.transform.position;

            RaycastHit[] hits = Physics.RaycastAll(currentNode, (goalNode - currentNode), (goalNode - currentNode).magnitude);
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.CompareTag("Room"))
                    {
                        smoothedList.Add(pathList[i - 1]);
                    }
                }
            }
        }

        smoothedList.Add(pathList[pathList.Count - 2]);
        smoothedList.Add(pathList[pathList.Count - 1]);

        return smoothedList;
    }


}
