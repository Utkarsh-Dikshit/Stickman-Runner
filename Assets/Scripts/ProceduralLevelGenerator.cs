using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralLevelGenerator : MonoBehaviour
{
    private int num_obstacles;

    public static ProceduralLevelGenerator instance;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject finish_line;
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject gate;

    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private CinemachineVirtualCamera cinemachine_camera;

    public Canvas canvas;

    private List<GameObject> elements = new List<GameObject>();

    private GameObject last_used_object;

    private float road_scale;
    private Vector3 player_pos;
    private float sep_by_distance;

    [HideInInspector] public int max_stickman_possible;
    [HideInInspector] public int min_stickman_possible;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //resetGameManager();
    }

    private void Start()
    {
        
    }

    private void instantiateElements()
    {
        instantiateGb(road, Vector3.zero, road.transform.rotation);
        
        GameObject gb_player = Instantiate(player, player_pos, player.transform.rotation);
        elements.Add(gb_player);
        cinemachine_camera.Follow = gb_player.transform;
        cinemachine_camera.LookAt = gb_player.transform;

        instantiateGb(finish_line, new Vector3(0, 0.294f, road_scale * 16), finish_line.transform.rotation);

        StartCoroutine(instantiateObstacle());
    }

    private void instantiateGb(GameObject gameobject, Vector3 pos, Quaternion quaternion)
    {
        GameObject gb = Instantiate(gameobject, pos, quaternion);
        elements.Add(gb);
    }

    private void instantiateEnemy(GameObject enemy, Vector3 pos, Quaternion quaternion)
    {
        GameObject gb = Instantiate(enemy, pos, quaternion);
        gb.transform.GetChild(2).GetComponent<EnemyManager>().initializeEnemyArea(min_stickman_possible, max_stickman_possible);
        elements.Add(gb);
    }

    public IEnumerator disableAll()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        foreach (GameObject gb in elements)
        {
            Destroy(gb);
        }
        elements.Clear();
    }

    IEnumerator instantiateObstacle()
    {
        instantiateGb(gate, new Vector3(0.06f, 1.12f, player_pos.z + sep_by_distance), Quaternion.identity);
        last_used_object = gate;

        List<GameObject> available_objects = new List<GameObject>(obstacles);
        available_objects.Remove(last_used_object);

        //yield return new WaitForSeconds(0.3f);
        yield return null;

        //Debug.Log("Obstacles " + num_obstacles);

        for (int i = 0; i < num_obstacles;)
        {
            bool is_spawned = false;

            GameObject gb = available_objects[Random.Range(0, available_objects.Count)];
            last_used_object = gb;

            if (gb.name == "Gate Manager")
            {
                instantiateGb(gb, new Vector3(0.06f, 1.12f, -road_scale * 16 + (i + 2) * sep_by_distance), Quaternion.identity);
                is_spawned = true;
            }

            else if (gb.name == "Obstacle Roller")
            {
                if (max_stickman_possible < 70)
                {
                    max_stickman_possible -= 10;
                    min_stickman_possible -= 5;
                }
                else
                {
                    max_stickman_possible -= 30;
                    min_stickman_possible -= 10;
                }
                float randomSelection = Random.Range(0, 2) == 0 ? 0.6f : -0.6f;
                instantiateGb(gb, new Vector3(randomSelection, 0.5f, (-road_scale * 16) + (i + 2) * sep_by_distance), gb.transform.rotation);
                is_spawned = true;
            }

            else if (gb.name == "Spikes Area")
            {
                if (max_stickman_possible < 80)
                {
                    max_stickman_possible -= 10;
                    min_stickman_possible -= 5;
                }
                else
                {
                    max_stickman_possible -= 20;
                    min_stickman_possible -= 5;
                }

                float randomSelection = Random.Range(0, 2) == 0 ? -1f : 1f;
                instantiateGb(gb, new Vector3(randomSelection, 0, -road_scale * 16 + (i + 2) * sep_by_distance), gb.transform.rotation);
                is_spawned = true;
            }
            
            else if (gb.name == "Enemy Area" && max_stickman_possible > 20)
            {
                instantiateEnemy(gb, new Vector3(0, 0, -road_scale * 16 + (i + 2) * sep_by_distance), gb.transform.rotation);
                is_spawned = true;
            }

            if (is_spawned)
            {
                i++;
                available_objects = new List<GameObject>(obstacles);
                available_objects.Remove(last_used_object);
            }

            yield return new WaitForSeconds(0.3f);
            //yield return null;
            //Debug.Log(max_stickman_possible + " " + min_stickman_possible);
        }
    }

    public void resetGameManager()
    {
        max_stickman_possible = 1;
        min_stickman_possible = 1;

        road_scale = Random.Range(1f, 3f);
        road.transform.localScale = new Vector3(1, 1, road_scale);

        player_pos = new Vector3(0, 0, -road_scale * 16);

        sep_by_distance = 6f;
        //sep_by_distance = (road_scale * 32) / (num_obstacles + 3);

        int max_range = (int)((road_scale * 32) / sep_by_distance);
        num_obstacles = max_range - 3;

        sep_by_distance = (road_scale * 32) / max_range;

        instantiateElements();
        canvas.gameObject.SetActive(false);
    }
}