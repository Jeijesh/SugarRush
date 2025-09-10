using UnityEngine;
using TMPro;

public class FruitCatchMinigame : MonoBehaviour
{
    [Header("References")]
    public RectTransform fruitPrefab;     // prefab buah (UI Image)
    public RectTransform spawnArea;       // parent area tempat buah jatuh (UI Panel)
    public RectTransform basket;          // basket (UI Image)
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    public float gameDuration = 10f;
    public float spawnIntervalEasy = 1.5f;
    public float spawnIntervalHard = 0.8f;
    public float fallSpeedEasy = 200f;   // pixel/sec
    public float fallSpeedHard = 350f;
    public float basketBoundary = 300f;  // max offset kiri/kanan dari posisi awal

    private float timer;
    private float spawnTimer;
    private bool running = false;
    private Patient lastPatient;

    private float currentSpawnInterval;
    private float currentFallSpeed;
    private Vector2 basketStartPos;

    private void Start()
    {
        basketStartPos = basket.anchoredPosition; // simpan posisi awal basket
    }

    private void Update()
    {
        // 🔹 Reset kalau pasien baru
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

        // 🔹 Gerak basket mengikuti kursor
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            spawnArea, Input.mousePosition, null, out localPoint);

        float clampedX = Mathf.Clamp(localPoint.x, basketStartPos.x - basketBoundary, basketStartPos.x + basketBoundary);
        basket.anchoredPosition = new Vector2(clampedX, basketStartPos.y);

        // 🔹 Timer jalan
        timer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnFruit();
            spawnTimer = currentSpawnInterval;
        }

        if (timer <= 0f)
        {
            EndGame();
        }
    }

    private void SetupGame(Patient p)
    {
        // 🔹 Hapus semua buah lama
        foreach (Transform child in spawnArea)
        {
            if (child != basket) Destroy(child.gameObject);
        }

        basket.anchoredPosition = basketStartPos;

        // 🔹 Difficulty
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

        timer = gameDuration;
        spawnTimer = 0f;
        running = true;

        feedbackText.text = "";
    }

    private void SpawnFruit()
    {
        RectTransform fruit = Instantiate(fruitPrefab, spawnArea);
        float randomX = Random.Range(-basketBoundary, basketBoundary);
        fruit.anchoredPosition = new Vector2(randomX, spawnArea.rect.height / 2f + 50f);
        fruit.gameObject.AddComponent<FruitFall>().Init(this, currentFallSpeed, basket);
    }

    private void EndGame()
    {
        running = false;
        if (lastPatient != null)
        {
            feedbackText.text = (lastPatient.fruit == 1) ? "Sufficient" : "Insufficient";
        }
    }

    // 🔹 Inner class: logic buah jatuh
    private class FruitFall : MonoBehaviour
    {
        private float speed;
        private RectTransform rect;
        private RectTransform basket;
        private FruitCatchMinigame game;

        public void Init(FruitCatchMinigame game, float fallSpeed, RectTransform basket)
        {
            this.game = game;
            this.speed = fallSpeed;
            this.basket = basket;
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (rect == null) return;

            rect.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);

            // hilang kalau jatuh di bawah layar
            if (rect.anchoredPosition.y < -Screen.height / 2f)
            {
                Destroy(gameObject);
                return;
            }

            // deteksi tabrakan kasar pakai posisi
            if (RectTransformUtility.RectangleContainsScreenPoint(basket, rect.position))
            {
                Destroy(gameObject);
            }
        }
    }
}
