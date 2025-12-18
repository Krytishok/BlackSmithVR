using UnityEngine;
using System.Collections;

public class BlankTrigger : MonoBehaviour
{
    [SerializeField] GameObject _swordMesh;
    [SerializeField] float _heatingDuration = 3f;    // ����� ������� �� ��������
    [SerializeField] float _overheatDuration = 2f;   // ����� ��������� �� ������
    [SerializeField] float _coolingDuration = 60f;   // ������������ ����� ������� ���������

    [Header("FX settings")]
    [SerializeField] ParticleSystem _fire;
    [SerializeField] ParticleSystem _steam;

    [Header("Glow Settings")]
    [SerializeField] float _maxGlowIntensity = 3f;
    [SerializeField] float _overheatGlowIntensity = 5f;

    private MeshRenderer _swordMeshRenderer;
    private Coroutine _heatingCoroutine;
    private Coroutine _coolingCoroutine;
    private Material _swordMaterial;
    private Light _swordGlow;

    // ������� ��������� ������� (0-1: ������ �� ��������, 1-2: �������� �� ������)
    public float _currentHeatLevel = 0f;
    private bool _isHeating = false;

    // ����� ��� ��������� ������ �������
    private readonly Color _coldMetalColor = new Color(0.5f, 0.5f, 0.5f); // ����� ������
    private readonly Color _redHotColor = new Color(1f, 0.3f, 0.1f);     // �������
    private readonly Color _yellowHotColor = new Color(1f, 0.8f, 0.2f);  // ������
    private readonly Color _whiteHotColor = new Color(0.8f, 0.8f, 0.8f); // �����

    public event System.Action OnHeating;
    public event System.Action OnCooling;

    private void Start()
    {
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();
        _swordMaterial = _swordMeshRenderer.material;

        // ������� �������� ����� ��� ��������
        CreateGlowLight();

        // ������������� �������� �������� ����
        _swordMaterial.color = _coldMetalColor;

        // �������������� ������� ������
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.enabled = false;
        }
    }

    private void CreateGlowLight()
    {
        _swordGlow = _swordMesh.AddComponent<Light>();
        _swordGlow.type = LightType.Point;
        _swordGlow.range = 2f;
        _swordGlow.intensity = 0f;
        _swordGlow.color = Color.red;
        _swordGlow.enabled = false;
    }

    public void StartHeating()
    {
        // ���� ��� �����������, ������ �� ������
        if (_isHeating) return;

        // ������������� ����������, ���� ��� �������
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }

        _isHeating = true;

        // ��������� �������� ������� � �������� ������
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
        }

        _heatingCoroutine = StartCoroutine(HeatSwordCoroutine());
        Debug.Log($"������ ������� ���� � ������: {_currentHeatLevel}");
    }

    public void StartCooling()
    {
        if (!_isHeating && _currentHeatLevel <= 0f) return;

        _isHeating = false;

        // ������������� ������, ���� �� �������
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        // ������������ ����������� ����� ���������� �� ������ �������� ������ �������
        float actualCoolingTime = CalculateActualCoolingTime(_currentHeatLevel);

        // ��������� �������� ����������
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
        }

        _coolingCoroutine = StartCoroutine(CoolSwordCoroutine(actualCoolingTime));
        Debug.Log($"������ ���������� ����. ������� �������: {_currentHeatLevel}, ����� ����������: {actualCoolingTime:F1} ������");
    }

    private IEnumerator HeatSwordCoroutine()
    {
        _swordGlow.enabled = true;

        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.enabled = true;
            _fire.Play();
        }

        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;
        float targetHeatLevel = 2f; // ������������ ������� ������� (�����)

        // ��������� ���������� ����� ������� ������ �� �������� ������
        float remainingHeatingTime = CalculateRemainingHeatingTime(startHeatLevel);

        while (_isHeating && _currentHeatLevel < targetHeatLevel)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / remainingHeatingTime;

            // ��������� ������� ������� �������
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, targetHeatLevel, progress);

            // ��������� ���������� ������� based on current heat level
            UpdateVisualEffects(_currentHeatLevel);

            yield return null;
        }

        // ���� ��� ��� ����������� � �������� ���������
        if (_isHeating)
        {
            _currentHeatLevel = targetHeatLevel;
            UpdateVisualEffects(_currentHeatLevel);
            Debug.Log($"��� �������� �� ������ �������!");
        }
    }

    private float CalculateRemainingHeatingTime(float currentHeatLevel)
    {
        if (currentHeatLevel < 1f)
        {
            // �� ������ ������� �� ��������
            float remainingProgress = 1f - currentHeatLevel;
            return remainingProgress * _heatingDuration;
        }
        else
        {
            // �� ������ ��������� �� ������
            float remainingProgress = 2f - currentHeatLevel;
            return remainingProgress * _overheatDuration;
        }
    }

    /// <summary>
    /// ������������ ����������� ����� ���������� �� ������ �������� ������ �������
    /// </summary>
    private float CalculateActualCoolingTime(float currentHeatLevel)
    {
        // �������� �����������: ����� ���������� = ������������ ����� * (������� ������� / 2)
        // 2 - ������������ ������� �������
        float coolingMultiplier = currentHeatLevel / 2f;
        return _coolingDuration * coolingMultiplier;
    }

    private void UpdateVisualEffects(float heatLevel)
    {
        // ���������� ����� ���������
        if (heatLevel <= 1f)
        {
            // �� ������ � ��������
            _swordMaterial.color = Color.Lerp(_coldMetalColor, _redHotColor, heatLevel);
            _swordGlow.color = Color.Lerp(Color.red, Color.red, heatLevel);
            _swordGlow.intensity = Mathf.Lerp(0f, _maxGlowIntensity, heatLevel);
        }
        else if (heatLevel <= 1.5f)
        {
            // �� �������� � �������
            float subProgress = (heatLevel - 1f) * 2f;
            _swordMaterial.color = Color.Lerp(_redHotColor, _yellowHotColor, subProgress);
            _swordGlow.color = Color.Lerp(Color.red, Color.yellow, subProgress);
            _swordGlow.intensity = _maxGlowIntensity;
        }
        else
        {
            // �� ������� � ������
            float subProgress = (heatLevel - 1.5f) * 2f;
            _swordMaterial.color = Color.Lerp(_yellowHotColor, _whiteHotColor, subProgress);
            _swordGlow.color = Color.Lerp(Color.yellow, Color.white, subProgress);
            _swordGlow.intensity = Mathf.Lerp(_maxGlowIntensity, _overheatGlowIntensity, subProgress);
        }

        // ���������� ������� ������
        UpdateParticleSystem(heatLevel);
    }

    private void UpdateParticleSystem(float heatLevel)
    {
        if (_fire == null) return;

        var main = _fire.main;
        var emission = _fire.emission;

        // ��������� ����� ������ � ����������� �� �����������
        if (heatLevel <= 1f)
        {
            // ��������� ������� ��� ���������� �������
            main.startColor = new Color(1f, 0.5f, 0.2f, 0.8f);
            emission.rateOverTime = Mathf.Lerp(0f, 20f, heatLevel);
        }
        else if (heatLevel <= 1.5f)
        {
            // ������ ������� ��� ������� �������
            main.startColor = new Color(1f, 0.8f, 0.3f, 0.9f);
            emission.rateOverTime = Mathf.Lerp(20f, 35f, (heatLevel - 1f) * 2f);
        }
        else
        {
            // ����� ������� ��� ���������
            main.startColor = new Color(1f, 1f, 1f, 1f);
            emission.rateOverTime = Mathf.Lerp(35f, 50f, (heatLevel - 1.5f) * 2f);
        }

        // ��������� ������� ������
        main.startSize = Mathf.Lerp(0.1f, 0.3f, heatLevel / 2f);
    }

    private IEnumerator CoolSwordCoroutine(float actualCoolingTime)
    {
        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;

        // ��������� ������� ������ ��� ����������
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.rateOverTime = 0f; // ���������� ��������� �������
        }

        while (elapsedTime < actualCoolingTime && _currentHeatLevel > 0f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / actualCoolingTime;

            // ������� ���������� ������ �������
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, 0f, progress);

            // ��������� ���������� �������
            UpdateVisualEffects(_currentHeatLevel);

            yield return null;
        }

        // ��������� ��������� ����������
        _currentHeatLevel = 0f;
        UpdateVisualEffects(0f);

        if (_fire != null)
        {
            _fire.Stop();
            var emission = _fire.emission;
            emission.enabled = false;
        }

        _swordGlow.enabled = false;
        _coolingCoroutine = null;
        Debug.Log($"��� ��������� ����� �� {actualCoolingTime:F1} ������");
    }

    // ����� ��� ��������������� ���������� (����� ������� �����)
    public void ForceCool()
    {
        _isHeating = false;

        // ������������� ��� ��������
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
        }

        // ������������ ����� ���������� � ���������
        float actualCoolingTime = CalculateActualCoolingTime(_currentHeatLevel);
        _coolingCoroutine = StartCoroutine(CoolSwordCoroutine(actualCoolingTime));
    }

    // ����� ��� �������� ������� �����������
    public float GetHeatLevel()
    {
        return _currentHeatLevel;
    }

    // ����� ��� ��������� ������ ������� ����� (��������, ��� �������� ���������)
    public void SetHeatLevel(float level)
    {
        _currentHeatLevel = Mathf.Clamp(level, 0f, 2f);
        UpdateVisualEffects(_currentHeatLevel);

        if (_currentHeatLevel > 0f)
        {
            _swordGlow.enabled = true;
        }
        else
        {
            _swordGlow.enabled = false;
        }
    }

    /// <summary>
    /// ������������ ���������� ����� ������� ��������� ��� ������� ������ �������
    /// </summary>
    public float GetRemainingCoolingTime()
    {
        return CalculateActualCoolingTime(_currentHeatLevel);
    }

    /// <summary>
    /// �������� ������� MeshRenderer �� ����� � ��������� ������� ������� �������
    /// </summary>
    public void ChangeMeshRenderer(GameObject newMeshObject)
    {
        if (newMeshObject == null)
        {
            Debug.LogError("����� ��� �� ����� ���� null!");
            return;
        }

        // �������� ����� MeshRenderer
        MeshRenderer newMeshRenderer = newMeshObject.GetComponent<MeshRenderer>();
        if (newMeshRenderer == null)
        {
            Debug.LogError("����� ������ �� �������� MeshRenderer!");
            return;
        }

        // ��������� ������� ���������
        float currentHeatLevel = _currentHeatLevel;
        bool wasHeating = _isHeating;

        // ������������� ������� ��������
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }

        // ��������� ������
        _swordMesh = newMeshObject;
        _swordMeshRenderer = newMeshRenderer;
        _swordMaterial = _swordMeshRenderer.material;


        // ��������� ������� ������� ������� � ������ ���������
        SetHeatLevel(currentHeatLevel);

        // ���� ��� ������� �������, ������������ ���
        if (wasHeating)
        {
            StartHeating();
        }
        else if (currentHeatLevel > 0f)
        {
            // ���� ��� �������, �� �� ����������� � ������ ������, ��������� ����������
            StartCooling();
        }

        Debug.Log($"MeshRenderer �������. ������� ������� �������: {currentHeatLevel}");
    }

    /// <summary>
    /// ������������� ������ ������ ��� ����� ������ ���������
    /// </summary>
    public void ChangeMaterial(Material newMaterial)
    {
        if (newMaterial == null)
        {
            Debug.LogError("����� �������� �� ����� ���� null!");
            return;
        }

        // ��������� ������� ���������
        float currentHeatLevel = _currentHeatLevel;

        // ��������� ��������
        _swordMaterial = newMaterial;
        _swordMeshRenderer.material = _swordMaterial;

        // ��������� ������� ������� ������� � ������ ���������
        SetHeatLevel(currentHeatLevel);

        Debug.Log($"�������� �������. ������� ������� �������: {currentHeatLevel}");
    }

    /// <summary>
    /// ��������� �������� ��� �� ����� GameObject � ����������� ���������� � �������
    /// </summary>
    public void ReplaceSword(GameObject newSwordPrefab)
    {
        if (newSwordPrefab == null)
        {
            Debug.LogError("������ ������ ���� �� ����� ���� null!");
            return;
        }

        // ��������� ������� ��������� � �������������
        float currentHeatLevel = _currentHeatLevel;
        bool wasHeating = _isHeating;


        // ������������� ������� ��������
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }


        // ��������� ��������� � ������ ����
        ChangeMeshRenderer(newSwordPrefab);

        // ���� ��� ������� �������, ������������ ���
        if (wasHeating)
        {
            StartHeating();
        }

        Debug.Log($"��� ��������� �������. ������� ������� �������: {currentHeatLevel}");
    }

    /// <summary>
    /// ��������� ������ �� ������� ��� (��� ����������� �������)
    /// </summary>
    public void UpdateMeshReference(GameObject newMeshObject)
    {
        if (newMeshObject == null)
        {
            Debug.LogError("����� ��� �� ����� ���� null!");
            return;
        }

        // ��������� ������� ���������
        float currentHeatLevel = _currentHeatLevel;

        // ��������� ������
        _swordMesh = newMeshObject;
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();

        // ���� � ������ ���� ��� ���������, ������� �����
        if (_swordMeshRenderer.material == null)
        {
            _swordMaterial = new Material(Shader.Find("Standard"));
            _swordMeshRenderer.material = _swordMaterial;
        }
        else
        {
            _swordMaterial = _swordMeshRenderer.material;
        }

        // ��������� ������� ������� �������
        SetHeatLevel(currentHeatLevel);

        Debug.Log($"������ �� ��� ���������. ������� ������� �������: {currentHeatLevel}");
    }


    /// <summary>
    /// �������� ��������� �� ���������� �������
    /// </summary>
    /// <param name="targetCoolingTime">������� ����� ���������� � �������� (�� ��������� 2)</param>
    /// <param name="useSmoothTransition">������������ ������� ������� (true) ��� �������� (false)</param>
    public void AccelerateCooling(float targetCoolingTime = 2f, bool useSmoothTransition = true)
    {
        // ������������� ������� ����������, ���� ��� �������
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }

        // ���� ���� ������, ������������� ���
        if (_isHeating)
        {
            _isHeating = false;
            if (_heatingCoroutine != null)
            {
                StopCoroutine(_heatingCoroutine);
                _heatingCoroutine = null;
            }
        }

        // ��������� ������������ �������
        if (targetCoolingTime <= 0f)
        {
            Debug.LogWarning($"������������ ����� ����������: {targetCoolingTime}. ������������ ����������� ��������: 0.1f");
            targetCoolingTime = 0.1f;
        }

        if(_currentHeatLevel <= 0.1f)
        {
            Debug.Log("������ �� ����������, ������ ��� ��������� ��� ������");
            return;
        }

        // ��������� ���������� ����������
        _steam.Play();
        _coolingCoroutine = StartCoroutine(AcceleratedCoolingCoroutine(targetCoolingTime, useSmoothTransition));
        Debug.Log($"�������� ���������� ���������� �� {targetCoolingTime} ������. ������� ������� �������: {_currentHeatLevel}");
    }

    private IEnumerator AcceleratedCoolingCoroutine(float targetCoolingTime, bool useSmoothTransition)
    {
        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;

        // ��������� ������� ������ ��� ����������
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.rateOverTime = 0f; // ������ ��������� �������
        }

        while (elapsedTime < targetCoolingTime && _currentHeatLevel > 0f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / targetCoolingTime;

            float easedProgress;
            if (useSmoothTransition)
            {
                // ���������� ������������ ������������ ��� ����� �������� ���������� � ������
                easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            }
            else
            {
                // �������� ����������
                easedProgress = progress;
            }

            // ������� ���������� ������ �������
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, 0f, easedProgress);

            // ��������� ���������� �������
            UpdateVisualEffects(_currentHeatLevel);

            // ����������� ��������� ������� � ����������� �� ���������
            if (_fire != null)
            {
                var emission = _fire.emission;
                float particleProgress = 1f - Mathf.Clamp01(elapsedTime / (targetCoolingTime * 0.5f));
                emission.rateOverTime = Mathf.Lerp(0f, 20f, particleProgress);
            }

            yield return null;
        }

        // ��������� ��������� ����������
        _currentHeatLevel = 0f;
        UpdateVisualEffects(0f);

        if (_fire != null)
        {
            _fire.Stop();
            var emission = _fire.emission;
            emission.enabled = false;
        }

        _swordGlow.enabled = false;
        _coolingCoroutine = null;
        Debug.Log($"��������� ��������� ������ �� {targetCoolingTime} ������ (���������� ����������)");
    }
}