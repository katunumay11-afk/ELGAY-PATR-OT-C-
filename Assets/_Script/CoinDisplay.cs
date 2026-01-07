using UnityEngine;
using TMPro; // TextMeshPro

public class CoinDisplay : MonoBehaviour
{
    // Unity'de buraya TextMeshPro objesini sürükleyeceksin
    public TextMeshProUGUI coinTexti; 

    void Start()
    {
        // 1. BANKADAN PARAYI ÇEK
        // DatabaseManager "ToplamCoin" anahtarıyla parayı kaydetti.
        // Biz de aynı anahtarla ("ToplamCoin") parayı çekiyoruz.
        int para = PlayerPrefs.GetInt("ToplamCoin", 0);

        // 2. EKRANA YAZ
        if (coinTexti != null)
        {
            coinTexti.text = "Para: " + para.ToString();
        }
    }
}