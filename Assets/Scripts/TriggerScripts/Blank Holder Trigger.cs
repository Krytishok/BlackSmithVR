using System;
using UnityEngine;

public class BlankHolderTrigger : MonoBehaviour
{
    [SerializeField] InteractableManager _InteractableManger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Blankable")
        {
            _InteractableManger.isInHolder = true;
            Debug.Log("Заготовка внутри холдера");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Blankable")
        {
            _InteractableManger.isInHolder = false;
            Debug.Log("Заготовка снаружи холдера");
        }
    }
}
