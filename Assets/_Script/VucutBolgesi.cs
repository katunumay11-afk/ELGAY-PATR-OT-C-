using UnityEngine;
using UnityEngine.SceneManagement; // Sahne değişimi için gerekli

public class VucutBolgesi : MonoBehaviour
{
    [Header("Bu Bölgenin ID'si Kaç?")]
    public int kategoriID; // Inspector'dan; Kafa için 1, Kol için 3 yaz.

    // Butona tıklandığında bu çalışacak
    public void BolgeyeTiklandi()
    {
        // 1. Seçilen ID'yi hafızaya (PlayerPrefs) kaydet
        PlayerPrefs.SetInt("SecilenKategoriID", kategoriID);
        
        // 2. Oyun sahnesini yükle
        // (Sahne adının "GameScene" olduğundan emin ol)
        SceneManager.LoadScene("QuestionsScene");
    }
}