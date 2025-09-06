using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Patient Settings")]
    public GameObject patientPrefab;
    public Transform spawnPoint;
    public PatientManager patientManager;
    public PatientVisualManager visualManager;

    private GameObject currentPatientGO;
    private Patient currentPatient;

    [Header("Timer")]
    public float totalTime = 60f;
    private float remainingTime;
    private bool gameEnded = false;

    [Header("Score")]
    public int basePoints = 500;
    public int penaltyPerDeviation = 150;
    public int multiplier = 5;
    private int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        remainingTime = totalTime;
        ScoreManager.Instance.UpdateScore(score, 0);
        StartCoroutine(TimerCoroutine());
        SpawnNextPatient();
    }

    private IEnumerator TimerCoroutine()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            ScoreManager.Instance.UpdateTimer(remainingTime);
            yield return null;
        }
        EndGame();
    }

    private void EndGame()
    {
        gameEnded = true;
        ScoreManager.Instance.ShowFinalScore(score);
    }

    public void SubmitAnswer(string userRisk)
    {
        if (currentPatient == null || gameEnded) return;

        int patientRisk = currentPatient.GetNumericRisk();
        int submittedRisk = Patient.ConvertRiskToNumeric(userRisk);
        int deviation = Mathf.Abs(patientRisk - submittedRisk);

        int delta = Mathf.RoundToInt((basePoints - penaltyPerDeviation * deviation) * multiplier / Mathf.Max(remainingTime, 1f));
        score += delta;
        ScoreManager.Instance.UpdateScore(score, delta);

        PatientMover mover = currentPatientGO.GetComponent<PatientMover>();
        if (mover != null) mover.OnPatientSubmitted();

        StartCoroutine(WaitPatientRightThenNext(mover));
    }

    private IEnumerator WaitPatientRightThenNext(PatientMover mover)
    {
        if (mover != null)
            while (!mover.IsAtRight()) yield return null;

        if (currentPatientGO != null) Destroy(currentPatientGO);

        SpawnNextPatient();
    }

    private void SpawnNextPatient()
    {
        currentPatient = patientManager.GeneratePatient();
        currentPatientGO = Instantiate(patientPrefab, spawnPoint.position, Quaternion.identity);

        PatientMover mover = currentPatientGO.GetComponent<PatientMover>();

        if (visualManager != null)
            visualManager.SetupPatient(currentPatient);

        PatientUI.Instance.ShowPatient(currentPatient);
    }

    public int GetScore() => score;
}
