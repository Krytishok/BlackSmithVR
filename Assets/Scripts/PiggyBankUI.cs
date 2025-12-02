using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(PiggyBank))]
public class PiggyBankUI : MonoBehaviour
{
    [Tooltip("TextMeshProUGUI компонент, показывающий число")]
    public TextMeshProUGUI countText;

    [Tooltip("Transform Canvas (или UI root) для pop-анимации и билборда")]
    public Transform uiTransform;

    public float popScale = 1.15f;
    public float popDuration = 0.12f;

    PiggyBank pig;

    void Awake()
    {
        pig = GetComponent<PiggyBank>();
        if (pig == null) Debug.LogError("PiggyBankUI requires PiggyBank on same GameObject.");
        pig.OnCoinCountChanged += UpdateUI;
    }

    void Start()
    {
        UpdateUI(pig.coinCount);
    }

    void UpdateUI(int newCount)
    {
        if (countText != null) countText.text = newCount.ToString();
        if (uiTransform != null) StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        if (uiTransform == null) yield break;
        Vector3 orig = uiTransform.localScale;
        Vector3 target = orig * popScale;
        float half = popDuration * 0.5f;
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            uiTransform.localScale = Vector3.Lerp(orig, target, t / half);
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            uiTransform.localScale = Vector3.Lerp(target, orig, t / half);
            yield return null;
        }
        uiTransform.localScale = orig;
    }

    void OnDestroy()
    {
        if (pig != null) pig.OnCoinCountChanged -= UpdateUI;
    }
}
