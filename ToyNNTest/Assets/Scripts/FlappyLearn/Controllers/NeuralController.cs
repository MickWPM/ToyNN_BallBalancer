using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{
    public class NeuralController : IBirdController
    {
        public static readonly float FlapThreshold = 0.8f;
        FlappyNeuralNet neuralNet;

        public NeuralController(int hiddenNodes)
        {
            neuralNet = new FlappyNeuralNet(hiddenNodes);
        }
        public NeuralController(float[] genome)
        {
            neuralNet = new FlappyNeuralNet(genome);
        }

        public bool DoFlapFromState(FlappyGameState gameState)
        {
            return (neuralNet.ProcessInputs(gameState.StateToArray)[0] > FlapThreshold);
        }

        public float[] GetGenome()
        {
            return neuralNet.GetGenome();
        }

        public string GetGenomeString()
        {
            float[] genome = neuralNet.GetGenome();
            string s = "";

            s += "[" + genome[0];
            for (int i = 1; i < genome.Length; i++)
            {
                s += ",\n" + genome[i];
            }
            s += "]";

            return s;

        }
    }
}