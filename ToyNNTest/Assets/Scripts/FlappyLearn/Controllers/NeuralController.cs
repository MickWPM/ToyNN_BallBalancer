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

        public bool DoFlapFromState(FlappyGameState gameState)
        {
            return (neuralNet.ProcessInputs(gameState.StateToArray)[0] > FlapThreshold);
        }

        public float[] NetGenome()
        {
            return neuralNet.GetGenome();
        }
    }
}