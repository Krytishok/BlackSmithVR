using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public class MarkerScript : MonoBehaviour
{
    public event System.Action OnTriggerActivated;

    [SerializeField] private string triggerTag = "Hammer";
    [SerializeField] ParticleSystem _blink;
    [SerializeField] GameObject _audioManager;
    [SerializeField] AudioSource[] _sounds;

    private Vector3 _posForBlink;

    System.Random random = new System.Random();




    private void Start()
    {
        if(_sounds == null)
        {
            _sounds = _audioManager.GetComponentsInChildren<AudioSource>(false);
        }
       
    }


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
        PlaySound();

    }

    private void PlaySound()
    {
        if (_sounds == null)
        {
            Debug.Log("Звук удара не инициализирован!");
            return;
        }
        _sounds[random.Next(_sounds.Length)].Play();
        Debug.Log($"Звук удара под номером {_sounds.Length}");
    }
}