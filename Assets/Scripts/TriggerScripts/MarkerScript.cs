using UnityEngine;

public class MarkerScript : MonoBehaviour
{
    public event System.Action OnTriggerActivated;

    [SerializeField] private string triggerTag = "Hammer";
    [SerializeField] ParticleSystem _blink;

    private Vector3 _posForBlink;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            _posForBlink = gameObject.transform.position;
            OnTriggerActivated?.Invoke();
        }
    }

    public void TeleportTo(Vector3 _position)
    {
        gameObject.transform.localPosition = _position;
        Debug.Log($"Объект Marker телепортирован в точку: {_position}");
        _blink.transform.position = _posForBlink;
        _blink.Play();
    }
}