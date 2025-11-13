using UnityEngine;

public class DeadZoneTrigger : MonoBehaviour
{
    [SerializeField] InteractableManager interactableManager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Blankable")
        {
            
            interactableManager.SpawBlank();
            Destroy(other.gameObject, 3f);
            Debug.Log("Предмет вылетел из игровой зоны");
        }
    }
}
