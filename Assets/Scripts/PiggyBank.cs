// PiggyBank.cs
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class PiggyBank : MonoBehaviour
{
    [Header("Settings")]
    public int coinCount = 0;
    public bool destroyCoinOnInsert = true;
    public AudioClip insertSound;
    public ParticleSystem insertVfx;
    public bool saveToPlayerPrefs = false;
    public string playerPrefsKey = "PiggyBankCoins";

    public event Action<int> OnCoinCountChanged;

    AudioSource audioSource;

    void Awake()
    {
        // Убеждаемся, что Collider - триггер
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("PiggyBank collider is not set to isTrigger = true. Setting it automatically.");
            col.isTrigger = true;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (saveToPlayerPrefs)
        {
            coinCount = PlayerPrefs.GetInt(playerPrefsKey, coinCount);
        }
    }

    void Start()
    {
        // Уведомляем UI о начальном значении
        OnCoinCountChanged?.Invoke(coinCount);
    }

    void OnTriggerEnter(Collider other)
    {
        // Сравниваем по тегу
        if (other.CompareTag("Coin"))
        {
            InsertCoin(other.gameObject);
        }
    }

    public void InsertCoin(GameObject coinObj)
    {
        coinCount++;
        OnCoinCountChanged?.Invoke(coinCount);

        if (insertSound != null)
        {
            audioSource.PlayOneShot(insertSound);
        }

        if (insertVfx != null)
        {
            insertVfx.transform.position = transform.position + Vector3.up * 0.2f;
            insertVfx.Play();
        }

        if (saveToPlayerPrefs)
        {
            PlayerPrefs.SetInt(playerPrefsKey, coinCount);
            PlayerPrefs.Save();
        }

        if (destroyCoinOnInsert)
        {
            Destroy(coinObj);
        }
        else
        {
            coinObj.SetActive(false);
        }
    }
}
