using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    private Vector2 position;

    private int energy = 10;
    private int speed;
    private int splitTimeRemaining = 10;

    bool increasingSize = true;

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

        transform.Translate(currentMovement * 50 * Time.deltaTime);
        Debug.DrawRay(transform.position, currentMovement * 50, Color.green, Time.deltaTime);

    }

    public void Init(NeuralNetwork neuralNetwork)
    {
        net = neuralNetwork;
        initilized = true;
    }
}
