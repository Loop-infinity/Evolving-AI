using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorCollisionController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Prey(Clone)")
        {
            Eat(collision.gameObject);
        }
    }

    private void Eat(GameObject prey)
    {
        GameObject.Destroy(prey);
    }
}
