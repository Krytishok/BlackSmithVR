using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeadZoneTrigger : MonoBehaviour
{
    [Tooltip("Если true — будет логировать попадания монет в DeadZone")]
    public bool debugLog = false;

    private void Reset()
    {
        // делаем коллайдер триггером по умолчанию, если компонент добавлен через Add Component
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        // Обрабатываем только монеты
        if (other.CompareTag("Coin"))
        {
            CoinRespawn coinRespawn = other.GetComponent<CoinRespawn>();
            if (coinRespawn != null)
            {
                if (debugLog) Debug.Log($"DeadZone: Respawning coin {other.gameObject.name}");
                coinRespawn.Respawn();
            }
            else
            {
                // если скрипт не прикреплён — делаем базовый ресет позиции/скорости
                if (debugLog) Debug.Log($"DeadZone: Coin without CoinRespawn encountered: {other.gameObject.name}");
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                // запасной подъём наверх
                other.transform.position += Vector3.up * 1f;
            }
        }
    }
}
