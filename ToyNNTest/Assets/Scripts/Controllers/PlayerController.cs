using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : IControllerBrain
{
    public GameControl.DataOutputs GetOutputs(GameControl.DataInputs dataInputX, GameControl.DataInputs dataInputY)
    {
        GameControl.DataOutputs outputs;
        outputs.outputX = Input.GetAxisRaw("Vertical");
        outputs.outputY = Input.GetAxisRaw("Horizontal");

        return outputs;
    }

    public override string ToString()
    {
        return "PLAYER CONTROLLER";
    }
}
