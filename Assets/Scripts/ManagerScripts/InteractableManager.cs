using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    [SerializeField] GameObject _swordBlank;
    [SerializeField] Transform _swordTransform;
    private bool _isInHolder; //Маркер наличия заготовки в холдере
    public bool isInHolder {  get { return _isInHolder; } set { _isInHolder = value; } }


    public void SpawBlank()
    {
        Debug.Log("Спавн");
        Instantiate(_swordBlank, _swordTransform.transform);
    }


}
