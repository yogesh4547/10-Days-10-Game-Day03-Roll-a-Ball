using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseScale = 0.08f;
    public float pulseSpeed = 3f;

    [Header("Ground Safety")]
    [SerializeField] private float minScaleMultiplier = 0.95f;

    [Header("Performance")]
    [SerializeField] private int updateEveryNFrames = 2;

    private Vector3 originalScale;
    private int frameCount = 0;
    private bool initialized = false;
    void Start()
    {
        Invoke("InitializePulse", 0.5f);
    }

    void InitializePulse()
    {
        originalScale = transform.localScale; initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        frameCount++;
        if (frameCount % updateEveryNFrames != 0)
            return;

        float pulseOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseScale;

        float scaleMultiplier = Mathf.Max(1f + pulseOffset, minScaleMultiplier);

        transform.localScale = originalScale * scaleMultiplier;
    }
}

