using UnityEngine;

public class AnvilTrigger : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private MarkerScript markerObject; // Объект с маркером

    [SerializeField] private GameObject[] _meshes;
    private int _currentIndexOfMesh = 0;


    private int _hitCounter = 0;
    private int _possibleHits = 12;
    private int _meshLevel = 0;

    public bool _isWorkDone = false;


    public event System.Action OnMeshChanged;

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

        ValidateMeshLevel();

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

    public GameObject ShowMeshByIndex(int index = 1000)
    {
        if (markerObject != null && index < _meshes.Length)
        {
            for(int i = 0;  i < _meshes.Length; i++)
            {
                _meshes[i].SetActive(false);
            }
            _meshes[index].gameObject.SetActive(true);
            _currentIndexOfMesh = index;
            return _meshes[index].gameObject;
        }
        else
        {

            Debug.Log("Меш не изменился, так как index превышает кол-во мешей или не был определен при вызове функции");
            return _meshes[_currentIndexOfMesh];
        }
    }

    private void ValidateMeshLevel()
    {
        if(_hitCounter % 3 == 0 && _hitCounter < _possibleHits)
        {

            _meshLevel++;
            ShowMeshByIndex(_meshLevel);

            OnMeshChanged?.Invoke();
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