// PiggyBankUI.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(PiggyBank))]
public class PiggyBankUI : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public Transform uiTransform; // сам Canvas transform для билбординга
    PiggyBank pig;

    void Awake()
    {
        pig = GetComponent<PiggyBank>();
        if (pig == null) Debug.LogError("PiggyBankUI requires PiggyBank on same GameObject.");

        if (countText == null)
            Debug.LogWarning("Assign countText in inspector.");

        pig.OnCoinCountChanged += UpdateUI;
    }

    void Start()
    {
        UpdateUI(pig.coinCount);
    }

    void UpdateUI(int newCount)
    {
        if (countText != null)
            countText.text = newCount.ToString();
        // можно добавить анимацию тут (scale, color flash)
    }

    void LateUpdate()
    {
        if (uiTransform != null && Camera.main != null)
        {
            // Билборд — всегда лицом к камере
            uiTransform.rotation = Quaternion.LookRotation(uiTransform.position - Camera.main.transform.position);
        }
    }

    void OnDestroy()
    {
        if (pig != null) pig.OnCoinCountChanged -= UpdateUI;
    }
}
