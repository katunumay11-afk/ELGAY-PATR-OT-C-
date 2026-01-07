using UnityEngine;
using System.Collections.Generic; // Listeleri kullanmak için gerekli

public class CiftlikYoneticisi : MonoBehaviour
{
    // Bu küçük sınıf, listede hem ID'yi hem de Objeyi yan yana görmeni sağlar
    [System.Serializable]
    public class Esya
    {
        public string id;           // Marketle aynı isim (Örn: traktor)
        public GameObject gorsel;   // Sahnedeki obje (Örn: Traktor Resmi)
    }

    [Header("TÜM EŞYALARI BURAYA EKLE")]
    public List<Esya> ciftlikEsyalari; // Inspector'da liste olarak görünecek

    void Start()
    {
        // Listedeki her bir eşya için tek tek kontrol yap
        foreach (Esya esya in ciftlikEsyalari)
        {
            // Hafızaya bak: Bu ID alınmış mı?
            int alindiMi = PlayerPrefs.GetInt(esya.id, 0);

            if (alindiMi == 1)
            {
                // Alınmışsa GÖSTER
                if(esya.gorsel != null) 
                    esya.gorsel.SetActive(true);
            }
            else
            {
                // Alınmamışsa GİZLE
                if(esya.gorsel != null) 
                    esya.gorsel.SetActive(false);
            }
        }
    }
}