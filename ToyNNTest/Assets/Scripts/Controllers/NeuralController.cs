using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralController : IControllerBrain
{
    NeuralNet neuralNet;
    public NeuralController(int numInputNodes, int firstHiddenLayerNodes, int numOutputNodes)
    {
        neuralNet = new NeuralNet(numInputNodes, firstHiddenLayerNodes, numOutputNodes);
        if (numOutputNodes != GameControl.NUM_OUTPUTS)
        {
            Debug.LogError("NUM OUTPUT NODES != 2");
            Debug.Break();
        }
    }

    public NeuralController(NeuralNet neuralNet)
    {
        this.neuralNet = neuralNet;
    }

    public void RandomiseNetowrk(int numInputNodes, int firstHiddenLayerNodes, int numOutputNodes)
    {
        this.neuralNet = new NeuralNet(numInputNodes, firstHiddenLayerNodes, numOutputNodes);
        if (numOutputNodes != GameControl.NUM_OUTPUTS)
        {
            Debug.LogError("NUM OUTPUT NODES != 2");
            Debug.Break();
        }
    }

    public GameControl.DataOutputs GetOutputs(GameControl.DataInputs dataInputX, GameControl.DataInputs dataInputY)
    {
        float[] outputX = neuralNet.ProcessInputs(dataInputY.Vector());
        float[] outputY = neuralNet.ProcessInputs(dataInputX.Vector());
        float[] outputs = new float[] { outputX[0], outputY[0] };
        
        ////Normal output activation
        //GameControl.DataOutputs output = new GameControl.DataOutputs
        //{
        //    outputX = outputs[0] * 2 - 1,
        //    outputY = outputs[1] * 2 - 1
        //};

        //Tanh output activation
        GameControl.DataOutputs output = new GameControl.DataOutputs
        {
            outputX = outputs[0],
            outputY = outputs[1]
        };
        return output;
    }

    public override string ToString()
    {
        float[] genome = neuralNet.GetGenome();
        string genomeString = "[" + genome[0];
        for (int i = 1; i < genome.Length; i++)
        {
            genomeString += ", " + genome[i];
        }
        genomeString += "]";
        return "NEURAL CONTROLLER : " + genomeString;
    }
}
