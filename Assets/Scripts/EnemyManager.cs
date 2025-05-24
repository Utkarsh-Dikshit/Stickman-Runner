using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy_stickman;
    [Range(0f, 1f)] [SerializeField] private float distance_factor, radius;

    [SerializeField] private Color enemy_zone_target_color;
    //[SerializeField] private StickmanSpawner stickman_spawner;
    
    public TextMeshPro enemy_stickman_counter;

    private Transform player;
    
    private Animator enemy_zone_animator;
    private SpriteRenderer enemy_zone_sprite;
    
    private int num_enemy_stickman;

    private bool attack;

    public void initializeEnemyArea(int random_min_num, int random_max_num)
    {
        int random_num = Random.Range(random_min_num, random_max_num) - 10;

        if (random_num < 20)
        {
            random_num = 20;
        }

        ProceduralLevelGenerator.instance.max_stickman_possible -= random_num;
        ProceduralLevelGenerator.instance.min_stickman_possible -= random_num;

        for (int i = 0; i < random_num; i++)
        {
            Instantiate(enemy_stickman, transform.position, new Quaternion(0, 180, 0, 1), transform);
        }

        num_enemy_stickman = transform.childCount;
        enemy_stickman_counter.text = num_enemy_stickman.ToString();

        formatEnemyStickMan();
        attack = false;

        GameObject enemy_zone;
        enemy_zone = transform.parent.GetChild(0).gameObject;
        enemy_zone_animator = enemy_zone.GetComponent<Animator>();
        enemy_zone_sprite = enemy_zone.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attack == true && num_enemy_stickman > 0)
        {
            num_enemy_stickman = transform.childCount;
            enemy_stickman_counter.text = num_enemy_stickman.ToString();

            Vector3 direc_to_player = player.position - transform.position;

            for (int i = 0; i < num_enemy_stickman; i++)
            {
                Transform current_trans = transform.GetChild(i);

                current_trans.rotation = Quaternion.Slerp(current_trans.rotation, Quaternion.LookRotation(direc_to_player, Vector3.up), 3f * Time.deltaTime);

                // Enemy stickman targets and moves towards the first player stickman
                if (player.GetChild(1).childCount > 0)
                {
                    // Using the first stickman as the main player stickman
                    Transform middle_player_stickman = player.GetChild(1).GetChild(0);

                    Vector3 distance = middle_player_stickman.position - current_trans.position;

                    if (distance.magnitude < 1.5f)
                    {
                        current_trans.position = Vector3.Lerp(current_trans.position, middle_player_stickman.position, 2f * Time.deltaTime);
                    }
                    else
                    {
                        current_trans.position = Vector3.Lerp(current_trans.position, middle_player_stickman.position, 1f * Time.deltaTime);
                    }
                }
            }
        }
    }

    // Arranging the stickman in spiral format in the X-Z plane
    private void formatEnemyStickMan()
    {
        for (int i = 1; i < num_enemy_stickman; i++)
        {
            var x = distance_factor * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distance_factor * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            Vector3 position = new Vector3(x, 0, z);

            transform.GetChild(i).localPosition = position;
        }
    }

    public void startAttack(Transform player_stickman)
    {
        attack = true;
        player = player_stickman;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("run", true);
        }
    }

    // Temporary, until we find better option. Can be used Later
    //public void stopAttacking()
    //{
    //    attack = false;
    //    for (int i = 0; i < num_enemy_stickman && gameObject.activeInHierarchy; i++)
    //    {
    //        transform.GetChild(i).GetComponent<Animator>().SetBool("run", false);
    //    }
    //    Destroy(this);
    //}

    // When all Enemies died
    public IEnumerator disableEnemyArea()
    {
        attack = false;

        enemy_zone_animator.enabled = false;

        transform.parent.GetChild(1).gameObject.SetActive(false);

        enemy_zone_sprite.DOColor(enemy_zone_target_color, 1f);
        transform.parent.GetChild(0).DOScale(0.8f, 1f);

        yield return new WaitForSecondsRealtime(1.3f);

        Destroy(transform.parent.gameObject);
    }
}