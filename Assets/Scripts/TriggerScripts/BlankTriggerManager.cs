using UnityEngine;
using System.Collections.Generic;

public class BlankTriggerManager : MonoBehaviour
{
    [SerializeField] BlankTrigger _blankTrigger;
    [SerializeField] string _tagForBlankTrigger;

    [SerializeField] AnvilTrigger _anvilTrigger;
    [SerializeField] string _tagForAnvilTrigger;
    [SerializeField] float _minHeatValueForMarker;

    [SerializeField] CraftManager _craftManager;
    [SerializeField] string _tagForCraftTable = "CraftTable";

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
            if (_blankTrigger._currentHeatLevel > _minHeatValueForMarker)
            {
                _anvilTrigger.SetActiveMarker();
            }
        }
        else if (other.CompareTag(_tagForBarrel))
        {
            _blankTrigger.AccelerateCooling();
        }
        else if (other.CompareTag(_tagForCraftTable))
        {
            if (_anvilTrigger._isWorkDone)
            {
                _craftManager.SetActiveCurrentSocket(true);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tagForBlankTrigger))
        {
            _blankTrigger.StartCooling();
        } else if (other.CompareTag(_tagForCraftTable))
        {
            _craftManager.SetActiveSockets(false);
        }
    }


    private void MeshChange()
    {
        _blankTrigger.ReplaceSword(_anvilTrigger.ShowMeshByIndex());
    }

    public List<string> GetListWithComponents()
    {
        List<string> list = _craftManager.craftNames;

        return list;
    }
}
