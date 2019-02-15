using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    public event System.Action BallOutOfBoundsEvent;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (BallOutOfBoundsEvent != null)
                BallOutOfBoundsEvent();
        }
    }
}
