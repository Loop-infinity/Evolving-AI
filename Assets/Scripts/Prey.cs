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

    protected override float FieldOfView => 360;

    protected override float ViewRange => 50;

    protected override int ViewRayCount => 10;

    private bool isMoving = false;
    private Vector2 currentMovement = default;

    const long angVelocityMultiplier = 3;
    const long speedMultiplier = 6;

    // Update is called once per frame
    void FixedUpdate()
    {
        var rayResult = CastViewConeRays("Predator(Clone)");

        if (net != null && net.Initialized)
        {
            //yay!! the prey survived another update, give it a cookie!
            net.AddFitness(1);

            var outputs = net?.FeedForward(rayResult);
            transform.Rotate(0, 0, outputs[0] * angVelocityMultiplier, Space.World);//controls the predator's rotation
            transform.position += this.transform.right * outputs[1] * speedMultiplier; //control the movement
        }
        //position = new Vector2(position.x + Random.Range(-0.02f, 0.02f), position.y + Random.Range(-0.02f, 0.02f));
        //transform.position = position;
        //Debug.Log(transform.position);
    }

    protected override void Move(float[] neuralNetworkOutputs)
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
