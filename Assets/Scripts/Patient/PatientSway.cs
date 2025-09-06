using UnityEngine;

public class PatientSway : MonoBehaviour
{
    [Header("Rotate Sway")]
    public float rotateAmount = 3f;   // derajat rotasi maksimal
    public float rotateSpeed = 1f;    // kecepatan rotasi

    [Header("Position Sway")]
    public Vector3 positionSwayAmount = new Vector3(0.02f, 0.05f, 0); // x, y, z
    public float positionSwaySpeed = 1f;

    private Vector3 initialPos;
    private float rotatePhase;
    private float positionPhase;

    private void Awake()
    {
        initialPos = transform.localPosition;
        rotatePhase = Random.Range(0f, Mathf.PI * 2);
        positionPhase = Random.Range(0f, Mathf.PI * 2);
    }

    private void Update()
    {
        // Rotasi halus atas-bawah
        float angle = Mathf.Sin(Time.time * rotateSpeed + rotatePhase) * rotateAmount;
        transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        // Gerakan halus posisi (misal maju-mundur dan sedikit samping)
        Vector3 offset = new Vector3(
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase) * positionSwayAmount.x,
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase * 1.5f) * positionSwayAmount.y,
            Mathf.Sin(Time.time * positionSwaySpeed + positionPhase * 0.7f) * positionSwayAmount.z
        );
        transform.localPosition = initialPos + offset;
    }
}
