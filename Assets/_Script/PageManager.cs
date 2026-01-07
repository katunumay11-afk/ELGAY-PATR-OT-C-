using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SayfaSistemi : MonoBehaviour
{
    [Header("SAYFA LİSTESİ")]
    // Sahnende (Hierarchy) hazır olan tüm sayfaları/panelleri buraya sırayla sürükle
    public List<GameObject> sayfalar; 

    [Header("BUTONLAR")]
    public Button ileriButon;
    public Button geriButon;

    private int suankiSayfa = 0; // Hangi sayfada olduğumuzu takip eder

    void Start()
    {
        // Butonlara tıklama görevlerini atıyoruz
        if (ileriButon != null) ileriButon.onClick.AddListener(IleriGit);
        if (geriButon != null) geriButon.onClick.AddListener(GeriGit);

        // Oyun başladığında sistemi ilk kez çalıştır
        SayfayiGuncelle();
    }

    // İLERİ BUTONU: Mevcut olanı kapatır, sonrakini açar
    public void IleriGit()
    {
        if (suankiSayfa < sayfalar.Count - 1)
        {
            suankiSayfa++;
            SayfayiGuncelle();
        }
    }

    // GERİ BUTONU: Mevcut olanı kapatır, bir öncekini açar
    public void GeriGit()
    {
        if (suankiSayfa > 0)
        {
            suankiSayfa--;
            SayfayiGuncelle();
        }
    }

    // ANA MANTIK: Sadece seçili sayfayı aktif eder, diğerlerini kapatır
    void SayfayiGuncelle()
    {
        for (int i = 0; i < sayfalar.Count; i++)
        {
            if (sayfalar[i] != null)
            {
                // Eğer sayfanın sırası "suankiSayfa" ise AKTİF yap, değilse KAPAT
                bool aktifMi = (i == suankiSayfa);
                sayfalar[i].SetActive(aktifMi);
            }
        }

        // Butonların tıklanabilirliğini otomatik ayarlar
        // İlk sayfada geri butonu, son sayfada ileri butonu pasif olur.
        if (ileriButon != null) ileriButon.interactable = (suankiSayfa < sayfalar.Count - 1);
        if (geriButon != null) geriButon.interactable = (suankiSayfa > 0);
    }
}