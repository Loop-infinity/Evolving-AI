using UnityEngine;

namespace Assets.Scripts.Pawns
{
    public class GenericPawn : BaseNeuralNetworkPawn
    {
        public GameObject foodPrefab;
        public float Energy { get; protected set; }
        public float MaxEnergy { get; protected set; }

        private float currentSpeedMultiplier;
        private float currentAngleVelocityMultiplier;
        const float baseEnergyConsumptionRate = 0.005f;

        private HealthBar healthBar;
        private SpriteRenderer renderer;

        protected override void Start()
        {
            healthBar = GetComponentInChildren<HealthBar>();
            renderer = GetComponent<SpriteRenderer>();
            base.Start();
        }

        void FixedUpdate()
        {
            net.AddFitness(0.1f);
            RunNeuralNetworkIO();    
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Eat(collision.gameObject);
        }

        private void BeforeDestroy()
        {
            for (int i = 0; i < 5; i++)
            {
                Instantiate(foodPrefab, new Vector3(transform.position.x + UnityEngine.Random.Range(-10f, 10f), transform.position.y + UnityEngine.Random.Range(-10f, 10f), 0), foodPrefab.transform.rotation);
            }
        }

        /// <summary>
        /// Initializes the pawn
        /// </summary>
        /// <param name="neuralNetwork">The neural network for the pawn to use</param>
        /// <param name="energy">The amount of energy the pawn spawns with</param>
        /// <param name="maxEnergy">The total amount of energy that the pawn can have</param>
        /// <param name="fieldOfView">The pawn's field of view</param>
        /// <param name="viewRange">How far the pawn can see</param>
        public void Init(NeuralNetwork neuralNetwork, float energy = 100f, float maxEnergy = 100f, float fieldOfView = 80f, float viewRange = 100f)
        {
            FieldOfView = fieldOfView;
            ViewRange = viewRange;
            Energy = energy;
            MaxEnergy = maxEnergy;
            if (renderer != null)
            {
                renderer.color = Color.red;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.cyan;
            }
            

            base.Init(neuralNetwork);
        }

        protected override void Move(float[] neuralNetworkOutputs)
        {
            
            transform.Rotate(0, 0, neuralNetworkOutputs[0] * currentAngleVelocityMultiplier, Space.World);//controls the pawn's rotation
            transform.position += -this.transform.right * neuralNetworkOutputs[1] * currentSpeedMultiplier; //control the movement

            if (neuralNetworkOutputs[1] != 0)
            {
                ConsumeEnergy();
            }
        }

        protected void RunNeuralNetworkIO()
        {
            var rayResult = CastViewConeRays();

            if (net != null && net.Initialized)
            {
                var outputs = net.FeedForward(rayResult);
                currentSpeedMultiplier = outputs[3] * 10;
                currentAngleVelocityMultiplier = outputs[2] * 6;
                Move(outputs);
            }
        }

        protected void ConsumeEnergy()
        {
            if (Energy > 0)
            {
                var angleVelocityConsumption = currentAngleVelocityMultiplier * 2.5f;
                var speedVelocityConsumption = currentSpeedMultiplier * 5f;
                angleVelocityConsumption = Mathf.Max(angleVelocityConsumption, -angleVelocityConsumption);
                speedVelocityConsumption = Mathf.Max(speedVelocityConsumption, -speedVelocityConsumption);

                var consumption = (angleVelocityConsumption + speedVelocityConsumption) * baseEnergyConsumptionRate;
                Energy -= consumption;

                healthBar.UpdateEnergy(Energy);

                if (Energy <= 0)
                {
                    BeforeDestroy();
                    Destroy(gameObject);
                }
            }
        }

        private void Eat(GameObject gameObject)
        {
            bool canDestroy = true;
            net.AddFitness(1);
            float energy = 0f;
            if (gameObject.tag == "Food")
            {
                energy = gameObject.GetComponent<Food>().Energy;
            }

            if (gameObject.name.StartsWith("Predator"))
            {
                energy = gameObject.GetComponent<Predator>().Energy;
            }

            if (gameObject.name.StartsWith("Prey"))
            {
                energy = 40;
            }

            if (gameObject.name.StartsWith("GenericPawn"))
            {
                energy = gameObject.GetComponent<GenericPawn>().Energy;
                if (energy > Energy)
                {
                    canDestroy = false;
                }

            }

            if (canDestroy)
            {
                Energy += energy;
                GameObject.Destroy(gameObject);
            }
        }
    }
}
