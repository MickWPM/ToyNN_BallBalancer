using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{
    public class HumanController : IBirdController
    {
        public bool DoFlapFromState(FlappyGameState gameState)
        {
            return Input.GetKey(KeyCode.Space);
        }
    }
}
