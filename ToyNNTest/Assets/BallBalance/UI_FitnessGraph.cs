using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FitnessGraph : MonoBehaviour
{
    public RectTransform graphPrefab, graph10Prefab;
    
    public void AddFitnessValue(int generation, float valueMax, float valueTen)
    {
        RectTransform t1 = Instantiate(graphPrefab, transform);
        t1.localPosition = new Vector3(2 + (5 * generation), 2, 0);
        t1.localScale = new Vector3(1, valueMax, 1);

        RectTransform t2 = Instantiate(graph10Prefab, transform);
        t2.localPosition = new Vector3(2 + (5 * generation), 2, 0);
        t2.localScale = new Vector3(1, valueTen, 1);
    }
}
