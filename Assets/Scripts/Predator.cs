using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    private Vector2 position;
    //const float MaxMovementInterval = 0.1f;

    public int Energy { get; set; } = 10;
    private int speed;

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

        Debug.Log(transform.position);
    }

    private void ConsumeEnergy()
    {
        if(Energy > 0)
        {
            Energy = Energy - 1;
        }
    }

    public void Init(NeuralNetwork net, Transform prey)
    {
        this.prey = prey;
        this.net = net;
        initilized = true;
    }

}
