using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{
    public class FlappyNeuralNet
    {
        int numInputNodes = 2, numOutputNodes = 1;
        int firstHiddenLayerNodes;

        float[,] inputToHiddenWeights;
        float[,] hiddenToOutputWeights;

        public int NumInputNodes { get => numInputNodes; private set => numInputNodes = value; }
        public int NumOutputNodes { get => numOutputNodes; private set => numOutputNodes = value; }
        public int FirstHiddenLayerNodes { get => firstHiddenLayerNodes; private set => firstHiddenLayerNodes = value; }

        public FlappyNeuralNet(int firstHiddenLayerNodes)
        {
            this.FirstHiddenLayerNodes = firstHiddenLayerNodes;

            SetupWeights();
        }


        public float[] ProcessInputs(float[] inputs)
        {
            float[] outputs = new float[numOutputNodes];

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


            for (int i = 0; i < numOutputNodes; i++)
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

                //Swap to Tanh
                //outputs[i] = Activate(summedActivation);
                outputs[i] = ActivateTanh(summedActivation);
            }

            return outputs;
        }

        float Activate(float val)
        {
            return (1 / (1 + Mathf.Exp(-val)));
        }

        float ActivateTanh(float val)
        {
            return (float)System.Math.Tanh(val);
        }


        void SetupWeights()
        {
            inputToHiddenWeights = new float[NumInputNodes + 1, FirstHiddenLayerNodes];
            for (int i = 0; i < NumInputNodes + 1; i++)
            {
                for (int j = 0; j < FirstHiddenLayerNodes; j++)
                {
                    inputToHiddenWeights[i, j] = Random.Range(-0.3f, 0.3f);
                }
            }

            hiddenToOutputWeights = new float[FirstHiddenLayerNodes + 1, NumOutputNodes];
            for (int i = 0; i < FirstHiddenLayerNodes + 1; i++)
            {
                for (int j = 0; j < NumOutputNodes; j++)
                {
                    hiddenToOutputWeights[i, j] = Random.Range(-0.3f, 0.3f);
                }
            }
        }


        #region GeneticAlorithm

        //public NeuralNet(float[] genome)
        //{
        //    this.NumInputNodes = (int)genome[0];
        //    this.FirstHiddenLayerNodes = (int)genome[1];
        //    this.NumOutputNodes = (int)genome[2];
        //    if (this.NumOutputNodes != GameControl.NUM_OUTPUTS_FOR_NN)
        //    {
        //        Debug.Log("[" + genome[0] + "," + genome[1] + "," + genome[2] + "]");
        //        Debug.LogError("NUM OUTPUT NODES != 2");
        //        Debug.Break();
        //    }
        //    SetupFromGenome(genome);
        //}

        //void SetupFromGenome(float[] genome)
        //{

        //    inputToHiddenWeights = new float[NumInputNodes + 1, FirstHiddenLayerNodes];
        //    for (int i = 0; i < NumInputNodes + 1; i++)
        //    {
        //        for (int j = 0; j < FirstHiddenLayerNodes; j++)
        //        {
        //            inputToHiddenWeights[i, j] = genome[3 + i * FirstHiddenLayerNodes + j];
        //        }
        //    }

        //    int offset = (NumInputNodes + 1) * FirstHiddenLayerNodes + 3;
        //    hiddenToOutputWeights = new float[FirstHiddenLayerNodes + 1, NumOutputNodes];
        //    for (int i = 0; i < FirstHiddenLayerNodes + 1; i++)
        //    {
        //        for (int j = 0; j < NumOutputNodes; j++)
        //        {
        //            hiddenToOutputWeights[i, j] = genome[offset + i * NumOutputNodes + j];
        //        }
        //    }
        //}

        public float[] GetGenome()
        {
            int genomeSize = (NumInputNodes + 1) * FirstHiddenLayerNodes + (FirstHiddenLayerNodes + 1) * NumOutputNodes + 3;

            float[] genome = new float[genomeSize];
            genome[0] = NumInputNodes;
            genome[1] = FirstHiddenLayerNodes;
            genome[2] = NumOutputNodes;

            for (int i = 0; i < NumInputNodes + 1; i++)
            {
                for (int j = 0; j < FirstHiddenLayerNodes; j++)
                {
                    int index = 3 + i * FirstHiddenLayerNodes + j;
                    genome[index] = inputToHiddenWeights[i, j];
                }
            }

            int offset = (NumInputNodes + 1) * FirstHiddenLayerNodes + 3;
            for (int i = 0; i < FirstHiddenLayerNodes + 1; i++)
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

        #endregion
    }
}