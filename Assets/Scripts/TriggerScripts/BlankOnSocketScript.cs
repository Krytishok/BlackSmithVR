using UnityEngine;

public class BlankOnSocketScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Guard"))
        {
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
    }
}
