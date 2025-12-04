using UnityEngine;

public class AnvilTrigger : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private MarkerScript markerObject; // Объект с маркером


    private int _hitCounter = 0;
    private int _possibleHits = 5;

    public bool _isWorkDone = false;

    private void Start()
    {
        if (markerObject != null)
        {
            markerObject.OnTriggerActivated += TeleportMarkerObject;
            markerObject.gameObject.SetActive(false);

            
        }
    }

    private void TeleportMarkerObject()
    {
        if (markerObject == null || _hitCounter >= _possibleHits) return;


        // Получаем позицию объекта с маркером
        Vector3 currentPos = markerObject.gameObject.transform.localPosition;

        // Случайная позиция по X
        float randomX = Random.Range(startPos.localPosition.x, endPos.localPosition.x);

        // Телепортируем объект с маркером
        markerObject.TeleportTo(new Vector3(randomX, currentPos.y, currentPos.z));

        _hitCounter++;
        Debug.Log($"Ударов сделано: {_hitCounter} Ударов осталось: {_possibleHits - _hitCounter}");
        if(_hitCounter >= _possibleHits)
        {
            _isWorkDone = true;
            markerObject.gameObject.SetActive(false);
        }

    }

    public void SetActiveMarker()
    {
        if(!_isWorkDone)
        {
            _hitCounter = 0;
            markerObject.gameObject.SetActive(true);
        }
        
    }

    private void OnDestroy()
    {
        if (markerObject != null)
        {
            markerObject.OnTriggerActivated -= TeleportMarkerObject;
        }
    }
}