using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))] 
public class WorkpieceController : MonoBehaviour
{
    [Header("Spawn / Hits")]
    public GameObject markerPrefab;    
    [Range(1, 100)] public int requiredHits = 12;
    public int maxSimultaneousMarkers = 1; 
    public float minDistanceBetweenMarkers = 0.15f;

    [Header("Anvil attachment")]
    public Transform anvilTransform;         
    [Header("Spawn sampling")]
    public int maxSpawnAttempts = 50;        
    public float spawnHeightOffset = 0.02f;  
    private int currentHits = 0;
    private List<GameObject> activeMarkers = new List<GameObject>();
    private MeshCollider meshCollider; 
    private Bounds localBounds;

    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogWarning("WorkpieceController: MeshCollider не найден. Рекомендуется добавить MeshCollider для точного спавна по поверхности.");
        }
    }

    void Start()
    {
        if (anvilTransform != null)
        {
            transform.SetParent(anvilTransform, worldPositionStays: false);

        }

        if (markerPrefab == null)
        {
            Debug.LogError("WorkpieceController: markerPrefab не назначен!");
            return;
        }


        var mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            localBounds = mf.sharedMesh.bounds;
        }
        else
        {

            var rend = GetComponent<Renderer>();
            if (rend != null)
            {

                localBounds = new Bounds(transform.InverseTransformPoint(rend.bounds.center), rend.bounds.size);
            }
            else
            {
                localBounds = new Bounds(Vector3.zero, Vector3.one);
            }
        }

        for (int i = 0; i < maxSimultaneousMarkers; i++)
            SpawnMarker();
    }


    public void SpawnMarker()
    {
        if (markerPrefab == null) return;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {

            Vector3 rndLocal = new Vector3(
                Random.Range(localBounds.min.x, localBounds.max.x),
                Random.Range(localBounds.min.y, localBounds.max.y),
                Random.Range(localBounds.min.z, localBounds.max.z)
            );


            Vector3 worldTop = transform.TransformPoint(new Vector3(rndLocal.x, localBounds.max.y + 0.5f, rndLocal.z));
            Ray ray = new Ray(worldTop, Vector3.down);
            RaycastHit hit;
            bool gotHit = false;


            if (meshCollider != null)
            {
                if (meshCollider.Raycast(ray, out hit, 2.0f))
                    gotHit = true;
            }
            else
            {

                if (Physics.Raycast(ray, out hit, 2.0f))
                {

                    if (hit.collider != null && hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform)
                        gotHit = true;
                    else
                    {
                       
                        gotHit = false;
                    }
                }
            }

            if (!gotHit) continue;

            Vector3 spawnPos = hit.point + hit.normal * spawnHeightOffset;

            Vector3 spawnLocalOnWorkpiece = transform.InverseTransformPoint(spawnPos);
            bool ok = true;
            foreach (var m in activeMarkers)
            {
                if (m == null) continue;
                float d = Vector3.Distance(m.transform.localPosition, spawnLocalOnWorkpiece);
                if (d < minDistanceBetweenMarkers)
                {
                    ok = false;
                    break;
                }
            }
            if (!ok) continue;

            GameObject marker = Instantiate(markerPrefab, spawnPos, Quaternion.LookRotation(hit.normal), transform);

            marker.transform.position = spawnPos;
            activeMarkers.Add(marker);
            return;
        }

        Vector3 centerTop = transform.TransformPoint(localBounds.center + new Vector3(0, localBounds.extents.y, 0));
        GameObject fallback = Instantiate(markerPrefab, centerTop + Vector3.up * spawnHeightOffset, Quaternion.identity, transform);
        activeMarkers.Add(fallback);
    }

  
    public void NotifyMarkerHit(GameObject marker)
    {
        if (activeMarkers.Contains(marker)) activeMarkers.Remove(marker);
        currentHits++;
        Debug.Log($"Удар по заготовке: {currentHits}/{requiredHits}");

        if (currentHits >= requiredHits)
        {
            Debug.Log("Заготовка готова!");
           
            foreach (var m in activeMarkers) if (m != null) Destroy(m);
            activeMarkers.Clear();
           
            enabled = false;
            return;
        }

    
        if (activeMarkers.Count < maxSimultaneousMarkers)
            SpawnMarker();
    }
}
