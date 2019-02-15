using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public event System.Action BallHitTargetEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (BallHitTargetEvent != null)
                BallHitTargetEvent();
        }
    }
}
