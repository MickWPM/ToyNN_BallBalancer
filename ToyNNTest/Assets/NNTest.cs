using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNTest : MonoBehaviour
{
    public int input = 2, output = 2, hidden = 3;
    public NeuralNet neuralNet;

    private void Awake()
    {
        neuralNet = new NeuralNet(input, hidden, output);
        savedGenome = neuralNet.GetGenome();
    }
    public float[] inputs = { .3f, 1.5f };
    public float[] outputs;
    public float[] savedGenome;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            LogGenome();
            outputs = neuralNet.ProcessInputs(inputs);

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            neuralNet = new NeuralNet(input, hidden, output);
            LogGenome();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Saving genome");
            savedGenome = neuralNet.GetGenome();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Loading genome");
            neuralNet = new NeuralNet(savedGenome);
        }
    }

    void LogGenome()
    {
        float[] genome = neuralNet.GetGenome();
        string s = "Genome: [" + genome[0];
        for (int i = 1; i < genome.Length; i++)
        {
            s += "," + genome[i];
        }
        s += "]";
        Debug.Log(s);
    }
}
