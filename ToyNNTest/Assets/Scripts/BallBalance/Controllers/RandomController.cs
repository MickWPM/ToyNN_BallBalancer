using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallBalance
{
    public class RandomController : IControllerBrain
    {
        public GameControl.DataOutputs GetOutputs(GameControl.DataInputs dataInputX, GameControl.DataInputs dataInputY)
        {
            GameControl.DataOutputs outputs;
            outputs.outputX = Random.Range(-1f, 1f);
            outputs.outputY = Random.Range(-1f, 1f);

            return outputs;
        }

        public override string ToString()
        {
            return "RANDOM CONTROLLER";
        }
    }
}