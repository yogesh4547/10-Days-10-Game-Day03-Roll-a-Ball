using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(15, 30, 45);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
