using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject predatorPrefab;
    public GameObject prey;

    private bool isTraning = false;
    private int predatorPopulationSize = 50;
    private int preyPopulationSize = 100;
    private int generationNumber = 0;
    private int[] layers = new int[] { 20, 10, 10, 2 }; //20 inputs and 2 output
    private List<NeuralNetwork> predatorNets;
    private List<NeuralNetwork> preyNets;
    private bool leftMouseDown = false;
    private List<Predator> predatorList = null;
    private bool predatorNetsInitComplete = false;


    void Timer()
    {
        isTraning = false;
    }


    void Update()
    {
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitPredatorNeuralNetworks();
            }
            else
            {
                if (predatorNets != null && predatorNetsInitComplete)
                {
                    predatorNets.Sort();

                    for (int i = 0; i < predatorPopulationSize / 2; i++)
                    {
                        predatorNets[i] = new NeuralNetwork(predatorNets[i + (predatorPopulationSize / 2)]);
                        predatorNets[i].Mutate();

                        predatorNets[i + (predatorPopulationSize / 2)] = new NeuralNetwork(predatorNets[i + (predatorPopulationSize / 2)]);
                    }

                    for (int i = 0; i < predatorPopulationSize; i++)
                    {
                        predatorNets[i].SetFitness(0f);
                    }
                }
            }


            generationNumber++;

            isTraning = true;
            Invoke("Timer", 15f);
            CreatePredatorBodies();
        }


        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

        if (leftMouseDown == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            prey.transform.position = mousePosition;
        }
    }


    private void CreatePredatorBodies()
    {
        if (predatorList != null)
        {
            for (int i = 0; i < predatorList.Count; i++)
            {
                GameObject.Destroy(predatorList[i].gameObject);
            }

        }

        predatorList = new List<Predator>();

        for (int i = 0; i < predatorPopulationSize; i++)
        {
            Predator predator = ((GameObject)Instantiate(predatorPrefab, new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-50f, 50f), 0), predatorPrefab.transform.rotation)).GetComponent<Predator>();
            predator.Init(predatorNets[i], prey.transform);
            predatorList.Add(predator);
        }

    }

    void InitPredatorNeuralNetworks()
    {
        
        predatorNets = new List<NeuralNetwork>();

        for (int i = 0; i < predatorPopulationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            predatorNets.Add(net);
        }
        predatorNetsInitComplete = true;
    }

    void InitPreyNeuralNetworks()
    {
        preyNets = new List<NeuralNetwork>();


        for (int i = 0; i < preyPopulationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            preyNets.Add(net);
        }
    }
}
