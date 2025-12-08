using UnityEngine;

public class DeadZoneTrigger : MonoBehaviour
{
    [SerializeField] InteractableManager interactableManager;
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Hammer"))
    {
        HammerRespawn respawn = other.GetComponent<HammerRespawn>();
        if (respawn != null)
        {
            respawn.Respawn();
        }
        return;
    }

    if (other.CompareTag("Blankable"))
    {
            
            interactableManager.SpawBlank();
            Destroy(other.gameObject, 3f);
            Debug.Log("������� ������� �� ������� ����");
        }
    }
}
