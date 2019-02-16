﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticController : MonoBehaviour
{
    struct GenomeFitness
    {
        public float fitness;
        public float[] genome;
    }

    public GameControl gameControl;
    public int numberOfGenerations = 100;
    public int numPerGeneration = 30;
    public int numHidden = 15;
    public float gameCutoffTime = 120;
    public float timeScalingDuringGame = 1;

    private float[] bestGenerationGenome;
    private float bestGenerationFitness;
    int currentController;
    private NeuralNet[] neuralNetsThisGeneration;
    private List<GenomeFitness> generationFitness;

    float totalTime = 0;
    private void Start()
    {
        gameControl.GameEndedWithFitness += GameEnded;
        gameControl.maxLevelCutoffTime = gameCutoffTime;
        NewGeneration();
    }

    bool doTime = true;
    private void Update()
    {
        if (doTime)
            totalTime += Time.deltaTime;
    }

    void NextGame()
    {
        StartNewGame(neuralNetsThisGeneration[currentController].GetGenome());
    }

    void StartNewGame(float[] genome)
    {
        Time.timeScale = timeScalingDuringGame;
        NeuralNet neuralNet = new NeuralNet(genome);
        gameControl.StartGame(new NeuralController(neuralNet));

        if (NewPopulationIndividualEvent != null)
            NewPopulationIndividualEvent(currentController);
    }

    public event System.Action<float> NewIndividualFitnessEvent;
    public event System.Action<float> NewBestFitnessEvent;
    public event System.Action<int> NewGenerationEvent;
    public event System.Action<int, float> EndGenerationEvent;
    public event System.Action<int> NewPopulationIndividualEvent;
    void GameEnded(float fitness)
    {
        GenomeFitness g = new GenomeFitness();
        g.fitness = fitness;
        g.genome = neuralNetsThisGeneration[currentController].GetGenome();
        generationFitness.Add(g);

        if (NewIndividualFitnessEvent != null)
            NewIndividualFitnessEvent(fitness);

        if (fitness > bestGenerationFitness)
        {
            bestGenerationFitness = fitness;
            bestGenerationGenome = g.genome;
            if (NewBestFitnessEvent != null)
                NewBestFitnessEvent(fitness);
        }

        Time.timeScale = 1;
        if (currentController < numPerGeneration - 1)
        {
            ++currentController;
            NextGame();
        }
        else
        {
            EndGeneration();
        }

    }


    public event System.Action<float[]> FinalGenerationCompleteEvent;
    void EndGeneration()
    {
        generationFitness.Sort((s1, s2) => s2.fitness.CompareTo(s1.fitness));

        //Debug.Log("Zero index fitness = " + generationFitness[0].fitness + ", "+ (generationFitness.Count - 1 )+ " index = " + generationFitness[generationFitness.Count-1].fitness);


        if (EndGenerationEvent != null)
            EndGenerationEvent(currentGeneration, bestGenerationFitness);
        //TEST
        if (currentGeneration < numberOfGenerations)
        {
            NewGeneration();
        } else
        {
            doTime = false;
            Debug.Log("TOTAL TIME : " + totalTime + " s");
            FinalGenerationCompleteEvent?.Invoke(generationFitness[0].genome);
        }
    }

    int currentGeneration = 0;
    public float percentOfTopToKeep;
    public float percentOfBottomToDiscard;
    void NewGeneration()
    {
        //First generation
        if (generationFitness == null)
        {
            FirstGen();
        }
        else
        {
            NewGen();
        }

        bestGenerationFitness = float.MinValue;
        currentController = 0;
        NextGame();
        if (NewGenerationEvent != null)
            NewGenerationEvent(currentGeneration);
    }

    void FirstGen()
    {
        generationFitness = new List<GenomeFitness>(numPerGeneration);
        neuralNetsThisGeneration = new NeuralNet[numPerGeneration];
        FillRandom();
    }

    void FillRandom(int startIndex = 0)
    {
        for (int i = startIndex; i < numPerGeneration; i++)
        {
            neuralNetsThisGeneration[i] = new NeuralNet(gameControl.DataInputLength(), numHidden, GameControl.NUM_OUTPUTS_FOR_NN);
        }
    }

    float baseMutateChance = 0.05f;
    float genomeElementMutateChance = 0.2f;
    void NewGen()
    {
        //Debug.Log("Next gen");
        ++currentGeneration;
        if (NewGenerationEvent != null)
            NewGenerationEvent(currentGeneration);

        int topToKeep = Mathf.CeilToInt(numPerGeneration * percentOfTopToKeep);
        int bottomToDiscard = (int)(numPerGeneration * percentOfBottomToDiscard);
        //Debug.Log("Keeping top" + topToKeep);
        for (int i = 0; i < topToKeep; i++)
        {
            neuralNetsThisGeneration[i] = new NeuralNet( generationFitness[i].genome );
        }

        for (int i = topToKeep; i < bottomToDiscard; i++)
        {
            if (Random.Range(0f, 1f) < 0.6f)
            {
                neuralNetsThisGeneration[i] = CrossoverAtNodes(i);
            } else
            {
                neuralNetsThisGeneration[i] = Crossover(i);
            }
            
        }

        FillRandom(bottomToDiscard);
        Mutate();
    }

    NeuralNet Crossover(int index)
    {
        int partnerIndex = Random.Range(0, index);
        float[] parentGenome = generationFitness[index].genome;
        float[] partnerGenome = generationFitness[partnerIndex].genome;
        float[] crossedGenome = new float[partnerGenome.Length];

        if (parentGenome.Length != parentGenome.Length)
        {
            Debug.LogError("Oh shit how are the genome lengths different?!");
            Debug.Break();
        }
        crossedGenome[0] = parentGenome[0];
        crossedGenome[1] = parentGenome[1];
        crossedGenome[2] = parentGenome[2];
        for (int i = 3; i < partnerGenome.Length; i++)
        {
            float val;
            if (Random.Range(0f, 1f) < 0.5f)
            {
                val = parentGenome[i];
            }
            else
            {
                val = partnerGenome[i];
            }
            crossedGenome[i] = val;
        }

        return new NeuralNet(crossedGenome);

    }

    NeuralNet CrossoverAtNodes(int index)
    {
        int partnerIndex = Random.Range(0, index);
        float[] parentGenome = generationFitness[index].genome;
        float[] partnerGenome = generationFitness[partnerIndex].genome;
        float[] crossedGenome = new float[partnerGenome.Length];

        NeuralNet parentNet = new NeuralNet(parentGenome);

        crossedGenome[0] = parentGenome[0];
        crossedGenome[1] = parentGenome[1];
        crossedGenome[2] = parentGenome[2];

        for (int i = 0; i < parentNet.NumInputNodes + 1; i++)
        {
            bool useParent = (Random.Range(0f, 1f) < 0.5f);
            for (int j = 0; j < parentNet.FirstHiddenLayerNodes; j++)
            {
                int subIndex = 3 + i * parentNet.FirstHiddenLayerNodes + j;
                crossedGenome[subIndex] = useParent ? parentGenome[subIndex] : partnerGenome[subIndex];
            }
        }

        int offset = (parentNet.NumInputNodes + 1) * parentNet.FirstHiddenLayerNodes + 3;

        for (int i = 0; i < parentNet.FirstHiddenLayerNodes + 1; i++)
        {
            bool useParent = (Random.Range(0f, 1f) < 0.5f);
            for (int j = 0; j < parentNet.NumOutputNodes; j++)
            {
                int subIndex = offset + i * parentNet.NumOutputNodes + j;
                crossedGenome[subIndex] = useParent ? parentGenome[subIndex] : partnerGenome[subIndex];
            }
        }
        return new NeuralNet(crossedGenome);
    }

    public float mutateMax = 0.3f;
    void Mutate()
    {

        //Debug.Log("Mutate");
        for (int i = 0; i < numPerGeneration; i++)
        {
            if (Random.Range(0f,1f) < baseMutateChance)
            {
                float[] genome = neuralNetsThisGeneration[i].GetGenome();
                for (int j = 3; j < genome.Length; j++)
                {
                    if (Random.Range(0f, 1f) < genomeElementMutateChance)
                    {
                        genome[j] += Random.Range(-mutateMax, mutateMax);
                    }
                }
                neuralNetsThisGeneration[i] = new NeuralNet(genome);
            }
        }
    }
}
