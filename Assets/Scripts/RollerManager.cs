using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerManager : MonoBehaviour
{ 
    private float rotationSpeed = 150f;

    private Transform roller_trans;

    void Start()
    {
        roller_trans = transform.GetChild(0);
    }

    void Update()
    {
        roller_trans.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
