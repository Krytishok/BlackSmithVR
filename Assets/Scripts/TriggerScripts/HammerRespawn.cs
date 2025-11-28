using UnityEngine;

public class HammerRespawn : MonoBehaviour
{
    public Transform returnPoint;

    public void Respawn()
    {
        // Телепортируем на слот
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
