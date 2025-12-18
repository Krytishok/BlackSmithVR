using UnityEngine;

public class ForgeTrigger : MonoBehaviour
{
    [SerializeField] private ForgeManager forgeManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BlankTrigger blank))
        {
            forgeManager.BlankEnteredForge(blank);
            blank.StartHeating(); // логично сразу здесь
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BlankTrigger blank))
        {
            forgeManager.BlankLeftForge(blank);
            blank.StartCooling();
        }
    }
}
