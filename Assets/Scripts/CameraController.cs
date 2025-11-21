using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0, 7, -7);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool useSmoothing = true;

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player not assigned to CameraController");
            return;
        }

        Vector3 desiredPosition = player.position + offset;

        if (useSmoothing)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else
        {
            transform.position = desiredPosition;
        }
    }
}
