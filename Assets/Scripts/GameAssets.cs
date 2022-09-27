using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;
    public Sprite Predator { get; set; }
    public Sprite Prey { get; set; }

    public void Awake()
    {
        instance = this;
    }

}
