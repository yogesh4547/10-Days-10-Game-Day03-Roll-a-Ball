using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 0.3f;
    public float bounceSpeed = 2f;

    [Header("Ground Safety")]
    [SerializeField] private float minHeightAboveGround = 0.3f;

    private Vector3 startPosition;
    private float groundY;

    void Start()
    {
        startPosition = transform.position;


        groundY = 0f;


        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundY = groundRenderer.bounds.max.y;
            }
        }
    }

    void Update()
    {
        float bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        float newY = startPosition.y + bounceOffset;


        float safeY = Mathf.Max(newY, groundY + minHeightAboveGround);

        transform.position = new Vector3(startPosition.x, safeY, startPosition.z);
    }
}
