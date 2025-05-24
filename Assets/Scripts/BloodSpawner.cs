using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;

public class BloodSpawner : MonoBehaviour
{
    [SerializeField] private Blood blood_red_blue;
    [SerializeField] private Blood blood_blue;

    private int active_red_blue_count = 0;
    private int active_blue_count = 0;
    private int max_red_blue_count = 30;
    private int max_blue_count = 20;

    public ObjectPool<Blood> pool_red_blue;
    public ObjectPool<Blood> pool_blue;
    
    void Awake()
    {
        pool_red_blue = new ObjectPool<Blood>(createRedBlueBlood, onTakeBloodFromPool, onReturnBloodToPool, onDestroyBlood, true, 35, 50);
        pool_blue = new ObjectPool<Blood>(createBlueBlood, onTakeBloodFromPool, onReturnBloodToPool, onDestroyBlood, true, 20, 40);
        
        preInstantiateRedAndBlueBlood(35);
        preInstantiateBlueBlood(20);
    }

    public void getRedBlueBlood(Vector3 position)
    {
        if (active_red_blue_count <= max_red_blue_count)
        {
            Blood blood = pool_red_blue.Get();
            blood.transform.position = position;
            active_red_blue_count++;
        }
    }

    public void getBlueBlood(Vector3 position)
    {
        if (active_blue_count <= max_blue_count)
        {
            Blood blood = pool_blue.Get();
            blood.transform.position = position;
            active_blue_count++;
        }
    }

    private void preInstantiateRedAndBlueBlood(int num_preinstantiate)
    {
        for (int i = 0; i < num_preinstantiate; i++)
        {
            Blood blood = createRedBlueBlood();
            active_red_blue_count++;
            pool_red_blue.Release(blood);
        }
    }

    private void preInstantiateBlueBlood(int num_preinstantiate)
    {
        for (int i = 0; i < num_preinstantiate; i++)
        {
            Blood blood = createBlueBlood();
            active_blue_count++;
            pool_blue.Release(blood);
        }
    }

    private Blood createRedBlueBlood()
    {
        Blood blood = Instantiate(blood_red_blue);
        blood.setBool(true);
        blood.setPool(pool_red_blue);

        return blood;
    }

    private Blood createBlueBlood()
    {
        Blood blood = Instantiate(blood_blue);
        blood.setBool(false);
        blood.setPool(pool_blue);

        return blood;
    }

    private void onTakeBloodFromPool(Blood blood)
    {        
        blood.transform.rotation = Quaternion.identity;

        blood.gameObject.SetActive(true);
    }

    private void onReturnBloodToPool(Blood blood)
    {
        blood.gameObject.SetActive(false);
        if (blood.is_red_blue_blood == true)
        {
            active_red_blue_count--;
        }
        else
        {
            active_blue_count--;
        }
    }
    
    private void onDestroyBlood(Blood blood)
    {
        Destroy(blood.gameObject);
    }
}
