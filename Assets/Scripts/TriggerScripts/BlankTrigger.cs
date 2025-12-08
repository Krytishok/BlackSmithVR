using UnityEngine;
using System.Collections;

public class BlankTrigger : MonoBehaviour
{
    [SerializeField] GameObject _swordMesh;
    [SerializeField] float _heatingDuration = 3f;    // Время нагрева до красного
    [SerializeField] float _overheatDuration = 2f;   // Время перегрева до белого
    [SerializeField] float _coolingDuration = 60f;   // МАКСИМАЛЬНОЕ время полного остывания

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

    // Текущее состояние нагрева (0-1: нагрев до красного, 1-2: перегрев до белого)
    public float _currentHeatLevel = 0f;
    private bool _isHeating = false;

    // Цвета для различных стадий нагрева
    private readonly Color _coldMetalColor = new Color(0.5f, 0.5f, 0.5f); // Серый металл
    private readonly Color _redHotColor = new Color(1f, 0.3f, 0.1f);     // Красный
    private readonly Color _yellowHotColor = new Color(1f, 0.8f, 0.2f);  // Желтый
    private readonly Color _whiteHotColor = new Color(0.8f, 0.8f, 0.8f); // Белый

    public event System.Action OnHeating;
    public event System.Action OnCooling;

    private void Start()
    {
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();
        _swordMaterial = _swordMeshRenderer.material;

        // Создаем источник света для свечения
        CreateGlowLight();

        // Устанавливаем исходный холодный цвет
        _swordMaterial.color = _coldMetalColor;

        // Инициализируем систему частиц
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
        // Если уже нагревается, ничего не делаем
        if (_isHeating) return;

        // Останавливаем охлаждение, если оно активно
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }

        _isHeating = true;

        // Запускаем корутину нагрева с текущего уровня
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
        }

        _heatingCoroutine = StartCoroutine(HeatSwordCoroutine());
        Debug.Log($"Начало нагрева меча с уровня: {_currentHeatLevel}");
    }

    public void StartCooling()
    {
        if (!_isHeating && _currentHeatLevel <= 0f) return;

        _isHeating = false;

        // Останавливаем нагрев, если он активен
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        // Рассчитываем фактическое время охлаждения на основе текущего уровня нагрева
        float actualCoolingTime = CalculateActualCoolingTime(_currentHeatLevel);

        // Запускаем корутину охлаждения
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
        }

        _coolingCoroutine = StartCoroutine(CoolSwordCoroutine(actualCoolingTime));
        Debug.Log($"Начало охлаждения меча. Уровень нагрева: {_currentHeatLevel}, время охлаждения: {actualCoolingTime:F1} секунд");
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
        float targetHeatLevel = 2f; // Максимальный уровень нагрева (белый)

        // Вычисляем оставшееся время нагрева исходя из текущего уровня
        float remainingHeatingTime = CalculateRemainingHeatingTime(startHeatLevel);

        while (_isHeating && _currentHeatLevel < targetHeatLevel)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / remainingHeatingTime;

            // Обновляем текущий уровень нагрева
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, targetHeatLevel, progress);

            // Применяем визуальные эффекты based on current heat level
            UpdateVisualEffects(_currentHeatLevel);

            yield return null;
        }

        // Если все еще нагреваемся и достигли максимума
        if (_isHeating)
        {
            _currentHeatLevel = targetHeatLevel;
            UpdateVisualEffects(_currentHeatLevel);
            Debug.Log($"Меч перегрет до белого каления!");
        }
    }

    private float CalculateRemainingHeatingTime(float currentHeatLevel)
    {
        if (currentHeatLevel < 1f)
        {
            // На стадии нагрева до красного
            float remainingProgress = 1f - currentHeatLevel;
            return remainingProgress * _heatingDuration;
        }
        else
        {
            // На стадии перегрева до белого
            float remainingProgress = 2f - currentHeatLevel;
            return remainingProgress * _overheatDuration;
        }
    }

    /// <summary>
    /// Рассчитывает фактическое время охлаждения на основе текущего уровня нагрева
    /// </summary>
    private float CalculateActualCoolingTime(float currentHeatLevel)
    {
        // Линейная зависимость: время охлаждения = максимальное время * (текущий уровень / 2)
        // 2 - максимальный уровень нагрева
        float coolingMultiplier = currentHeatLevel / 2f;
        return _coolingDuration * coolingMultiplier;
    }

    private void UpdateVisualEffects(float heatLevel)
    {
        // Обновление цвета материала
        if (heatLevel <= 1f)
        {
            // От серого к красному
            _swordMaterial.color = Color.Lerp(_coldMetalColor, _redHotColor, heatLevel);
            _swordGlow.color = Color.Lerp(Color.red, Color.red, heatLevel);
            _swordGlow.intensity = Mathf.Lerp(0f, _maxGlowIntensity, heatLevel);
        }
        else if (heatLevel <= 1.5f)
        {
            // От красного к желтому
            float subProgress = (heatLevel - 1f) * 2f;
            _swordMaterial.color = Color.Lerp(_redHotColor, _yellowHotColor, subProgress);
            _swordGlow.color = Color.Lerp(Color.red, Color.yellow, subProgress);
            _swordGlow.intensity = _maxGlowIntensity;
        }
        else
        {
            // От желтого к белому
            float subProgress = (heatLevel - 1.5f) * 2f;
            _swordMaterial.color = Color.Lerp(_yellowHotColor, _whiteHotColor, subProgress);
            _swordGlow.color = Color.Lerp(Color.yellow, Color.white, subProgress);
            _swordGlow.intensity = Mathf.Lerp(_maxGlowIntensity, _overheatGlowIntensity, subProgress);
        }

        // Обновление системы частиц
        UpdateParticleSystem(heatLevel);
    }

    private void UpdateParticleSystem(float heatLevel)
    {
        if (_fire == null) return;

        var main = _fire.main;
        var emission = _fire.emission;

        // Изменение цвета частиц в зависимости от температуры
        if (heatLevel <= 1f)
        {
            // Оранжевые частицы при нормальном нагреве
            main.startColor = new Color(1f, 0.5f, 0.2f, 0.8f);
            emission.rateOverTime = Mathf.Lerp(0f, 20f, heatLevel);
        }
        else if (heatLevel <= 1.5f)
        {
            // Желтые частицы при сильном нагреве
            main.startColor = new Color(1f, 0.8f, 0.3f, 0.9f);
            emission.rateOverTime = Mathf.Lerp(20f, 35f, (heatLevel - 1f) * 2f);
        }
        else
        {
            // Белые частицы при перегреве
            main.startColor = new Color(1f, 1f, 1f, 1f);
            emission.rateOverTime = Mathf.Lerp(35f, 50f, (heatLevel - 1.5f) * 2f);
        }

        // Изменение размера частиц
        main.startSize = Mathf.Lerp(0.1f, 0.3f, heatLevel / 2f);
    }

    private IEnumerator CoolSwordCoroutine(float actualCoolingTime)
    {
        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;

        // Обновляем систему частиц для охлаждения
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.rateOverTime = 0f; // Постепенно уменьшаем частицы
        }

        while (elapsedTime < actualCoolingTime && _currentHeatLevel > 0f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / actualCoolingTime;

            // Плавное уменьшение уровня нагрева
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, 0f, progress);

            // Применяем визуальные эффекты
            UpdateVisualEffects(_currentHeatLevel);

            yield return null;
        }

        // Финальное состояние охлаждения
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
        Debug.Log($"Меч полностью остыл за {actualCoolingTime:F1} секунд");
    }

    // Метод для принудительного охлаждения (можно вызвать извне)
    public void ForceCool()
    {
        _isHeating = false;

        // Останавливаем все корутины
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
            _heatingCoroutine = null;
        }

        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
        }

        // Рассчитываем время охлаждения и запускаем
        float actualCoolingTime = CalculateActualCoolingTime(_currentHeatLevel);
        _coolingCoroutine = StartCoroutine(CoolSwordCoroutine(actualCoolingTime));
    }

    // Метод для проверки текущей температуры
    public float GetHeatLevel()
    {
        return _currentHeatLevel;
    }

    // Метод для установки уровня нагрева извне (например, при загрузке состояния)
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
    /// Рассчитывает оставшееся время полного остывания при текущем уровне нагрева
    /// </summary>
    public float GetRemainingCoolingTime()
    {
        return CalculateActualCoolingTime(_currentHeatLevel);
    }

    /// <summary>
    /// Заменяет текущий MeshRenderer на новый и применяет текущий уровень нагрева
    /// </summary>
    public void ChangeMeshRenderer(GameObject newMeshObject)
    {
        if (newMeshObject == null)
        {
            Debug.LogError("Новый меш не может быть null!");
            return;
        }

        // Получаем новый MeshRenderer
        MeshRenderer newMeshRenderer = newMeshObject.GetComponent<MeshRenderer>();
        if (newMeshRenderer == null)
        {
            Debug.LogError("Новый объект не содержит MeshRenderer!");
            return;
        }

        // Сохраняем текущее состояние
        float currentHeatLevel = _currentHeatLevel;
        bool wasHeating = _isHeating;

        // Останавливаем текущие корутины
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

        // Обновляем ссылки
        _swordMesh = newMeshObject;
        _swordMeshRenderer = newMeshRenderer;
        _swordMaterial = _swordMeshRenderer.material;


        // Применяем текущий уровень нагрева к новому материалу
        SetHeatLevel(currentHeatLevel);

        // Если был процесс нагрева, возобновляем его
        if (wasHeating)
        {
            StartHeating();
        }
        else if (currentHeatLevel > 0f)
        {
            // Если меш горячий, но не нагревается в данный момент, запускаем охлаждение
            StartCooling();
        }

        Debug.Log($"MeshRenderer заменен. Текущий уровень нагрева: {currentHeatLevel}");
    }

    /// <summary>
    /// Перегруженная версия метода для смены только материала
    /// </summary>
    public void ChangeMaterial(Material newMaterial)
    {
        if (newMaterial == null)
        {
            Debug.LogError("Новый материал не может быть null!");
            return;
        }

        // Сохраняем текущее состояние
        float currentHeatLevel = _currentHeatLevel;

        // Обновляем материал
        _swordMaterial = newMaterial;
        _swordMeshRenderer.material = _swordMaterial;

        // Применяем текущий уровень нагрева к новому материалу
        SetHeatLevel(currentHeatLevel);

        Debug.Log($"Материал заменен. Текущий уровень нагрева: {currentHeatLevel}");
    }

    /// <summary>
    /// Полностью заменяет меч на новый GameObject с сохранением трансформа и нагрева
    /// </summary>
    public void ReplaceSword(GameObject newSwordPrefab)
    {
        if (newSwordPrefab == null)
        {
            Debug.LogError("Префаб нового меча не может быть null!");
            return;
        }

        // Сохраняем текущее состояние и трансформацию
        float currentHeatLevel = _currentHeatLevel;
        bool wasHeating = _isHeating;


        // Останавливаем текущие корутины
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


        // Применяем изменения к новому мешу
        ChangeMeshRenderer(newSwordPrefab);

        // Если был процесс нагрева, возобновляем его
        if (wasHeating)
        {
            StartHeating();
        }

        Debug.Log($"Меш полностью заменен. Текущий уровень нагрева: {currentHeatLevel}");
    }

    /// <summary>
    /// Обновляет ссылку на текущий меш (без уничтожения старого)
    /// </summary>
    public void UpdateMeshReference(GameObject newMeshObject)
    {
        if (newMeshObject == null)
        {
            Debug.LogError("Новый меш не может быть null!");
            return;
        }

        // Сохраняем текущее состояние
        float currentHeatLevel = _currentHeatLevel;

        // Обновляем ссылку
        _swordMesh = newMeshObject;
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();

        // Если у нового меша нет материала, создаем новый
        if (_swordMeshRenderer.material == null)
        {
            _swordMaterial = new Material(Shader.Find("Standard"));
            _swordMeshRenderer.material = _swordMaterial;
        }
        else
        {
            _swordMaterial = _swordMeshRenderer.material;
        }

        // Применяем текущий уровень нагрева
        SetHeatLevel(currentHeatLevel);

        Debug.Log($"Ссылка на меш обновлена. Текущий уровень нагрева: {currentHeatLevel}");
    }


    /// <summary>
    /// Ускоряет остывание до указанного времени
    /// </summary>
    /// <param name="targetCoolingTime">Целевое время охлаждения в секундах (по умолчанию 2)</param>
    /// <param name="useSmoothTransition">Использовать плавный переход (true) или линейный (false)</param>
    public void AccelerateCooling(float targetCoolingTime = 2f, bool useSmoothTransition = true)
    {
        // Останавливаем текущее охлаждение, если оно активно
        if (_coolingCoroutine != null)
        {
            StopCoroutine(_coolingCoroutine);
            _coolingCoroutine = null;
        }

        // Если идет нагрев, останавливаем его
        if (_isHeating)
        {
            _isHeating = false;
            if (_heatingCoroutine != null)
            {
                StopCoroutine(_heatingCoroutine);
                _heatingCoroutine = null;
            }
        }

        // Проверяем корректность времени
        if (targetCoolingTime <= 0f)
        {
            Debug.LogWarning($"Некорректное время охлаждения: {targetCoolingTime}. Используется минимальное значение: 0.1f");
            targetCoolingTime = 0.1f;
        }

        if(_currentHeatLevel <= 0.1f)
        {
            Debug.Log("Ничего не происходит, потому что заготовка уже остыла");
            return;
        }

        // Запускаем ускоренное охлаждение
        _steam.Play();
        _coolingCoroutine = StartCoroutine(AcceleratedCoolingCoroutine(targetCoolingTime, useSmoothTransition));
        Debug.Log($"Запущено ускоренное охлаждение за {targetCoolingTime} секунд. Текущий уровень нагрева: {_currentHeatLevel}");
    }

    private IEnumerator AcceleratedCoolingCoroutine(float targetCoolingTime, bool useSmoothTransition)
    {
        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;

        // Обновляем систему частиц для охлаждения
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.rateOverTime = 0f; // Быстро уменьшаем частицы
        }

        while (elapsedTime < targetCoolingTime && _currentHeatLevel > 0f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / targetCoolingTime;

            float easedProgress;
            if (useSmoothTransition)
            {
                // Используем квадратичную интерполяцию для более быстрого охлаждения в начале
                easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            }
            else
            {
                // Линейное охлаждение
                easedProgress = progress;
            }

            // Плавное уменьшение уровня нагрева
            _currentHeatLevel = Mathf.Lerp(startHeatLevel, 0f, easedProgress);

            // Применяем визуальные эффекты
            UpdateVisualEffects(_currentHeatLevel);

            // Динамически уменьшаем частицы в зависимости от прогресса
            if (_fire != null)
            {
                var emission = _fire.emission;
                float particleProgress = 1f - Mathf.Clamp01(elapsedTime / (targetCoolingTime * 0.5f));
                emission.rateOverTime = Mathf.Lerp(0f, 20f, particleProgress);
            }

            yield return null;
        }

        // Финальное состояние охлаждения
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
        Debug.Log($"Заготовка полностью остыла за {targetCoolingTime} секунд (ускоренное охлаждение)");
    }
}