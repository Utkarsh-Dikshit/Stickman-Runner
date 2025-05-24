using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GateManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro gate_value;

    private int increase_count;

    private int num_stickman;

    private List <int> numbers = new List<int> { 1, 3, 2, 1, 2, 1};

    [HideInInspector] public int stickman_max, stickman_min;
    [HideInInspector] public int select_method;

    private void OnEnable()
    {
        transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = true; // gate right
        transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = true; // gate left
    }

    void Awake()
    {
        selectMethod();
    }

    public void selectMethod()
    {
        stickman_max = ProceduralLevelGenerator.instance.max_stickman_possible;
        stickman_min = ProceduralLevelGenerator.instance.min_stickman_possible;

        select_method = numbers[Random.Range(0, numbers.Count)];
        numbers.Remove(select_method);

        // add
        if (select_method == 1)
        {
            if (stickman_max > 100)
            {
                increase_count = Random.Range(10, 30);
            }
            else if (stickman_max > 40 && stickman_max <= 100)
            {
                increase_count = Random.Range(20, 40);
            }
            else
            {
                increase_count = Random.Range(30, 101);
            }
            stickman_max += increase_count;
            stickman_min += increase_count;
            gate_value.text = "+";
        }

        // multiply
        else if (select_method == 2)
        {
            if (stickman_max > 80)
            {
                increase_count = 1;
            }
            else
            {
                increase_count = Random.Range(1, 3);
            }
            stickman_max *= increase_count;
            stickman_min *= increase_count;
            gate_value.text = "x";
        }

        // subtract
        else if (select_method == 3)
        {
            if (stickman_max > 100)
            {
                increase_count = Random.Range(50, 101);
            }
            else
            {
                increase_count = Random.Range(1, 101);
            }
            stickman_max -= increase_count;
            stickman_min -= increase_count;
            gate_value.text = "-";
        }

        else
        {
            Debug.Log("Gate Value Not Selected");
        }

        gate_value.text += increase_count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false; // gate right
            transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false; // gate left

            num_stickman = PlayerManager.instance.num_player_stickman;

            if (select_method == 1)
            {
                StartCoroutine(PlayerManager.instance.updateNumberOfPlayerStickman(num_stickman + increase_count));
            }
            else if (select_method == 2)
            {
                StartCoroutine(PlayerManager.instance.updateNumberOfPlayerStickman(num_stickman * increase_count));
            }
            else if (select_method == 3)
            {
                StartCoroutine(PlayerManager.instance.updateNumberOfPlayerStickman(num_stickman - increase_count));
            }
        }
    }
}