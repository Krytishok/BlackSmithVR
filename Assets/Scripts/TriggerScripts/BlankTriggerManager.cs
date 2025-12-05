using UnityEngine;

public class BlankTriggerManager : MonoBehaviour
{
    [SerializeField] BlankTrigger _blankTrigger;
    [SerializeField] string _tagForBlankTrigger;

    [SerializeField] AnvilTrigger _anvilTrigger;
    [SerializeField] string _tagForAnvilTrigger;
    [SerializeField] float _minHeatValueForMarker;


    private void Start()
    {
        _anvilTrigger.OnMeshChanged += MeshChange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tagForBlankTrigger))
        {
            _blankTrigger.StartHeating();
        }
        else if (other.CompareTag(_tagForAnvilTrigger))
        {
            if(_blankTrigger._currentHeatLevel > _minHeatValueForMarker)
            {
                _anvilTrigger.SetActiveMarker();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tagForBlankTrigger))
        {
            _blankTrigger.StartCooling();
        }
    }


    private void MeshChange()
    {
        _blankTrigger.ReplaceSword(_anvilTrigger.ShowMeshByIndex());
    }
}
