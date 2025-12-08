using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    public event System.Action OnTriggerActivated;

    [SerializeField] private string triggerTag = "Hammer";


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            OnTriggerActivated?.Invoke();
        }
    }

    public void TeleportTo(Vector3 _position)
    {
        gameObject.transform.localPosition = _position;
        Debug.Log($"Объект Marker телепортирован в точку: {_position}");
    }
}