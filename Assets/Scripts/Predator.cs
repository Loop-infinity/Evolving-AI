using Assets.Scripts.Pawns;
using UnityEngine;

[System.Serializable]
public class Predator : BaseNeuralNetworkPawn
{
    private Vector2 position;
    //const float MaxMovementInterval = 0.1f;

    public int Energy { get; set; } = 10;
    private int speed;

    protected override float fieldOfView => 80;

    protected override float viewRange => 100;

    public int KillCount { get; set; }

    const long angVelocityMultiplier = 3;
    const long speedMultiplier = 12;

   // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
        var rayResult = CastViewConeRays("Prey(Clone)");
        if(net != null && net.Initialized)
        {
            var outputs = net?.FeedForward(rayResult);
            transform.Rotate(0, 0, outputs[0] * angVelocityMultiplier, Space.World);//controls the predator's rotation
            transform.position += this.transform.right * outputs[1] * speedMultiplier; //control the movement
            //rBody.angularVelocity = outputs[0] * angVelocityMultiplier;
            //rBody.velocity = this.transform.right * outputs[1] * speed;
        }

    }

    private void ConsumeEnergy()
    {
        if(Energy > 0)
        {
            Energy = Energy - 1;
        }
    }

    private void IncrementPredatorKillCount()
    {
        KillCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (net != null && net.Initialized && collision.gameObject.name == "Prey(Clone)")
        {
            Eat(collision.gameObject);
        }
    }

    private void Eat(GameObject prey)
    {
        IncrementPredatorKillCount();
        net.AddFitness(1);
        GameObject.Destroy(prey);
    }

    public void SetPredatorColor(Color color)
    {
        this.GetComponent<SpriteRenderer>().color = color;
    }

    protected override void Move()
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
}
