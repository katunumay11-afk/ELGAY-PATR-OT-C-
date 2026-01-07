using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class MarketUrunButonu : MonoBehaviour
{
    [Header("--- KİMLİK (Çok Önemli!) ---")]
    [Tooltip("CiftlikYoneticisi listesindeki 'id' ile AYNI ismi yazmalısın!")]
    public string urunID; // Örn: traktor, doktor, inek

    [Header("--- FİYAT VE AYARLAR ---")]
    public int fiyat; // Bu ürün kaç para? (Her butonda farklı yapabilirsin)
    
    [Header("--- GEREKLİLER ---")]
    public Button buButon;            // Tıklanan butonun kendisi
    public TextMeshProUGUI coinTexti; // Sahnedeki Coin yazısı (Güncellemek için)

    [Header("--- OPSİYONEL ---")]
    public GameObject satildiResmi;   // Satılınca açılacak 'Tik' işareti (Varsa)

    void Start()
    {
        // Oyun açılınca kontrol et: Bu ürün daha önce alınmış mı?
        // CiftlikYoneticisi de aynı anahtara (urunID) bakıyor.
        if (PlayerPrefs.GetInt(urunID, 0) == 1)
        {
            ZatenAlinmisModunaGec();
        }
    }

    // BUTONUN "ON CLICK" KISMINA BU FONKSİYONU BAĞLA
    public void SatinAl()
    {
        // 1. Cüzdandaki parayı öğren
        int mevcutPara = PlayerPrefs.GetInt("ToplamCoin", 0);

        // 2. Para yetiyor mu?
        if (mevcutPara >= fiyat)
        {
            // --- EVET, YETİYOR ---
            
            // A. Parayı düş ve kaydet
            mevcutPara -= fiyat;
            PlayerPrefs.SetInt("ToplamCoin", mevcutPara);

            // B. Ürünü "ALINDI" olarak kaydet (1 = Alındı)
            // Bu kısım CiftlikYoneticisi'nin okuyacağı kısmı değiştirir!
            PlayerPrefs.SetInt(urunID, 1);
            PlayerPrefs.Save();

            // C. Ekrandaki parayı güncelle
            if (coinTexti != null) coinTexti.text = "Para: " + mevcutPara.ToString();

            // D. Butonu kilitle
            ZatenAlinmisModunaGec();

            Debug.Log(urunID + " satın alındı! Diğer sahnede artık görünecek.");
        }
        else
        {
            Debug.Log("Paran yetersiz!");
        }
    }

    void ZatenAlinmisModunaGec()
    {
        // Butonu tıklanamaz yap
        if (buButon != null) buButon.interactable = false;

        // Varsa 'Satıldı' (Tik) resmini aç
        if (satildiResmi != null) satildiResmi.SetActive(true);
    }
}