// PlayAreaBounds.cs
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayAreaBounds : MonoBehaviour
{
    private BoxCollider box;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    // Возвращает мировые bounds игрового поля
    public Bounds GetWorldBounds()
    {
        return box.bounds;
    }

    // Визуализация в редакторе (необязательно, но удобно)
    void OnDrawGizmosSelected()
    {
        var col = GetComponent<BoxCollider>();
        if (col != null)
        {
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.15f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.6f);
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
