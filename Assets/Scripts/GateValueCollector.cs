using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GateValueCollector : MonoBehaviour
{
    //public int max, min;

    private GateManager left_gate, right_gate;

    private void Awake()
    {
        right_gate = transform.GetChild(0).GetComponent<GateManager>();
        left_gate = transform.GetChild(1).GetComponent<GateManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (left_gate.select_method == 3 && right_gate.select_method == 3)
        {
            selectRandomGate().selectMethod();
        }
        else if (left_gate.select_method == right_gate.select_method && right_gate.select_method == 2)
        {
            selectRandomGate().selectMethod();
        }

        ProceduralLevelGenerator.instance.max_stickman_possible = Mathf.Max(left_gate.stickman_max, right_gate.stickman_max);
        ProceduralLevelGenerator.instance.min_stickman_possible = Mathf.Min(left_gate.stickman_min, right_gate.stickman_min);

        if (ProceduralLevelGenerator.instance.min_stickman_possible < 0)
        {
            ProceduralLevelGenerator.instance.min_stickman_possible = 1;
        }
        if (ProceduralLevelGenerator.instance.max_stickman_possible < 0)
        {
            ProceduralLevelGenerator.instance.max_stickman_possible = 1;
        }
    }

    private GateManager selectRandomGate()
    {
        return Random.Range(0, 2) == 0 ? left_gate : right_gate;
    }
}