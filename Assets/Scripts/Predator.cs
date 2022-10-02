using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Predator : MonoBehaviour
{
    private Vector2 position;
    //const float MaxMovementInterval = 0.1f;
    
    [SerializeField]
    private bool debug = true;

    public int Energy { get; set; } = 10;
    private int speed;


    //the cone width that the entity can use for raycasting
    private int fieldOfView = 80;

    //the distance for which the entity can cast rays
    private int viewRange = 50;

    //the amount of rays cast within the cone of view
    private int viewRayCount = 10;

    //private float time = 0;


    private bool initilized = false;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private CircleCollider2D circleCollider;
    const long angVelocityMultiplier = 3;
    const long speedMultiplier = 5;

    void Start()
    {
        //transform.position = position;

        rBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

   // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
        var rayResult = CastViewConeRays();
        if(net != null && net.Initialized)
        {
            var outputs = net?.FeedForward(rayResult);
            transform.Rotate(0, 0, outputs[0] * angVelocityMultiplier, Space.World);//controls the predator's rotation
            transform.position += this.transform.right * outputs[1] * speedMultiplier; //control the movement
            //rBody.angularVelocity = outputs[0] * angVelocityMultiplier;
            //rBody.velocity = this.transform.right * outputs[1] * speed;

        }

    }

    private void Move()
    {
        position = transform.position;
        position = new Vector2(position.x + UnityEngine.Random.Range(-0.5f, 0.5f), position.y + UnityEngine.Random.Range(-0.5f, 0.5f));
        transform.position = position;

        //if they move
        if (Energy > 0)
        {
            ConsumeEnergy();
        }
    }

    private void ConsumeEnergy()
    {
        if(Energy > 0)
        {
            Energy = Energy - 1;
        }
    }

    //OLD RAYCASTING THAT WAS SORT OF WORKING
    //private RaycastHit2D CastRay(Vector3 forwardView, float angle){

    //    var dir = Quaternion.Euler(0, 0, angle) * forwardView;
    //    //var startPoint = GetPointOnCircle(circleCollider.radius, transform.position, angle);
    //    var startPoint = transform.position;
    //    RaycastHit2D hit = Physics2D.Raycast(startPoint, dir, viewRange);

    //    if (debug)
    //    {
    //        Color rayColor = Color.red;
    //        float distance = viewRange;

    //        if (hit.collider != null)
    //        {
    //            rayColor = Color.yellow;
    //            distance = hit.distance;
    //        }

    //        Debug.DrawRay(startPoint, dir * distance, rayColor, Time.deltaTime);
    //    }


    //    return hit;
    //}

    private RaycastHit2D CastRay(float radialDistance, float angle)
    {
        var x = (Mathf.Sin(Mathf.Deg2Rad * angle)) * radialDistance;
        var y = (Mathf.Cos(Mathf.Deg2Rad * angle)) * radialDistance;

        var dir = new Vector3(x, y);

        var startPoint = transform.position + (new Vector3(x, y));
        RaycastHit2D hit = Physics2D.Raycast(startPoint, dir, viewRange);

        if (debug)
        {
            Color rayColor = Color.red;
            float distance = viewRange - radialDistance;

            if (hit.collider != null)
            {
                rayColor = Color.yellow;
                distance = hit.distance - radialDistance;
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
        var radialDistance = circleCollider.radius * 6;


        for (int i = -(halfFov); i < (halfFov); i+=degreesBetweenRays)
        {
            var hit = CastRay(radialDistance, i);

            if (hit.collider != null)
            {              
                //doing this math as float gives 0 so use decimal then convert back to float
                decimal normalisedDistance = (1m / viewRange) * (decimal)hit.distance;
                output[outputIndex] = (float)normalisedDistance;
            }
            else
            {
                output[outputIndex] = 0;
            }
            outputIndex++;
        }

        return output;
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true;
    }

}
