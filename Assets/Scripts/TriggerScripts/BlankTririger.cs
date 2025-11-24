using UnityEngine;
using System.Collections;

public class BlankTririger : MonoBehaviour
{
    [SerializeField] GameObject _swordMesh;
    [SerializeField] float _heatingDuration = 3f;    // Время нагрева до красного
    [SerializeField] float _overheatDuration = 2f;   // Время перегрева до белого
    [SerializeField] float _coolingDuration = 5f;    // Время остывания

    [Header("FX settings")]
    [SerializeField] ParticleSystem _fire;

    [Header("Glow Settings")]
    [SerializeField] float _maxGlowIntensity = 3f;
    [SerializeField] float _overheatGlowIntensity = 5f;

    private MeshRenderer _swordMeshRenderer;
    private Coroutine _heatingCoroutine;
    private Color _originalColor;
    private Material _swordMaterial;
    private Light _swordGlow;

    // Текущее состояние нагрева (0-1: нагрев до красного, 1-2: перегрев до белого)
    private float _currentHeatLevel = 0f;
    private bool _isHeating = false;

    // Цвета для различных стадий нагрева
    private readonly Color _coldMetalColor = new Color(0.5f, 0.5f, 0.5f); // Серый металл
    private readonly Color _redHotColor = new Color(1f, 0.3f, 0.1f);     // Красный
    private readonly Color _yellowHotColor = new Color(1f, 0.8f, 0.2f);  // Желтый
    private readonly Color _whiteHotColor = new Color(0.8f, 0.8f, 0.8f); // Белый

    private void Start()
    {
        _swordMeshRenderer = _swordMesh.GetComponent<MeshRenderer>();
        _swordMaterial = _swordMeshRenderer.material;
        _originalColor = _swordMaterial.color;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Forge"))
        {
            // Останавливаем предыдущую корутину, если она запущена
            if (_heatingCoroutine != null)
            {
                StopCoroutine(_heatingCoroutine);
            }

            _isHeating = true;

            // Запускаем корутину нагрева с текущего уровня
            _heatingCoroutine = StartCoroutine(HeatSwordCoroutine());
            Debug.Log($"Начало нагрева меча с уровня: {_currentHeatLevel}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Forge"))
        {
            _isHeating = false;

            // При выходе из горна начинаем охлаждение
            if (_heatingCoroutine != null)
            {
                StopCoroutine(_heatingCoroutine);
            }

            _heatingCoroutine = StartCoroutine(CoolSwordCoroutine());
            Debug.Log($"Начало охлаждения меча");
        }
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
        var colorOverLifetime = _fire.colorOverLifetime;

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

    private IEnumerator CoolSwordCoroutine()
    {
        float elapsedTime = 0f;
        float startHeatLevel = _currentHeatLevel;

        // Обновляем систему частиц для охлаждения
        if (_fire != null)
        {
            var emission = _fire.emission;
            emission.rateOverTime = 0f; // Постепенно уменьшаем частицы
        }

        while (elapsedTime < _coolingDuration && _currentHeatLevel > 0f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _coolingDuration;

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
        Debug.Log($"Меч полностью остыл");
    }

    // Метод для принудительного охлаждения (можно вызвать извне)
    public void ForceCool()
    {
        _isHeating = false;
        if (_heatingCoroutine != null)
        {
            StopCoroutine(_heatingCoroutine);
        }
        _heatingCoroutine = StartCoroutine(CoolSwordCoroutine());
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
    }
}