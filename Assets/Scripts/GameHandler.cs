using Assets.Scripts.Pawns;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public GameObject predatorPrefab;
    public GameObject preyPrefab;
    public GameObject genericPawnPrefab;
    public float timeScale = 1;

    private bool isTraning = false;
    private int predatorPopulationSize = 40;
    private int preyPopulationSize = 40;
    private int genericPawnPopulationSize = 40;
    private int generationNumber = 0;
    private int[] layers = new int[] { 11, 10, 10, 2 }; //20 inputs and 2 output
    private List<NeuralNetwork> predatorNets;
    private List<NeuralNetwork> preyNets;
    private List<NeuralNetwork> genericPawnNets;
    private bool leftMouseDown = false;
    private List<Predator> predatorList = null;
    private List<GenericPawn> genericPawnList = null;
    private bool predatorNetsInitComplete = false;
    private bool preyNetsInitComplete = false;
    private bool genericPawnNetsInitComplete = false;

    private List<Prey> preyList = null;

    public Text generationNumberText;
    void Timer()
    {
        isTraning = false;
    }


    void FixedUpdate()
    {
        if (this.timeScale > 0)
        {
            Time.timeScale = timeScale;
        }
        
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitPredatorNeuralNetworks();
                InitPreyNeuralNetworks();
                InitGenericPawnNeuralNetworks();
            }
            else
            {
                if (predatorNets != null && predatorNetsInitComplete)
                {
                    CopyMutatePredatorNeuralNetworks();
                }

                if (preyNets != null && preyNetsInitComplete)
                {
                    CopyMutatePreyNeuralNetworks();
                }

                if (genericPawnNets != null && genericPawnNetsInitComplete)
                {
                    CopyMutateGenericPawnNeuralNetworks();
                }

                var food = GameObject.FindGameObjectsWithTag("Food");

                if (food.Length > 50)
                {
                    foreach (var f in food.Skip(50))
                    {
                        Destroy(f);
                    }
                }
            }


            generationNumber++;
            generationNumberText.text = "Generation No: " + generationNumber;

            isTraning = true;
            Invoke("Timer", 60f);
            CreatePredatorBodies();
            CreatePreyBodies();
            CreateGenericPawnBodies();
        }       
    }

    private void Update()
    {
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
            //prey.transform.position = mousePosition;
        }
    }


    private void CreatePredatorBodies()
    {
        if (predatorList != null)
        {
            for (int i = 0; i < predatorList.Count; i++)
            {
                if(predatorList[i] != null)
                {
                    GameObject.Destroy(predatorList[i].gameObject);
                }
            }
        }

        predatorList = new List<Predator>();

        for (int i = 0; i < predatorPopulationSize; i++)
        {
            var predatorGameObj = ((GameObject)Instantiate(predatorPrefab, new Vector3(UnityEngine.Random.Range(-350f, 350f), UnityEngine.Random.Range(-200f, 200f), 0), predatorPrefab.transform.rotation));
            Predator predator = predatorGameObj.GetComponent<Predator>();
            predator.Init(predatorNets[i]);
            predatorList.Add(predator);
            //Change the fittest predator's color to blue
            if (i == predatorPopulationSize - 1)
            {
                predator.SetPredatorColor(Color.blue);
            }
        }

    }

    private void CreatePreyBodies()
    {
        if (preyList != null)
        {
            for (int i = 0; i < preyList.Count; i++)
            {
                if(preyList[i] != null)
                {
                    GameObject.Destroy(preyList[i].gameObject);
                }
            }
        }

        preyList = new();

        for (int i = 0; i < preyPopulationSize; i++)
        {
            Prey prey = Instantiate(preyPrefab, new Vector3(Random.Range(-350f, 350f), UnityEngine.Random.Range(-200f, 200f), 0), preyPrefab.transform.rotation).GetComponent<Prey>();
            prey.Init(preyNets[i]);
            preyList.Add(prey);
        }
    }

    private void CreateGenericPawnBodies()
    {
        if (genericPawnList != null)
        {
            for (int i = 0; i < genericPawnList.Count; i++)
            {
                if (genericPawnList[i] != null)
                {
                    GameObject.Destroy(genericPawnList[i].gameObject);
                }
            }
        }

        genericPawnList = new();

        for (int i = 0; i < genericPawnPopulationSize; i++)
        {
            GenericPawn genericPawn = Instantiate(genericPawnPrefab, new Vector3(Random.Range(-350f, 350f), UnityEngine.Random.Range(-200f, 200f), 0), genericPawnPrefab.transform.rotation).GetComponent<GenericPawn>();
            genericPawn.Init(genericPawnNets[i]);
            genericPawnList.Add(genericPawn);
        }
    }

    void InitGenericPawnNeuralNetworks()
    {
        genericPawnNets = new List<NeuralNetwork>();
        var genericPawnLayers = new int[] { 11, 10, 10, 4 };

        for (int i = 0; i < genericPawnPopulationSize; i++)
        {
            var net = new NeuralNetwork(genericPawnLayers);
            net.Mutate();
            genericPawnNets.Add(net);
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

        preyNetsInitComplete = true;
    }

    void CopyMutatePredatorNeuralNetworks()
    {
        predatorNets.Sort();
        Debug.Log("Highest Fitness Score: " + predatorNets[predatorPopulationSize - 1].GetFitness());

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

    public void RepoducePredator(GameObject predator)
    {
        var child = ((GameObject)Instantiate(predator, predator.transform.position, predator.transform.rotation)).GetComponent<Predator>();
        var parent = predator.GetComponent<Predator>();

        child.Init(new NeuralNetwork(parent.net));
        child.Energy = 100;
    }

    void CopyMutateGenericPawnNeuralNetworks()
    {
        genericPawnNets.Sort();
        Debug.Log("Highest Fitness Score: " + predatorNets[predatorPopulationSize - 1].GetFitness());

        for (int i = 0; i < genericPawnPopulationSize / 2; i++)
        {
            genericPawnNets[i] = new NeuralNetwork(genericPawnNets[i + (genericPawnPopulationSize / 2)]);
            genericPawnNets[i].Mutate();

            genericPawnNets[i + (genericPawnPopulationSize / 2)] = new NeuralNetwork(genericPawnNets[i + (genericPawnPopulationSize / 2)]);
        }

        for (int i = 0; i < genericPawnPopulationSize; i++)
        {
            genericPawnNets[i].SetFitness(0f);
        }
    }

    void CopyMutatePreyNeuralNetworks()
    {
        preyNets.Sort();

        for (int i = 0; i < preyPopulationSize / 2; i++)
        {
            preyNets[i] = new NeuralNetwork(preyNets[i + (preyPopulationSize / 2)]);
            preyNets[i].Mutate();

            preyNets[i + (preyPopulationSize / 2)] = new NeuralNetwork(preyNets[i + (preyPopulationSize / 2)]);
        }

        for (int i = 0; i < preyPopulationSize; i++)
        {
            preyNets[i].SetFitness(0f);
        }
    }
}
