// Coin.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour
{
    [Header("Play Area Reference")]
    public PlayAreaBounds playArea; // перетащить PlayArea или найти автоматически

    [Header("Respawn / Clamp settings")]
    public float respawnHeight = 1.0f; // высота при телепорте внутрь
    public float clampPadding = 0.1f; // отступ от края
    public float teleportDistanceThreshold = 5f; // если улетела дальше - телепортируем в центр

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playArea == null)
        {
            var paObj = GameObject.Find("PlayArea");
            if (paObj != null)
                playArea = paObj.GetComponent<PlayAreaBounds>();
        }
        if (playArea == null)
            Debug.LogWarning("Coin: PlayAreaBounds not assigned and GameObject 'PlayArea' not found.");
    }

    void FixedUpdate()
    {
        if (playArea == null) return;

        Bounds b = playArea.GetWorldBounds();
        Vector3 pos = transform.position;

        bool outside = pos.x < b.min.x || pos.x > b.max.x ||
                       pos.y < b.min.y || pos.y > b.max.y ||
                       pos.z < b.min.z || pos.z > b.max.z;

        if (!outside) return;

        // Вычисляем зажимаемую позицию
        Vector3 clamped = pos;
        clamped.x = Mathf.Clamp(pos.x, b.min.x + clampPadding, b.max.x - clampPadding);
        clamped.y = Mathf.Clamp(pos.y, b.min.y + clampPadding, b.max.y - clampPadding);
        clamped.z = Mathf.Clamp(pos.z, b.min.z + clampPadding, b.max.z - clampPadding);

        // Если монета "улетела" очень далеко — телепортируем к центру PlayArea на высоту respawnHeight
        if (Vector3.Distance(pos, clamped) > teleportDistanceThreshold)
        {
            clamped = b.center + Vector3.up * respawnHeight;
        }

        // Ставим позицию, обнуляем скорость
        rb.position = clamped;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
