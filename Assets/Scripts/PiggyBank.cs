using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class PiggyBank : MonoBehaviour
{
    [Header("Настройки")]
    public int coinCount = 0;
    public bool destroyCoinOnInsert = true; // если false — просто деактивируем монету (для пула)
    public AudioClip insertSound;
    public ParticleSystem insertVfx;
    public bool saveToPlayerPrefs = false;
    public string playerPrefsKey = "PiggyBankCoins";

    public event Action<int> OnCoinCountChanged;

    AudioSource audioSource;

    void Awake()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (saveToPlayerPrefs)
            coinCount = PlayerPrefs.GetInt(playerPrefsKey, coinCount);
    }

    void Start()
    {
        OnCoinCountChanged?.Invoke(coinCount);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;
        InsertCoin(other.gameObject);
    }

    public void InsertCoin(GameObject coin)
    {
        coinCount++;
        OnCoinCountChanged?.Invoke(coinCount);

        if (insertSound != null) audioSource.PlayOneShot(insertSound);
        if (insertVfx != null) insertVfx.Play();

        if (saveToPlayerPrefs)
        {
            PlayerPrefs.SetInt(playerPrefsKey, coinCount);
            PlayerPrefs.Save();
        }

        if (destroyCoinOnInsert)
            Destroy(coin);
        else
            coin.SetActive(false);
    }
}
