using System;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CraftManager : MonoBehaviour
{
    [SerializeField] MarkerScript _marker;
    [SerializeField] GameObject[] _craftSockets;

    private bool _IsFinished = false;

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
                //_craftSockets[i].SetActive(active); ВРЕМЕННО!
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

        //_marker.gameObject.SetActive(false);
    }

    private void ComponentMounted()
    {
        ApplyComponent();

        _marker.PlaySound();

        _marker.gameObject.SetActive(false);
        _currentSocket++;
        if(_currentSocket < _craftSockets.Length)
        {
            _craftSockets[_currentSocket].SetActive(true);
        }
        else
        {
            _currentSocket = _craftSockets.Length - 1;
        }
        
    }

    
    private void ApplyComponent() // Удаляет указанный слой из маски Interaction Layer, сохраняя остальные слои активными
    {
        //Getting HeldGameObject From Socket
        _component = _craftSockets[_currentSocket].GetComponent<XRSocketInteractor>().interactablesSelected[0].transform.gameObject;

        int maskToRemove = InteractionLayerMask.GetMask("Default");

        _component.GetComponent<XRGrabInteractable>().interactionLayers &= ~maskToRemove;

    }
   
}
