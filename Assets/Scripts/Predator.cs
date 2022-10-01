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
    private int viewRange = 20;

    //the amount of rays cast within the cone of view
    private int viewRayCount = 10;

    //private float time = 0;


    private bool initilized = false;
    private Transform prey;

    private NeuralNetwork net;
    private Rigidbody2D rBody;

    void Start()
    {
        //transform.position = position;

        rBody = GetComponent<Rigidbody2D>();
    }

   // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        var rayResult = CastViewConeRays();
        net?.FeedForward(rayResult);
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

    private RaycastHit2D CastRay(Vector3 forwardView, float angle){
        
        var dir = Quaternion.Euler(0, 0, angle) * forwardView;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRange);
        
        if (debug)
        {
            Color rayColor = Color.red;
            float distance = viewRange;

            if (hit.collider != null)
            {
                rayColor = Color.yellow;
                distance = hit.distance;
            }

            Debug.DrawRay(transform.position, dir * distance, rayColor, Time.deltaTime);
        }
        

        return hit;
    }

    private float[] CastViewConeRays()
    {
        var output = new float[fieldOfView];
        var degreesBetweenRays = fieldOfView / viewRayCount;
        var halfFov = fieldOfView / 2;
        var outputIndex = 0;


        for (int i = -(halfFov); i < (halfFov); i+=degreesBetweenRays)
        {
            var hit = CastRay(Vector3.right, i);

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

    public void Init(NeuralNetwork net, Transform prey)
    {
        this.prey = prey;
        this.net = net;
        initilized = true;
    }

}
