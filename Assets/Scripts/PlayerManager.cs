using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField] private TextMeshPro player_stickman_counter;
    [SerializeField] private float player_speed_vertical, player_speed_horizontal;
    [Range(0f, 1f)][SerializeField] private float distance_factor, radius;

    [HideInInspector] public bool attack, gamestate;
    [HideInInspector] public int num_player_stickman;
    [HideInInspector] public Transform parent_stickmans;

    private Transform enemy;
    private bool move_by_touch;
    private Vector3 mouse_start_pos, player_start_pos;
    private Camera main_camera;

    private EnemyManager enemy_manager;
    private StickmanSpawner stickman_spawner;

    private bool is_activated = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        main_camera = Camera.main;
        gamestate = true;
        attack = false;

        parent_stickmans = transform.GetChild(1);
        stickman_spawner = ProceduralLevelGenerator.instance.gameObject.GetComponent<StickmanSpawner>();
        stickman_spawner.getBlueStickman(parent_stickmans.position, parent_stickmans);
    }

    // Update is called once per frame
    void Update()
    {
        num_player_stickman = parent_stickmans.childCount;
        player_stickman_counter.text = num_player_stickman.ToString();

        if (attack == true)
        {

            Vector3 direc_to_enemy = new Vector3(enemy.position.x, transform.position.y, enemy.position.z) - transform.position;

            // Letting the player_stickman to look towards the centre enemy_stickman
            for (int i = 0; i < num_player_stickman; i++)
            {
                Transform current_stickman = parent_stickmans.GetChild(i);
                current_stickman.rotation = Quaternion.Slerp(current_stickman.rotation, Quaternion.LookRotation(direc_to_enemy, Vector3.up), Time.deltaTime * 3f);
            }

            // Move Towards the enemy stickman (centered)
            if (enemy.childCount > 0)
            {
                for (int i = 0; i < num_player_stickman; i++)
                {
                    Transform current_stickman = parent_stickmans.GetChild(i);
                    Transform middle_enemy_stickman = enemy.GetChild(0);

                    Vector3 distance = middle_enemy_stickman.position - current_stickman.position;

                    if (distance.magnitude < 0.5f)
                    {
                        current_stickman.position = Vector3.Lerp(current_stickman.position, new Vector3(middle_enemy_stickman.position.x, current_stickman.position.y, middle_enemy_stickman.position.z), Time.deltaTime * 2.5f);
                    }
                    else if (distance.magnitude < 1.3f)
                    {
                        current_stickman.position = Vector3.Lerp(current_stickman.position, new Vector3(middle_enemy_stickman.position.x, current_stickman.position.y, middle_enemy_stickman.position.z), Time.deltaTime * 1.5f);
                    }
                    else if (distance.magnitude < 2.0f)
                    {
                        current_stickman.position = Vector3.Lerp(current_stickman.position, new Vector3(middle_enemy_stickman.position.x, current_stickman.position.y, middle_enemy_stickman.position.z), Time.deltaTime * 0.75f);
                    }
                    else
                    {
                        current_stickman.position = Vector3.Lerp(current_stickman.position, new Vector3(middle_enemy_stickman.position.x, current_stickman.position.y, middle_enemy_stickman.position.z), Time.deltaTime * 0.2f);
                    }
                }
            }

            // You win the fight with enemies
            else
            {
                attack = false;

                StartCoroutine(enemy_manager.disableEnemyArea());

                formatStickman();

                player_speed_vertical = 2.5f;

                for (int i = 0; i < num_player_stickman; i++)
                {
                    parent_stickmans.GetChild(i).rotation = Quaternion.identity;
                }
            }

        }
        
        else
        {
            moveThePlayerHorizontally();
        }

        checkForLoose();

        // Vertical Movement By Player
        if (gamestate)
        {
            transform.Translate(transform.forward * Time.deltaTime * player_speed_vertical);
        }
    }

    public void checkForLoose()
    {
        if (num_player_stickman == 0)
        {
            //Debug.Log("Loose");
            //if (enemy_manager.isActiveAndEnabled)
            //{
            //    enemy_manager.stopAttacking();
            //}
            gamestate = false;
            attack = false;

            ProceduralLevelGenerator.instance.canvas.gameObject.SetActive(true);
            StartCoroutine(ProceduralLevelGenerator.instance.disableAll());

            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public IEnumerator formatAfterObstacle()
    {
        if (is_activated)
        {
            is_activated = false;
            yield return new WaitForSeconds(1.5f);

            formatStickman();

            is_activated = true;
        }
    }

    private void formatStickman()
    {
        num_player_stickman = parent_stickmans.childCount;
        if (num_player_stickman > 90)
        {
            distance_factor = 0.095f;
        }
        else if (num_player_stickman <= 10)
        {
            distance_factor = 0.18f;
        }
        else
        {
            distance_factor = 0.12f;
        }
        Transform trans = parent_stickmans;
        for (int i = 0; i < num_player_stickman; i++)
        {
            var x = distance_factor * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distance_factor * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            var NewPos = new Vector3(x, 0, z);

            trans.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private void moveThePlayerHorizontally()
    {
        if (Input.GetMouseButtonDown(0) && gamestate)
        {
            move_by_touch = true;

            var plane = new Plane(Vector3.up, 0f);

            var ray = main_camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                mouse_start_pos = ray.GetPoint(distance + 1f);
                player_start_pos = transform.position;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            move_by_touch = false;
        }

        if (move_by_touch)
        {
            //A plane is created at y = 0 with a normal vector pointing up (Vector3.up). This plane will be used to calculate the intersection point with a ray cast from the camera.
            //A ray is created from the camera through the current mouse position. This ray will be used to determine where the mouse is pointing in the 3D world.
            var plane = new Plane(Vector3.up, 0f);
            var ray = main_camera.ScreenPointToRay(Input.mousePosition);

            // The Raycast method checks if the ray intersects with the plane.
            // If it does, the intersection distance is stored in the distance variable, and the code inside the block executes.
            if (plane.Raycast(ray, out var distance))
            {
                // This method calculates the exact point in world space where the ray intersects the plane, plus an additional offset of 1 unit along the ray.
                var mousePos = ray.GetPoint(distance + 1f);

                var move = mousePos - mouse_start_pos;

                var control = player_start_pos + move;

                // The x-coordinate of the new position is clamped to ensure the player doesn’t move too far left or right. The clamping range depends on the number of stickmen:
                // If there are more than 50 stickmen, the range is narrower
                if (num_player_stickman > 100)
                    control.x = Mathf.Clamp(control.x, -0.35f, 0.35f);
                else if (num_player_stickman > 70)
                    control.x = Mathf.Clamp(control.x, -0.42f, 0.42f);
                else if (num_player_stickman > 50)
                    control.x = Mathf.Clamp(control.x, -0.5f, 0.5f);
                else if (num_player_stickman > 20)
                    control.x = Mathf.Clamp(control.x, -0.6f, 0.6f);
                else if (num_player_stickman > 5)
                    control.x = Mathf.Clamp(control.x, -0.8f, 0.8f);
                else
                    control.x = Mathf.Clamp(control.x, -1.1f, 1.1f);


                transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * player_speed_horizontal)
                    , transform.position.y, transform.position.z);

            }
        }
    }

    //private void moveThePlayerHorizontally()
    //{
    //    if (Input.GetMouseButtonDown(0) && gamestate)
    //    {
    //        move_by_touch = true;

    //        var plane = new Plane(Vector3.up, 0f);
    //        var ray = main_camera.ScreenPointToRay(Input.mousePosition);

    //        if (plane.Raycast(ray, out var distance))
    //        {
    //            mouse_start_pos = ray.GetPoint(distance + 1f);
    //            player_start_pos = transform.position;
    //        }
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        move_by_touch = false;
    //    }

    //    if (move_by_touch)
    //    {
    //        var plane = new Plane(Vector3.up, 0f);
    //        var ray = main_camera.ScreenPointToRay(Input.mousePosition);

    //        if (plane.Raycast(ray, out var distance))
    //        {
    //            var mousePos = ray.GetPoint(distance + 1f);
    //            var move = mousePos - mouse_start_pos;
    //            var control = player_start_pos + move;

    //            // Calculate swipe speed
    //            float swipeSpeed = move.magnitude / Time.deltaTime;

    //            // Threshold for considering a swipe as a fast swipe
    //            float fastSwipeThreshold = 1.5f; // Adjust this value as needed

    //            // Clamp x-coordinate based on the number of stickmen and fast swipe
    //            if (swipeSpeed > fastSwipeThreshold)
    //            {
    //                if (move.x > 0)
    //                    control.x = player_start_pos.x + Mathf.Clamp(Mathf.Abs(control.x - player_start_pos.x), -1.1f, 1.1f); // swipe right
    //                else
    //                    control.x = player_start_pos.x - Mathf.Clamp(Mathf.Abs(control.x - player_start_pos.x), -1.1f, 1.1f); // swipe left
    //            }
    //            else
    //            {
    //                if (num_player_stickman > 100)
    //                    control.x = Mathf.Clamp(control.x, -0.35f, 0.35f);
    //                else if (num_player_stickman > 70)
    //                    control.x = Mathf.Clamp(control.x, -0.42f, 0.42f);
    //                else if (num_player_stickman > 50)
    //                    control.x = Mathf.Clamp(control.x, -0.5f, 0.5f);
    //                else if (num_player_stickman > 20)
    //                    control.x = Mathf.Clamp(control.x, -0.6f, 0.6f);
    //                else if (num_player_stickman > 5)
    //                    control.x = Mathf.Clamp(control.x, -0.8f, 0.8f);
    //                else
    //                    control.x = Mathf.Clamp(control.x, -1.1f, 1.1f);
    //            }

    //            // Move the player horizontally immediately without interpolation
    //            transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * player_speed_horizontal)
    //                , transform.position.y, transform.position.z);
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy_zone"))
        {
            enemy = other.transform;

            enemy_manager = enemy.GetComponent<EnemyManager>();

            attack = true;

            player_speed_vertical = 0.4f;

            enemy_manager.startAttack(transform);
        }

        if (other.CompareTag("finish"))
        {
            Debug.Log("Won");

            for (int i = 0; i < parent_stickmans.childCount; i++)
            {
                parent_stickmans.GetChild(i).GetComponent<Animator>().SetBool("run", false);
            }

            gamestate = false;

            // Temporary, until we find better option
            StartCoroutine(updateNumberOfPlayerStickman(0));

            ProceduralLevelGenerator.instance.canvas.gameObject.SetActive(true);
            StartCoroutine(ProceduralLevelGenerator.instance.disableAll());

            //StartCoroutine(exitApplication());
        }
    }

    IEnumerator exitApplication()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("Application Quit");

        Application.Quit();
    }

    // Cacn use IEnumerator here
    public IEnumerator updateNumberOfPlayerStickman(int number)
    {
        Transform trans = parent_stickmans;

        if (number >= num_player_stickman)
        {
            for (int i = num_player_stickman; i < number; ++i)
            {
                stickman_spawner.getBlueStickman(trans.position, trans);
            }
        }
        else
        {
            // Temporary, until we find better options
            if (number <= 0)
            {
                number = 0;
                for (int i = num_player_stickman - 1; i >= number; i--)
                {
                    trans.GetChild(i).GetComponent<PlayerStickmanManager>().destroySelf();
                }
            }
            else
            {
                for (int i = num_player_stickman - 1; i >= number; i--)
                {
                    trans.GetChild(i).GetComponent<PlayerStickmanManager>().destroySelf();
                    yield return null;
                }
            }
        }

        num_player_stickman = trans.childCount;
        
        formatStickman();

        move_by_touch = true;
    }
}
