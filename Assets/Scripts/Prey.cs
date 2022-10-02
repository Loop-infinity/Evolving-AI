using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prey : MonoBehaviour
{
    private Vector2 position;
    private int instanceId;

    private int energy = 10;
    private int speed;
    private int splitTimeRemaining = 10;

    bool increasingSize = true;

    [SerializeField]
    bool debug = true;

    //the cone width that the entity can use for raycasting
    private int fieldOfView = 360;

    //the distance for which the entity can cast rays
    private float viewRange = 50;

    //the amount of rays cast within the cone of view
    private int viewRayCount = 10;

    private NeuralNetwork net;
    private bool initilized = false;

    private Rigidbody2D rBody;
    CircleCollider2D circleCollider;

    private bool isMoving = false;
    private Vector2 currentMovement = default;

    //private void Awake()
    //{
    //    position = new Vector2(2, 0);
    //}

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = position;

        rBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CastViewConeRays();
        Move();
        //position = new Vector2(position.x + Random.Range(-0.02f, 0.02f), position.y + Random.Range(-0.02f, 0.02f));
        //transform.position = position;
        //Debug.Log(transform.position);
    }

    private void Move()
    {
        var newMovement = !isMoving;

        if (isMoving)
        {
            var rand = Random.Range(0f, 10f);
            if (rand > 9.9f)
            {
                newMovement = true;
            }
        }

        if (newMovement)
        {
            var movements = new[] {
                transform.up,
                -transform.up,
                -transform.right,
                transform.right
            };

            currentMovement = movements[Random.Range((int)0, (int)4)];
            isMoving = true;
        }

        transform.Translate(currentMovement * 20 * Time.deltaTime);
        Debug.DrawRay(transform.position, currentMovement * 50, Color.magenta, Time.deltaTime);

    }

    public void Init(NeuralNetwork neuralNetwork)
    {
        net = neuralNetwork;
        initilized = true;
        instanceId = GetInstanceID();
    }

    private RaycastHit2D CastRay(float radialDistance, float angle)
    {
        var x = (Mathf.Sin(Mathf.Deg2Rad * angle)) * radialDistance;
        var y = (Mathf.Cos(Mathf.Deg2Rad * angle)) * radialDistance;

        var dir = new Vector3(x, y);

        var startPoint = transform.position + (new Vector3(x, y));
        RaycastHit2D hit = Physics2D.Raycast(startPoint, dir, viewRange);

        if (debug)
        {
            Color rayColor = Color.green;
            float distance = viewRange;

            if (hit.collider != null)
            {
                rayColor = Color.blue;
                distance = hit.distance;
            }

            Debug.DrawRay(startPoint, (dir/radialDistance) * distance, rayColor, Time.deltaTime);
        }


        return hit;
    }

    private Vector2 GetPointOnCircle(float radius, Vector2 origin, float angle)
    {
        var x = radius * Mathf.Cos(angle);
        var y = radius * Mathf.Sin(angle);

        return origin + new Vector2(x, y);
    }

    private float[] CastViewConeRays()
    {
        var output = new float[viewRayCount];
        var degreesBetweenRays = fieldOfView / viewRayCount;
        var halfFov = fieldOfView / 2;
        var outputIndex = 0;
        var radialDistnace = circleCollider.radius * 6;


        for (int i = 0; i < fieldOfView; i += degreesBetweenRays)
        {
            var hit = CastRay(radialDistnace, i);
           
            if (hit.collider != null)
            {
                //doing this math as float gives 0 so use decimal then convert back to float
                output[outputIndex] = (1 / viewRange) * hit.distance;
            }
            else
            {
                output[outputIndex] = 0;
            }
            outputIndex++;
        }

        return output;
    }
}
