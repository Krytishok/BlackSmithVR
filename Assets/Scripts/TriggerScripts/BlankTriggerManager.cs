using UnityEngine;

public class BlankTriggerManager : MonoBehaviour
{
    [SerializeField] BlankTrigger _blankTrigger;
    [SerializeField] string _tagForBlankTrigger;

    [SerializeField] AnvilTrigger _anvilTrigger;
    [SerializeField] string _tagForAnvilTrigger;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tagForBlankTrigger))
        {
            _blankTrigger.StartHeating();
        }
        else if (other.CompareTag(_tagForAnvilTrigger))
        {
            _anvilTrigger.SetActiveMarker();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tagForBlankTrigger))
        {
            _blankTrigger.StartCooling();
        }
    }
}
