using UnityEngine;

public class ThermometerNeedle : MonoBehaviour
{
    [Header("Heat")]
    [SerializeField] private float minHeat = 0f;
    [SerializeField] private float maxHeat = 2f;

    [Header("Rotation (Z axis)")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public void SetHeat(float heat)
    {
        float t = Mathf.InverseLerp(minHeat, maxHeat, heat);
        float angle = Mathf.Lerp(minAngle, maxAngle, t);

        // ВАЖНО: Quaternion + Z
        rect.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
