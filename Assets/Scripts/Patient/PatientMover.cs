using UnityEngine;

public class PatientMover : MonoBehaviour
{
    public Transform leftPoint;
    public Transform centerPoint;
    public Transform rightPoint;
    public float moveSpeed = 2f;

    private Transform targetPoint;
    private bool movingToRight = false;
    private bool patientSubmitted = false;

    private void Start()
    {
        targetPoint = centerPoint;
    }

    private void Update()
    {
        if (targetPoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                if(patientSubmitted && targetPoint == rightPoint)
                {
                    // Pasien sudah sampai kanan, siap dihancurkan/dihandle GameManager
                }
            }
        }
    }

    public void OnPatientSubmitted()
    {
        patientSubmitted = true;
        targetPoint = rightPoint;
    }

    public bool IsAtRight()
    {
        return targetPoint == rightPoint && Vector3.Distance(transform.position, rightPoint.position) < 0.01f;
    }
}
