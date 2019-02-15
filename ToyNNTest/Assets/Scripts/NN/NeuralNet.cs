using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNet 
{
    int numInputNodes, numOutputNodes;
    int firstHiddenLayerNodes;

    float[,] inputToHiddenWeights;
    float[,] hiddenToOutputWeights;

    public int NumInputNodes { get => numInputNodes; private set => numInputNodes = value; }
    public int NumOutputNodes { get => numOutputNodes; private set => numOutputNodes = value; }
    public int FirstHiddenLayerNodes { get => firstHiddenLayerNodes; private set => firstHiddenLayerNodes = value; }

    public NeuralNet(int numInputNodes, int firstHiddenLayerNodes, int numOutputNodes)
    {
        this.NumInputNodes = numInputNodes;
        this.NumOutputNodes = numOutputNodes;
        this.FirstHiddenLayerNodes = firstHiddenLayerNodes;
        if (this.NumOutputNodes != GameControl.NUM_OUTPUTS)
        {
            Debug.LogError("NUM OUTPUT NODES != 2");
            Debug.Break();
        }
        SetupWeights();
    }


    public float[] ProcessInputs(float[] inputs)
    {
        if (NumOutputNodes != GameControl.NUM_OUTPUTS)
        {
            Debug.LogWarning("NUM OUTPUTS NOT EQUAL TO TWO HERE!!!");
            Debug.Break();
        }
        //float[] outputs = new float[numOutputNodes];
        float[] outputs = new float[GameControl.NUM_OUTPUTS];

        float[] hiddenActivations = new float[FirstHiddenLayerNodes];
        for (int i = 0; i < FirstHiddenLayerNodes; i++)
        {
            float rawVal;
            float summedActivation = 0;
            for (int j = 0; j < NumInputNodes; j++)
            {
                rawVal = inputToHiddenWeights[j, i] * inputs[j];
                if (rawVal < 0) rawVal *= 0.05f;

                summedActivation += rawVal;
            }
            rawVal = inputToHiddenWeights[NumInputNodes, i];
            if (rawVal < 0) rawVal *= 0.05f;

            summedActivation += rawVal;

            hiddenActivations[i] = summedActivation;
        }

        //for (int i = 0; i < numOutputNodes; i++)
        for (int i = 0; i < GameControl.NUM_OUTPUTS; i++)
        {
            float summedActivation = 0;
            for (int j = 0; j < FirstHiddenLayerNodes; j++)
            {
                float a = hiddenToOutputWeights[j, i];
                float b = hiddenActivations[j];
                float rawVal = a * b;
                
                summedActivation += rawVal;
            }
            summedActivation += hiddenToOutputWeights[FirstHiddenLayerNodes, i];

            outputs[i] = 1 / (1 + Mathf.Exp(-summedActivation)); 
        }

        return outputs;
    }


    void SetupWeights()
    {
        inputToHiddenWeights = new float[NumInputNodes+1, FirstHiddenLayerNodes];
        for (int i = 0; i < NumInputNodes+1; i++)
        {
            for (int j = 0; j < FirstHiddenLayerNodes; j++)
            {
                inputToHiddenWeights[i, j] = Random.Range(-0.3f, 0.3f);
            }
        }

        hiddenToOutputWeights = new float[FirstHiddenLayerNodes+1, NumOutputNodes];
        for (int i = 0; i < FirstHiddenLayerNodes+1; i++)
        {
            for (int j = 0; j < NumOutputNodes; j++)
            {
                hiddenToOutputWeights[i, j] = Random.Range(-0.3f, 0.3f);
            }
        }
    }


    public NeuralNet(float[] genome)
    {
        this.NumInputNodes = (int)genome[0];
        this.FirstHiddenLayerNodes = (int)genome[1];
        this.NumOutputNodes = (int)genome[2];
        if (this.NumOutputNodes != GameControl.NUM_OUTPUTS)
        {
            Debug.Log("[" + genome[0] + "," + genome[1] + "," + genome[2] + "]");
            Debug.LogError("NUM OUTPUT NODES != 2");
            Debug.Break();
        }
        SetupFromGenome(genome);
    }

    void SetupFromGenome(float[] genome)
    {

        inputToHiddenWeights = new float[NumInputNodes+1, FirstHiddenLayerNodes];
        for (int i = 0; i < NumInputNodes+1; i++)
        {
            for (int j = 0; j < FirstHiddenLayerNodes; j++)
            {
                inputToHiddenWeights[i, j] = genome[3 + i * FirstHiddenLayerNodes + j];
            }
        }

        int offset = (NumInputNodes+1) * FirstHiddenLayerNodes + 3;
        hiddenToOutputWeights = new float[FirstHiddenLayerNodes+1, NumOutputNodes];
        for (int i = 0; i < FirstHiddenLayerNodes+1; i++)
        {
            for (int j = 0; j < NumOutputNodes; j++)
            {
                hiddenToOutputWeights[i, j] = genome[offset + i * NumOutputNodes + j];
            }
        }
    }


    public float[] GetGenome()
    {
        int genomeSize = (NumInputNodes+1) * FirstHiddenLayerNodes + (FirstHiddenLayerNodes+1) * NumOutputNodes + 3;

        float[] genome = new float[genomeSize];
        genome[0] = NumInputNodes;
        genome[1] = FirstHiddenLayerNodes;
        genome[2] = NumOutputNodes;

        for (int i = 0; i < NumInputNodes+1; i++)
        {
            for (int j = 0; j < FirstHiddenLayerNodes; j++)
            {
                int index = 3 + i * FirstHiddenLayerNodes + j;
                genome[index] = inputToHiddenWeights[i, j];
            }
        }

        int offset = (NumInputNodes+1) * FirstHiddenLayerNodes + 3;
        for (int i = 0; i < FirstHiddenLayerNodes+1; i++)
        {
            for (int j = 0; j < NumOutputNodes; j++)
            {
                int index = offset + i * NumOutputNodes + j;
                float weight = hiddenToOutputWeights[i, j];
                genome[index] = weight;
            }
        }

        return genome;
    }

}
