using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeadZoneTrigger : MonoBehaviour
{
    [Tooltip("Если true — логировать попадания объектов в DeadZone")] 
    [SerializeField] private bool debugLog = false;

    private void Reset()
    {
        // делаем коллайдер триггером по умолчанию, если компонент добавлен через Add Component
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void HandleCoin(Collider other)
    {
        CoinRespawn coinRespawn = other.GetComponent<CoinRespawn>();
        if (coinRespawn != null)
        {
            if (debugLog) Debug.Log($"DeadZone: respawn coin {other.gameObject.name}");
            coinRespawn.Respawn();
        }
        else
        {
            // если скрипт не прикреплён — просто обнулим скорость и чуть поднимем
            if (debugLog) Debug.Log($"DeadZone: coin without CoinRespawn {other.gameObject.name}");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            other.transform.position += Vector3.up * 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other) return;

        // ----- МОЛОТ -----
        if (other.CompareTag("Hammer"))
        {
            HammerRespawn respawn = other.GetComponent<HammerRespawn>();
            if (respawn != null)
            {
                if (debugLog) Debug.Log($"DeadZone: respawn hammer {other.gameObject.name}");
                respawn.Respawn();
            }
            return;
        }

        // ----- МОНЕТА (при входе в зону) -----
        if (other.CompareTag("Coin"))
        {
            HandleCoin(other);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other) return;

        // ----- МОНЕТА (при выходе из зоны) -----
        if (other.CompareTag("Coin"))
        {
            HandleCoin(other);
        }
    }
}
