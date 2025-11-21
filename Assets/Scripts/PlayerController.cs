using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float drag = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private TextMeshProUGUI startScreenBestTime;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;

    [Header("Level System")]
    [SerializeField] private int pickupsPerLevel = 7;
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private Transform ground;
    [SerializeField] private int totalLevels = 3;

    private Rigidbody rb;
    private int count;
    private int currentLevel = 1;
    private int pickupsInCurrentLevel = 0;
    private float startTime;
    private float elapsedTime;
    private bool gameStarted = false;
    private bool gameFinished = false;
    private bool countdownActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;

        count = 0;
        currentLevel = 1;
        pickupsInCurrentLevel = 0;
        gameStarted = false;
        gameFinished = false;
        countdownActive = false;

        UpdateCountText();
        UpdateLevelText();

        if (startScreen != null)
        {
            startScreen.SetActive(true);
            DisplayBestTimeOnStart();
        }

        if (winPanel != null)
            winPanel.SetActive(false);

        if (countText != null)
            countText.gameObject.SetActive(false);
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (levelText != null)
            levelText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted && !countdownActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.anyKeyDown))
        {
            StartGame();
        }

        if (gameStarted && !gameFinished)
        {
            elapsedTime = Time.time - startTime;

            if (Time.frameCount % 3 == 0)
            {
                UpdateTimerText();
            }

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            rb.AddForce(movement * moveSpeed);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }

    void StartGame()
    {
        countdownActive = true;
        StartCoroutine(CountdownStart());
    }

    IEnumerator CountdownStart()
    {
        if (startScreen != null)
        {
            yield return StartCoroutine(FadeOutStartScreen());
        }

        GameObject countdownObj = new GameObject("Countdown");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            countdownActive = false;
            yield break;
        }

        countdownObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI countdownText = countdownObj.AddComponent<TextMeshProUGUI>();
        countdownText.fontSize = 120;
        countdownText.fontStyle = FontStyles.Bold;
        countdownText.alignment = TextAlignmentOptions.Center;

        RectTransform rect = countdownObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 150);
        rect.anchoredPosition = Vector2.zero;

        string[] countdown = { "3", "2", "1", "GO!" };

        for (int i = 0; i < countdown.Length; i++)
        {
            countdownText.text = countdown[i];
            countdownText.color = (i < 3) ? Color.yellow : Color.green;

            countdownText.transform.localScale = Vector3.one * 2f;
            float elapsed = 0f;
            while (elapsed < 0.7f)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(2f, 1f, elapsed / 0.7f);
                countdownText.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
        }

        Destroy(countdownObj);

        gameStarted = true;
        startTime = Time.time;

        if (countText != null)
            countText.gameObject.SetActive(true);
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (levelText != null)
            levelText.gameObject.SetActive(true);

        UpdateBallColor();
        SpawnPickups(pickupsPerLevel);
    }

    IEnumerator FadeOutStartScreen()
    {
        CanvasGroup canvasGroup = startScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = startScreen.AddComponent<CanvasGroup>();
        }

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        startScreen.SetActive(false);
    }

    void SpawnPickups(int amount)
    {
        if (pickupPrefab == null || ground == null) return;

        Renderer groundRenderer = ground.GetComponent<Renderer>();
        if (groundRenderer == null) return;

        Bounds bounds = groundRenderer.bounds;
        List<Vector3> spawnedPositions = new List<Vector3>();
        float minDistanceBetweenPickups = 1.5f;

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;
            int maxAttempts = 50;

            while (!validPosition && attempts < maxAttempts)
            {
                float randomX = Random.Range(bounds.min.x + 1f, bounds.max.x - 1f);
                float randomZ = Random.Range(bounds.min.z + 1f, bounds.max.z - 1f);
                float spawnY = bounds.max.y + 1.0f;
                spawnPosition = new Vector3(randomX, spawnY, randomZ);

                validPosition = true;
                foreach (Vector3 existingPos in spawnedPositions)
                {
                    if (Vector3.Distance(spawnPosition, existingPos) < minDistanceBetweenPickups)
                    {
                        validPosition = false;
                        break;
                    }
                }
                attempts++;
            }

            spawnedPositions.Add(spawnPosition);
            GameObject pickup = Instantiate(pickupPrefab, spawnPosition, Quaternion.Euler(45, 45, 45));
            pickup.tag = "Pickup";

            StartCoroutine(PickupSpawnAnimation(pickup));

            Renderer renderer = pickup.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);

                if (currentLevel == 1)
                {
                    renderer.material.color = Color.yellow;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.yellow * 0.5f);
                }
                else if (currentLevel == 2)
                {
                    PickupBounce bounceScript = pickup.AddComponent<PickupBounce>();
                    bounceScript.bounceHeight = 0.25f;
                    bounceScript.bounceSpeed = 2f;

                    renderer.material.color = Color.cyan;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.cyan * 0.8f);
                }
                else if (currentLevel >= 3)
                {
                    PickupBounce bounceScript = pickup.AddComponent<PickupBounce>();
                    PickupPulse pulseScript = pickup.AddComponent<PickupPulse>();

                    bounceScript.bounceHeight = 0.2f;
                    bounceScript.bounceSpeed = 2f;
                    pulseScript.pulseScale = 0.08f;
                    pulseScript.pulseSpeed = 3f;

                    renderer.material.color = Color.magenta;
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.magenta * 1.0f);
                }
            }
        }

        pickupsInCurrentLevel = amount;
    }

    IEnumerator PickupSpawnAnimation(GameObject pickup)
    {
        Vector3 originalScale = pickup.transform.localScale;
        pickup.transform.localScale = Vector3.zero;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration && pickup != null)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, elapsed / duration);
            pickup.transform.localScale = originalScale * scale;
            yield return null;
        }

        if (pickup != null)
            pickup.transform.localScale = originalScale;
    }

    void UpdateBallColor()
    {
        Renderer ballRenderer = GetComponent<Renderer>();
        if (ballRenderer != null)
        {
            ballRenderer.material = new Material(ballRenderer.material);

            if (currentLevel == 1)
                ballRenderer.material.color = Color.white;
            else if (currentLevel == 2)
                ballRenderer.material.color = new Color(0.5f, 0.8f, 1f);
            else if (currentLevel == 3)
                ballRenderer.material.color = new Color(1f, 0.5f, 1f);
        }
    }

    void DisplayBestTimeOnStart()
    {
        if (startScreenBestTime != null)
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

            if (bestTime > 0)
            {
                int minutes = Mathf.FloorToInt(bestTime / 60f);
                int seconds = Mathf.FloorToInt(bestTime % 60f);
                int milliseconds = Mathf.FloorToInt((bestTime * 100f) % 100f);
                startScreenBestTime.text = string.Format("Best Time: {0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            }
            else
            {
                startScreenBestTime.text = "Best Time: --:--.--";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            SpawnCollectEffect(other.transform.position);
            Destroy(other.gameObject);
            count++;
            pickupsInCurrentLevel--;
            UpdateCountText();

            if (pickupsInCurrentLevel <= 0)
            {
                if (currentLevel < totalLevels)
                {
                    currentLevel++;
                    ShowLevelUpText();
                    UpdateLevelText();
                    UpdateBallColor();
                    SpawnPickups(pickupsPerLevel);
                }
                else
                {
                    WinGame();
                }
            }
        }
    }

    void SpawnCollectEffect(Vector3 position)
    {
        GameObject effectObj = new GameObject("CollectEffect");
        effectObj.transform.position = position;

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 3f;
        main.startSize = 0.2f;
        main.maxParticles = 20;
        main.startColor = Color.yellow;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        Destroy(effectObj, 1f);
    }

    void ShowLevelUpText()
    {
        StartCoroutine(LevelUpAnimation());
    }

    IEnumerator LevelUpAnimation()
    {
        GameObject levelUpObj = new GameObject("LevelUpText");
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) yield break;

        levelUpObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI levelUpText = levelUpObj.AddComponent<TextMeshProUGUI>();
        levelUpText.text = "LEVEL " + currentLevel + "!";
        levelUpText.fontSize = 80;
        levelUpText.fontStyle = FontStyles.Bold;
        levelUpText.alignment = TextAlignmentOptions.Center;
        levelUpText.color = Color.yellow;

        RectTransform rect = levelUpObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 100);
        rect.anchoredPosition = Vector2.zero;

        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            levelUpText.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            Color color = levelUpText.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            levelUpText.color = color;

            yield return null;
        }

        Destroy(levelUpObj);
    }

    void UpdateCountText()
    {
        if (countText != null)
            countText.text = "Pickups: " + count.ToString();
    }

    void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel.ToString();
            StartCoroutine(AnimateLevelText());
        }
    }

    IEnumerator AnimateLevelText()
    {
        if (levelText == null) yield break;

        Transform textTransform = levelText.transform;
        Vector3 originalScale = textTransform.localScale;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.5f, elapsed / duration);
            textTransform.localScale = originalScale * scale;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.5f, 1f, elapsed / duration);
            textTransform.localScale = originalScale * scale;
            yield return null;
        }

        textTransform.localScale = originalScale;
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            timerText.text = string.Format("Time: {0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }
    }

    void WinGame()
    {
        gameFinished = true;

        float bestTime = PlayerPrefs.GetFloat("BestTime", 999999f);
        bool newRecord = elapsedTime < bestTime;

        if (newRecord)
        {
            PlayerPrefs.SetFloat("BestTime", elapsedTime);
            PlayerPrefs.Save();
            bestTime = elapsedTime;
        }

        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

            string timeString = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

            if (newRecord)
            {
                finalTimeText.text = "NEW RECORD!\nTime: " + timeString;
                finalTimeText.color = Color.yellow;
            }
            else
            {
                finalTimeText.text = "Your Time: " + timeString;
            }
        }

        if (bestTimeText != null)
        {
            int bestMinutes = Mathf.FloorToInt(bestTime / 60f);
            int bestSeconds = Mathf.FloorToInt(bestTime % 60f);
            int bestMilliseconds = Mathf.FloorToInt((bestTime * 100f) % 100f);
            bestTimeText.text = string.Format("Best Time: {0:00}:{1:00}.{2:00}", bestMinutes, bestSeconds, bestMilliseconds);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
