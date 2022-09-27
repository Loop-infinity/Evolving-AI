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

    private void Awake()
    {
        position = new Vector2(2, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        //position = new Vector2(position.x + Random.Range(-0.02f, 0.02f), position.y + Random.Range(-0.02f, 0.02f));
        //transform.position = position;
        //Debug.Log(transform.position);
    }
}
