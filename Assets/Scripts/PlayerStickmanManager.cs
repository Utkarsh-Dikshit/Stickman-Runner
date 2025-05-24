using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerStickmanManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem red_and_blue_blood;
    [SerializeField] private ParticleSystem single_blue_blood;

    private BloodSpawner blood_spawner;

    private Animator stickman_animator;
    private Collider col;

    private ObjectPool<PlayerStickmanManager> pool_blue_stickman;

    private void Start()
    {
        blood_spawner = ProceduralLevelGenerator.instance.gameObject.GetComponent<BloodSpawner>();
        col = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        stickman_animator = GetComponent<Animator>();
        stickman_animator.SetBool("run", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("red_stickman") && other.transform.parent.childCount > 0)
        {
            destroyStickmen(other.gameObject);
        }
        else if (other.CompareTag("obstacle"))
        {
            if (PlayerManager.instance == null)
            {
                Debug.Log("NULL");
            }

            PlayerManager.instance.StartCoroutine(PlayerManager.instance.formatAfterObstacle());
            destroySelf();
        }
    }

    private void destroyStickmen(GameObject redStickman)
    {
        // Disable colliders to prevent further collisions during this frame
        col.enabled = false;

        redStickman.GetComponent<Collider>().enabled = false;

        Destroy(redStickman);
        pool_blue_stickman.Release(this);

        blood_spawner.getRedBlueBlood(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z));

    }

    public void destroySelf()
    {
        // Disable collider to prevent further collisions during this frame

        col.enabled = false;

        pool_blue_stickman.Release(this);

        blood_spawner.getBlueBlood(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z));
    }

    public void setPool(ObjectPool<PlayerStickmanManager> pool_blue_stickman)
    {
        this.pool_blue_stickman = pool_blue_stickman;
    }

}
