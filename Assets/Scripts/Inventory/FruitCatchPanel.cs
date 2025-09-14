using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FruitCatchMinigame : MonoBehaviour
{
    [Header("Referensi")]
    public RectTransform[] fruitPrefabs;
    public RectTransform spawnArea;        
    public RectTransform basket;           

    [Header("Teks Feedback")]
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failedText;
    public TextMeshProUGUI intakeText;

    public Button exitButton;

    [Header("Pengaturan")]
    public float gameDuration = 10f;          
    public float spawnIntervalEasy = 1.5f;
    public float spawnIntervalHard = 0.8f;
    public float fallSpeedEasy = 200f;   
    public float fallSpeedHard = 350f;
    public float basketBoundary = 300f;  

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip catchSound;

    private float spawnTimer;
    private bool running = false;
    private Patient lastPatient;

    private float currentSpawnInterval;
    private float currentFallSpeed;
    private Vector2 basketStartPos;

    private int fruitsToSpawn;
    private int fruitsSpawned;
    private int fruitsActive;
    private int fruitsCaught;

    private void Start()
    {
        basketStartPos = basket.anchoredPosition;

        if (exitButton != null)
            exitButton.onClick.AddListener(ClosePanel);

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);
        if (intakeText != null) intakeText.text = "";
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

        // Basket mengikuti mouse
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            spawnArea, Input.mousePosition, null, out localPoint);

        float clampedX = Mathf.Clamp(localPoint.x, basketStartPos.x - basketBoundary, basketStartPos.x + basketBoundary);
        basket.anchoredPosition = new Vector2(clampedX, basketStartPos.y);

        // Spawn buah
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && fruitsSpawned < fruitsToSpawn)
        {
            SpawnFruit();
            fruitsSpawned++;
            spawnTimer = currentSpawnInterval;
        }

        // Selesai jika semua buah spawn dan tidak ada buah aktif
        if (fruitsSpawned >= fruitsToSpawn && fruitsActive <= 0)
        {
            EndGame();
        }
    }

    private void SetupGame(Patient p)
    {
        // Hapus semua buah sebelumnya
        foreach (Transform child in spawnArea)
        {
            if (child != basket) Destroy(child.gameObject);
        }

        basket.anchoredPosition = basketStartPos;

        // Tentukan difficulty berdasarkan data pasien
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
        fruitsCaught = 0;

        spawnTimer = 0f;
        running = true;

        if (successText != null) successText.gameObject.SetActive(false);
        if (failedText != null) failedText.gameObject.SetActive(false);
        if (intakeText != null) intakeText.text = $"Asupan buah pasien: {fruitsCaught}/{fruitsToSpawn}";
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

        // Success jika tangkap ≥ 90% buah yang spawn
        bool success = fruitsCaught >= Mathf.CeilToInt(fruitsToSpawn * 0.9f);

        if (successText != null) successText.gameObject.SetActive(success);
        if (failedText != null) failedText.gameObject.SetActive(!success);

        if (intakeText != null)
            intakeText.text = $"Asupan buah pasien: {fruitsCaught}/{fruitsToSpawn}";

        // Update dropdown pasien: 0 = kurang, 1 = cukup
        if (lastPatient != null)
        {
            int requiredFruits = 3; // threshold cukup
            lastPatient.fruit = (fruitsCaught >= requiredFruits) ? 1 : 0;

            if (PatientUI.Instance != null)
            {
                PatientUI.Instance.FillField("Fruit");
                PatientUI.Instance.RefreshDropdowns(lastPatient);
            }
        }

        // Update score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddGameResult(success);
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // Inner class untuk buah jatuh
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

            // Buah jatuh di luar spawn area
            if (rect.anchoredPosition.y < -spawnArea.rect.height / 2f - 50f)
            {
                Destroy(gameObject);
                return;
            }

            // Buah kena basket
            if (RectTransformUtility.RectangleContainsScreenPoint(basket, rect.position))
            {
                if (game.audioSource != null && game.catchSound != null)
                    game.audioSource.PlayOneShot(game.catchSound);

                game.fruitsCaught++;
                if (game.intakeText != null)
                    game.intakeText.text = $"Asupan buah pasien: {game.fruitsCaught}/{game.fruitsToSpawn}";

                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (game != null) game.fruitsActive--;
        }
    }
}
