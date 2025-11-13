using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Marker : MonoBehaviour
{
    public float destroyDelay = 0.02f; 
    private WorkpieceController owner;

    void Start()
    {
        owner = GetComponentInParent<WorkpieceController>();
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Hammer") || other.CompareTag("PlayerHand") || other.gameObject.layer == LayerMask.NameToLayer("XRHands"))
        {
            if (owner != null)
                owner.NotifyMarkerHit(gameObject);

         
            Destroy(gameObject, destroyDelay);
        }
        else
        {
        
        }
    }
}
