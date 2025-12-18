using System;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField] GameObject[] _craftSockets;


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
}
