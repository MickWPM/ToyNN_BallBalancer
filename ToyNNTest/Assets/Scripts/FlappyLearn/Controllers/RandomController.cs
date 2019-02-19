using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyLearn
{
    public class RandomController : IBirdController
    {
        public bool DoFlapFromState(FlappyGameState gameState)
        {
            if (gameState.heightAboveMidPoint > 5)
                return false;
            return Random.Range(0f, 1f) < 0.05f;
        }
    }
}
