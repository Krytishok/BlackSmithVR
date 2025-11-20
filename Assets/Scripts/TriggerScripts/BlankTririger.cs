using UnityEngine;
using System.Collections;

public class BlankTririger : MonoBehaviour
{
    [SerializeField] GameObject _swordMesh;
    [SerializeField] Material _material;

    private MeshRenderer _swordMeshRenderer;

    private void Start()
    {
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Forge"))
        {
            _swordMeshRenderer.material.color = Color.red;
            Debug.Log($"Изменение цвета: {_swordMeshRenderer.materials[0].name}");
        }
    }
}
