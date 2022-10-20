using Assets.Scripts.Pawns;
using UnityEngine;

[System.Serializable]
public class Predator : BaseNeuralNetworkPawn
{
    private Vector2 position;
    //const float MaxMovementInterval = 0.1f;

    [field: SerializeField]
    public float Energy { get; set; } = 100;
    const float EnergyConsumptionRate = 0.1f;
    const float MaxEnergy = 100f;
    private int speed;

    protected override float FieldOfView => 80;

    protected override float ViewRange => 100;

    public int KillCount { get; set; }

    const long angVelocityMultiplier = 3;
    const long speedMultiplier = 6;

   // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
        var rayResult = CastViewConeRays("Prey(Clone)");
        if(net != null && net.Initialized)
        {
            var outputs = net?.FeedForward(rayResult);
            transform.Rotate(0, 0, outputs[0] * angVelocityMultiplier, Space.World);//controls the predator's rotation
            transform.position += -this.transform.right * outputs[1] * speedMultiplier; ; //control the movement

            if (outputs[1] != 0)
            {
                ConsumeEnergy();
            }

            //transform.Rotate(0, 0, outputs[0] * angVelocityMultiplier, Space.World);
            //rBody.AddForce(this.transform.right * outputs[1] * speedMultiplier);
            //rBody.angularVelocity = outputs[0] * angVelocityMultiplier;
            //rBody.velocity = this.transform.right * outputs[1] * speedMultiplier;
        }

    }

    private void ConsumeEnergy()
    {
        if(Energy > 0)
        {
            Energy = Energy - ((MaxEnergy/100) * EnergyConsumptionRate);
            var healthBar = gameObject.GetComponentInChildren<HealthBar>();
            healthBar.UpdateEnergy(Energy);

            if (Energy <= 0)
            {
                GameObject.Destroy(this.gameObject);
            }

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
        Energy += 40;
        IncrementPredatorKillCount();
        net.AddFitness(1);
        GameObject.Destroy(prey);

        var gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
        gameHandler.RepoducePredator(gameObject);
    }

    public void SetPredatorColor(Color color)
    {
        this.GetComponent<SpriteRenderer>().color = color;
    }

    protected override void Move(float[] neuralNetworkOutputs)
    {
        position = transform.position;
        position = new Vector2(position.x + UnityEngine.Random.Range(-0.5f, 0.5f), position.y + UnityEngine.Random.Range(-0.5f, 0.5f));
        transform.position = position;

        //if the predator moves move, consume energy.
        if (Energy > 0)
        {
            ConsumeEnergy();
        }
    }
}
