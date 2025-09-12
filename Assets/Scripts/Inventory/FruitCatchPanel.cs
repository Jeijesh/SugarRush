using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FruitCatchMinigame : MonoBehaviour
{
    [Header("References")]
    public RectTransform[] fruitPrefabs;
    public RectTransform spawnArea;        
    public RectTransform basket;           
    public TextMeshProUGUI feedbackText;
    public Button exitButton;

    [Header("Settings")]
    public float gameDuration = 10f;          
    public float spawnIntervalEasy = 1.5f;
    public float spawnIntervalHard = 0.8f;
    public float fallSpeedEasy = 200f;   
    public float fallSpeedHard = 350f;
    public float basketBoundary = 300f;  

    [Header("Audio")]
    public AudioSource audioSource;       // 🔹 audio source (drag dari Inspector)
    public AudioClip catchSound;          // 🔹 suara buah ketangkap

    private float spawnTimer;
    private bool running = false;
    private Patient lastPatient;

    private float currentSpawnInterval;
    private float currentFallSpeed;
    private Vector2 basketStartPos;

    private int fruitsToSpawn;
    private int fruitsSpawned;
    public int fruitsActive;

    private void Start()
    {
        basketStartPos = basket.anchoredPosition;

        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);
    }

    private void Update()
    {
        if (PatientUI.Instance != null && PatientUI.Instance.currentPatient != null)
        {
            Patient p = PatientUI.Instance.currentPatient;
            if (p != lastPatient)
            {
                SetupGame(p);
                lastPatient = p;
            }
        }

        if (!running) return;

        // basket follow mouse
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            spawnArea, Input.mousePosition, null, out localPoint);

        float clampedX = Mathf.Clamp(localPoint.x, basketStartPos.x - basketBoundary, basketStartPos.x + basketBoundary);
        basket.anchoredPosition = new Vector2(clampedX, basketStartPos.y);

        // spawn logic
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && fruitsSpawned < fruitsToSpawn)
        {
            SpawnFruit();
            fruitsSpawned++;
            spawnTimer = currentSpawnInterval;
        }

        if (fruitsSpawned >= fruitsToSpawn && fruitsActive <= 0)
        {
            EndGame();
        }
    }

    private void SetupGame(Patient p)
    {
        foreach (Transform child in spawnArea)
        {
            if (child != basket) Destroy(child.gameObject);
        }

        basket.anchoredPosition = basketStartPos;

        if (p.fruit == 1)
        {
            currentSpawnInterval = spawnIntervalEasy;
            currentFallSpeed = fallSpeedEasy;
        }
        else
        {
            currentSpawnInterval = spawnIntervalHard;
            currentFallSpeed = fallSpeedHard;
        }

        fruitsToSpawn = Mathf.CeilToInt(gameDuration / currentSpawnInterval);
        fruitsSpawned = 0;
        fruitsActive = 0;

        spawnTimer = 0f;
        running = true;
        feedbackText.text = "";
    }

    private void SpawnFruit()
    {
        int index = Random.Range(0, fruitPrefabs.Length);
        RectTransform fruit = Instantiate(fruitPrefabs[index], spawnArea);

        float randomX = Random.Range(-basketBoundary, basketBoundary);
        fruit.anchoredPosition = new Vector2(randomX, spawnArea.rect.height / 2f + 50f);

        fruitsActive++;
        fruit.gameObject.AddComponent<FruitFall>().Init(this, currentFallSpeed, basket, spawnArea);
    }

    private void EndGame()
    {
        running = false;
        if (lastPatient != null)
        {
            feedbackText.text = (lastPatient.fruit == 1) ? "Sufficient" : "Insufficient";
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // Inner class
    private class FruitFall : MonoBehaviour
    {
        private float speed;
        private RectTransform rect;
        private RectTransform basket;
        private FruitCatchMinigame game;
        private RectTransform spawnArea;

        public void Init(FruitCatchMinigame game, float fallSpeed, RectTransform basket, RectTransform spawnArea)
        {
            this.game = game;
            this.speed = fallSpeed;
            this.basket = basket;
            this.spawnArea = spawnArea;
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (rect == null) return;

            rect.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);

            // buah jatuh keluar
            if (rect.anchoredPosition.y < -spawnArea.rect.height / 2f - 50f)
            {
                Destroy(gameObject);
                return;
            }

            // buah kena basket
            if (RectTransformUtility.RectangleContainsScreenPoint(basket, rect.position))
            {
                // 🔹 play sound saat ketangkap
                if (game.audioSource != null && game.catchSound != null)
                {
                    game.audioSource.PlayOneShot(game.catchSound);
                }

                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (game != null) game.fruitsActive--;
        }
    }
}
