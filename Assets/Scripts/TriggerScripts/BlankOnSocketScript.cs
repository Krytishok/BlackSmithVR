using UnityEngine;

public class BlankOnSocketScript : MonoBehaviour
{
    [SerializeField] string _tag;

    public GameObject _gameObject;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_tag))
        {
            _gameObject = other.gameObject;
        }
    }
}
