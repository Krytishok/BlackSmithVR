using UnityEngine;

public class BlankTriggerManager : MonoBehaviour
{
    [SerializeField] BlankTrigger _blankTrigger;
    [SerializeField] string _tagForBlankTrigger;

    [SerializeField] AnvilTrigger _anvilTrigger;
    [SerializeField] string _tagForAnvilTrigger;
    [SerializeField] float _minHeatValueForMarker;

    [SerializeField] string _tagForBarrel;


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
        } else if (other.CompareTag(_tagForBarrel))
        {
            _blankTrigger.AccelerateCooling();
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
