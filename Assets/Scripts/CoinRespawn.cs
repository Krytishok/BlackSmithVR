using UnityEngine;

public class CoinRespawn : MonoBehaviour
{
    [Tooltip("Точка, куда телепортировать монету при попадании в DeadZone")]
    public Transform returnPoint;

    [Tooltip("Если true и returnPoint не задан — просто поднимаем монету вверх на 1 метр")]
    public bool autoRespawnIfMissingReturnPoint = true;

    public void Respawn()
    {
        if (returnPoint != null)
        {
            transform.position = returnPoint.position;
            transform.rotation = returnPoint.rotation;
        }
        else if (autoRespawnIfMissingReturnPoint)
        {
            transform.position = transform.position + Vector3.up * 1.0f;
        }
        else
        {
            Debug.LogWarning($"CoinRespawn: returnPoint not assigned on {gameObject.name}");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }
    }
}
