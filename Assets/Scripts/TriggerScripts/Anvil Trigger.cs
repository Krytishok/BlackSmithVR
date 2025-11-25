using UnityEngine;
using System.Collections;

public class AnvilTrigger : MonoBehaviour
{
    [SerializeField] private Collider swordCollider;
    [SerializeField] private GameObject marker;
    [SerializeField] private float markerHeightOffset = 0.1f; // Высота маркера над поверхностью

    private void Start()
    {
        // Проверяем наличие необходимых компонентов
        if (swordCollider == null)
        {
            Debug.LogError("Sword Collider is not assigned in AnvilManager!");
        }

        if (marker == null)
        {
            Debug.LogError("Marker is not assigned in AnvilManager!");
        }

        // Изначально скрываем маркер
        if (marker != null)
            marker.SetActive(false);
    }

    /// <summary>
    /// Телепортирует маркер на случайную позицию на грани коллайдера меча
    /// </summary>
    public void TeleportMarkerToRandomEdge()
    {
        if (swordCollider == null || marker == null)
        {
            Debug.LogError("Sword Collider or Marker is not assigned!");
            return;
        }

        Vector3 randomEdgePoint = GetRandomPointOnColliderEdge();
        PlaceMarker(randomEdgePoint);
    }

    /// <summary>
    /// Получает случайную точку на грани коллайдера
    /// </summary>
    private Vector3 GetRandomPointOnColliderEdge()
    {
        Bounds bounds = swordCollider.bounds;

        // Выбираем случайную грань коллайдера (0-5 для 6 граней бокса)
        int randomFace = Random.Range(0, 6);

        Vector3 randomPoint = Vector3.zero;

        switch (randomFace)
        {
            case 0: // Верхняя грань (Y max)
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.max.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                break;

            case 1: // Нижняя грань (Y min)
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.min.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                break;

            case 2: // Правая грань (X max)
                randomPoint = new Vector3(
                    bounds.max.x,
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                break;

            case 3: // Левая грань (X min)
                randomPoint = new Vector3(
                    bounds.min.x,
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                break;

            case 4: // Передняя грань (Z max)
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    bounds.max.z
                );
                break;

            case 5: // Задняя грань (Z min)
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    bounds.min.z
                );
                break;
        }

        return randomPoint;
    }

    /// <summary>
    /// Размещает маркер в указанной позиции с учетом нормали поверхности
    /// </summary>
    private void PlaceMarker(Vector3 position)
    {
        // Активируем маркер если он был скрыт
        if (!marker.activeSelf)
            marker.SetActive(true);

        // Вычисляем нормаль поверхности для правильного размещения маркера
        Vector3 surfaceNormal = GetSurfaceNormal(position);

        // Позиционируем маркер
        marker.transform.position = position + surfaceNormal * markerHeightOffset;

        // Ориентируем маркер перпендикулярно поверхности
        marker.transform.up = surfaceNormal;
    }

    /// <summary>
    /// Получает нормаль поверхности в указанной точке
    /// </summary>
    private Vector3 GetSurfaceNormal(Vector3 position)
    {
        // Используем Raycast для определения нормали поверхности
        RaycastHit hit;
        Vector3 rayDirection = Vector3.zero;

        // Определяем направление луча в зависимости от позиции относительно центра коллайдера
        Vector3 center = swordCollider.bounds.center;

        if (Mathf.Abs(position.x - center.x) > Mathf.Abs(position.y - center.y) &&
            Mathf.Abs(position.x - center.x) > Mathf.Abs(position.z - center.z))
        {
            rayDirection = position.x > center.x ? Vector3.left : Vector3.right;
        }
        else if (Mathf.Abs(position.y - center.y) > Mathf.Abs(position.z - center.z))
        {
            rayDirection = position.y > center.y ? Vector3.down : Vector3.up;
        }
        else
        {
            rayDirection = position.z > center.z ? Vector3.back : Vector3.forward;
        }

        // Пускаем луч для определения нормали
        if (Physics.Raycast(position + rayDirection * 0.1f, -rayDirection, out hit, 0.2f))
        {
            return hit.normal;
        }

        // Если Raycast не сработал, возвращаем приблизительную нормаль
        return Vector3.up;
    }

    /// <summary>
    /// Для отладки: визуализация границ коллайдера в редакторе
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (swordCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(swordCollider.bounds.center, swordCollider.bounds.size);
        }
    }
}