using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Blood : MonoBehaviour
{
    private ObjectPool<Blood> pool;

    [HideInInspector] public bool is_red_blue_blood;

    private void OnEnable()
    {
        StartCoroutine(disableAfterTime());
    }

    private IEnumerator disableAfterTime()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        
        pool.Release(this);
    }

    public void setBool(bool value)
    {
        is_red_blue_blood = value;
    }
           
    public void setPool(ObjectPool<Blood> pool)
    {
        this.pool = pool;
    }
}
