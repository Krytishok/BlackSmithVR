using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CraftManager : MonoBehaviour
{
    [SerializeField] MarkerScript _marker;
    [SerializeField] GameObject[] _craftSockets;

    private int _socketsMounted = 0;

    private int _currentSocket = 0;

    private GameObject _component;


    private void Start()
    {
        SetActiveSockets(false);
    }


    public void SetActiveSockets(bool active)
    {
        if(_craftSockets != null)
        {
            for (int i = 0; i < _craftSockets.Length; i++)
            {
                _craftSockets[i].SetActive(active);
            }
        }
        else
        {
            Debug.Log("Сокеты для крафта не назначены!");
        }
        
    }

    public void SetActiveCurrentSocket(bool active)
    {
        if(_craftSockets != null)
        {
            SetActiveSockets(false);


            _craftSockets[_currentSocket].SetActive(active);
        }
    }

    public void SocketSelected()
    {
        _marker.gameObject.SetActive(true);
        _marker.TeleportTo(_craftSockets[_currentSocket].gameObject.transform.localPosition, false);
        _marker.OnTriggerActivated += ComponentMounted;

    }
    public void SocketExited()
    {
        _marker.OnTriggerActivated -= ComponentMounted;
        _marker.gameObject.SetActive(false);
    }

    private void ComponentMounted()
    {
        ApplyComponent();

        _craftSockets[_currentSocket].GetComponent<XRSocketInteractor>().socketActive = false;

        Destroy(_component);

        _socketsMounted++;
        _marker.gameObject.SetActive(false);
        _currentSocket++;
        _craftSockets[_currentSocket].SetActive(true);
    }


    private void ApplyComponent()
    {
        //Getting HeldGameObject From Socket
        _component = _craftSockets[_currentSocket].GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.GetChild(0).gameObject;


        _craftSockets[_currentSocket].GetComponentInChildren<MeshFilter>().mesh = _component.GetComponent<MeshFilter>().mesh;
        _craftSockets[_currentSocket].GetComponentInChildren<MeshRenderer>().materials = _component.GetComponent<MeshRenderer>().materials;

        _craftSockets[_currentSocket].GetComponentInChildren<Transform>().transform.localPosition = _component.transform.localPosition;
        _craftSockets[_currentSocket].GetComponentInChildren<Transform>().transform.localRotation = _component.transform.localRotation;
        _craftSockets[_currentSocket].GetComponentInChildren<Transform>().transform.localScale = _component.transform.localScale;
    }
   
}
