using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StickmanSpawner : MonoBehaviour
{
    [SerializeField] private PlayerStickmanManager player_stickman;

    public ObjectPool<PlayerStickmanManager> pool_player_stickman;

    private Transform parent_blue;

    void Awake()
    {
        pool_player_stickman = new ObjectPool<PlayerStickmanManager>(createPlayerStickman, onTakeStickmanFromPool, onReturnStickmanToPool, onDestroyStickman, true, 100, 200);
        
        preInstantiateBlueStickman(100);
    }

    public void getBlueStickman(Vector3 position, Transform parent)
    {
        parent_blue = parent;
        PlayerStickmanManager stickman = pool_player_stickman.Get();
        stickman.transform.position = position;
    }

    private void preInstantiateBlueStickman(int num_stickman)
    {
        for (int i = 0; i < num_stickman; i++)
        {
            PlayerStickmanManager stickman = createPlayerStickman();
            pool_player_stickman.Release(stickman);
        }
    }

    private PlayerStickmanManager createPlayerStickman()
    {
        PlayerStickmanManager stickman = Instantiate(player_stickman);
        stickman.setPool(pool_player_stickman);

        return stickman;
    }

    private void onTakeStickmanFromPool(PlayerStickmanManager stickman)
    {
        stickman.transform.rotation = Quaternion.identity;
        stickman.transform.parent = parent_blue;
        stickman.gameObject.SetActive(true);
    }

    private void onReturnStickmanToPool(PlayerStickmanManager stickman)
    {
        stickman.gameObject.transform.parent = null;
        stickman.gameObject.SetActive(false);
    }

    private void onDestroyStickman(PlayerStickmanManager stickman)
    {
        Destroy(stickman.gameObject);
    }
}