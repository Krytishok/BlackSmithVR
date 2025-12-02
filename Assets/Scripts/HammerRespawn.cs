using UnityEngine;

public class HammerRespawn : MonoBehaviour
{
    [SerializeField]
    private Transform returnPoint;

    /// <summary>
    /// Телепортирует молот обратно на слот и сбрасывает скорость.
    /// </summary>
    public void Respawn()
    {
        if (returnPoint == null)
        {
            Debug.LogWarning("HammerRespawn: returnPoint не задан", this);
            return;
        }

        transform.position = returnPoint.position;
        transform.rotation = returnPoint.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}