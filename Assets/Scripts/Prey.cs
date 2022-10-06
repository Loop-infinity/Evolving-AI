using Assets.Scripts.Pawns;
using UnityEngine;

[System.Serializable]
public class Prey : BaseNeuralNetworkPawn
{
    private Vector2 position;

    private int energy = 10;
    private int speed;
    private int splitTimeRemaining = 10;

    bool increasingSize = true;

    protected override float fieldOfView => 360;

    protected override float viewRange => 50;

    protected override int viewRayCount => 10;

    private bool isMoving = false;
    private Vector2 currentMovement = default;

    // Update is called once per frame
    void FixedUpdate()
    {
        CastViewConeRays();
        Move();
        //position = new Vector2(position.x + Random.Range(-0.02f, 0.02f), position.y + Random.Range(-0.02f, 0.02f));
        //transform.position = position;
        //Debug.Log(transform.position);
    }

    protected override void Move()
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
}
