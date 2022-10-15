using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public void UpdateEnergy(float energy)
    {
        transform.localScale = new Vector2(energy / 10, 0.5f);
    }
}
