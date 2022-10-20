using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Pawns
{
    [System.Serializable]
    public abstract class BaseNeuralNetworkPawn : MonoBehaviour
    {
        [SerializeField]
        bool debug = true;

        //the cone width that the entity can use for raycasting
        protected virtual float FieldOfView { get; set; }

        //the distance for which the entity can cast rays
        protected virtual float ViewRange { get; set; }

        /// <summary>
        /// The amount of rays cast within the cone of view
        /// </summary>
        protected virtual int ViewRayCount { get; set; } = 10;

        public NeuralNetwork net;
        protected bool initialized = false;

        protected Rigidbody2D rBody;

        protected CircleCollider2D circleCollider;

        protected virtual void Start()
        {
            //transform.position = position;

            rBody = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
        }

        protected abstract void Move(float[] neuralNetworkOutputs);

        public virtual void Init(NeuralNetwork neuralNetwork)
        {
            net = neuralNetwork;
            initialized = true;
        }

        protected virtual RaycastHit2D CastRay(float radialDistance, float angle)
        {
            var dir = Quaternion.Euler(0, 0, angle) * transform.right;

            var startPoint = transform.position + (dir * radialDistance);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, dir, ViewRange);

            if (debug)
            {
                Color rayColor = Color.red;
                float distance = ViewRange - radialDistance;

                if (hit.collider != null)
                {
                    rayColor = Color.yellow;
                    distance = hit.distance - radialDistance;
                }

                Debug.DrawRay(startPoint, (dir) * distance, rayColor, Time.deltaTime);
            }


            return hit;
        }

        /// <summary>
        /// Fires Raycasting according to configured fieldOfView, viewRayCount and degreesBetweenRays values
        /// </summary>
        /// <param name="targetGameObjectName">When used, ray hits will only count if the game object hit has the provided name</param>
        /// <returns></returns>
        protected virtual float[] CastViewConeRays(string targetGameObjectName = null)
        {
            var output = new float[ViewRayCount + 1];
            var degreesBetweenRays = FieldOfView / ViewRayCount;
            var halfFov = FieldOfView / 2f;
            var outputIndex = 0;
            var radialDistance = circleCollider.radius * 5.5f;
            float nothingInSight = 1;

            for (float i = -(halfFov); i < (halfFov); i += degreesBetweenRays)
            {
                var hit = CastRay(radialDistance, i);

                if (hit.collider != null && (targetGameObjectName == null || hit.collider.gameObject.name == targetGameObjectName))
                {
                    output[outputIndex] = (1f / ViewRange) * hit.distance;
                    nothingInSight = 0;
                }
                else
                {
                    output[outputIndex] = 0;
                }
                outputIndex++;
            }

            output[ViewRayCount] = nothingInSight;

            return output;
        }
    }
}
