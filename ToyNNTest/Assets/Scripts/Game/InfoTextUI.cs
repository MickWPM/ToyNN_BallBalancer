using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextUI : MonoBehaviour
{
    public UI_FitnessGraph fitnessGraph;
    public TMPro.TextMeshProUGUI realtimeInfoTextMesh, bestFitnessTextMesh, newGeneticIndividaulInPopulationTextMesh, newGenerationTextMesh;
    public GameControl gameControl;
    public  GeneticController geneticControl;
    public GameObject trainingCompleteGO;
    public TMPro.TextMeshProUGUI runningResultsTextMesh;

    int generation, individual;
    float topAllTimeFitness = float.MinValue;

    public int numTopFitnessesToRecord = 20;
    private void Awake()
    {
        geneticControl.NewBestFitnessEvent += (float fitness) => 
                                {
                                    bestFitnessTextMesh.text = "Best Fitness this generation: " + fitness;
                                    //Debug.Log("Top fitness: " + topAllTimeFitness);
                                    //Debug.Log("fitness: " + fitness);
                                    if (fitness > topAllTimeFitness)
                                    {
                                        //Debug.Log("(fitness > topAllTimeFitness)");
                                        topAllTimeFitness = fitness;
                                        topFitnessString = "Top fitness: " + fitness + " in generation " + generation;
                                    }
                                };
        geneticControl.NewGenerationEvent += (int generation) =>
        {
            newGenerationTextMesh.text = "Current generation: " + generation;
            this.generation = generation;
            fitnesses.Clear();
        };


        geneticControl.EndGenerationEvent += (int generation, float bestFitness) =>
        {
            fitnessGraph.AddFitnessValue(generation, bestFitness, fitnesses[numTopFitnessesToRecord-1]);
        };

        geneticControl.NewPopulationIndividualEvent += (int individual) => { newGeneticIndividaulInPopulationTextMesh.text = "Current Individual: " + individual; this.individual = individual; };
        geneticControl.NewIndividualFitnessEvent += NewIndividualFitness;

        geneticControl.FinalGenerationCompleteEvent += (float[] finalGenome) => { };
        trainingCompleteGO.SetActive(false);
        fitnesses = new List<float>(11);
        runningResultsTextMesh.text = topFitnessString;
    }

    void FinalGenDone(float[] bestGenome)
    {
        gameControl.StartGame(new NeuralController(new NeuralNet(bestGenome)));

        string s = "Best genome: [" + bestGenome[0];

        for (int i = 1; i < bestGenome.Length; i++)
        {
            s += ", " + bestGenome[i];
        }
        s += "]";
        Debug.Log(s);
        trainingCompleteGO.SetActive(true);
    }

    string topFitnessString = "No top fitness yet";
    List<float> fitnesses;
    void NewIndividualFitness(float fitness)
    {
        if (fitnesses.Count > numTopFitnessesToRecord && fitnesses[numTopFitnessesToRecord-1] > fitness)
            return;

        fitnesses.Add(fitness);
        fitnesses.Sort((s1, s2) => s2.CompareTo(s1));
        string fitString = "Top fitness this population:\n";
        int iterateMax = Mathf.Min(numTopFitnessesToRecord, fitnesses.Count);
        for (int i = 0; i < iterateMax; i++)
        {
            fitString += "\n"+(i + 1) + ". " + fitnesses[i];
        }



        runningResultsTextMesh.text = topFitnessString + '\n' + fitString;

    }

    void Update()
    {
        realtimeInfoTextMesh.text = "CONTROLLER: " + gameControl.controllerBrain + "\n" + gameControl.GetDataString() + "\n\nCurrent score: " + gameControl.GetFitnessScore().ToString();

    }
}
