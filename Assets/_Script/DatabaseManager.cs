using UnityEngine;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

// --- 1. VERİ MODELİ ---
[System.Serializable]
public class Soru
{
    public int kategoriID;
    public string soruMetni;
    public string A, B, C, D; 
    public string dogruCevap;

    public Soru(int id, string soru, string a, string b, string c, string d, string dogru)
    {
        this.kategoriID = id;
        this.soruMetni = soru;
        this.A = a;
        this.B = b;
        this.C = c;
        this.D = d;
        this.dogruCevap = dogru;
    }
}

// --- 2. YÖNETİCİ SINIF ---
public class DatabaseManager : MonoBehaviour
{
    [Header("TEXT NESNELERİ")]
    public TextMeshProUGUI puanText; // Bölüm puanı
    public TextMeshProUGUI canText;  // Kalan can
    public TextMeshProUGUI coinText; // Toplam Coin (Para)

    [Header("SORU ALANLARI")]
    public TextMeshProUGUI soruMetniText;      
    public TextMeshProUGUI secenekAText;       
    public TextMeshProUGUI secenekBText;       
    public TextMeshProUGUI secenekCText;       
    public TextMeshProUGUI secenekDText;       
    public TextMeshProUGUI sonucText;          

    [Header("KARAR PANELİ")]
    public GameObject kararPaneli; 
    public Button btnHarca;        
    public Button btnDevam;        

    [Header("ŞIK BUTONLARI")]
    public Button butonA;
    public Button butonB;
    public Button butonC;
    public Button butonD;

    [Header("AYARLAR")]
    public float beklemeSuresi = 3f;

    [HideInInspector]
    public List<Soru> tumSorularHavuzu = new List<Soru>();

    private List<Soru> aktifSoruListesi;
    private string dogruSik;
    
    private int toplamPuan = 0;   // O anki bölüm skoru
    private int toplamCoin = 0;   // Kumbaradaki toplam para
    private int kalanCan = 3;
    private int sonKontrolPuani = 0;
    private bool cevapVerildiMi = false;

    void Start()
    {
        // Butonları Kur
        if(butonA) { butonA.onClick.RemoveAllListeners(); butonA.onClick.AddListener(() => CevapVer("A")); }
        if(butonB) { butonB.onClick.RemoveAllListeners(); butonB.onClick.AddListener(() => CevapVer("B")); }
        if(butonC) { butonC.onClick.RemoveAllListeners(); butonC.onClick.AddListener(() => CevapVer("C")); }
        if(butonD) { butonD.onClick.RemoveAllListeners(); butonD.onClick.AddListener(() => CevapVer("D")); }

        // Panel Butonlarını Kur
        if(btnHarca) { btnHarca.onClick.RemoveAllListeners(); btnHarca.onClick.AddListener(MarketSahnesineGit); }
        if(btnDevam) { btnDevam.onClick.RemoveAllListeners(); btnDevam.onClick.AddListener(OyunaDevamEt); }

        if(kararPaneli) kararPaneli.SetActive(false);

        VeritabaniniDoldur();
        
        toplamPuan = 0;
        kalanCan = 3;

        // --- HAFIZADAKİ PARAYI YÜKLE ---
        toplamCoin = PlayerPrefs.GetInt("ToplamCoin", 0);
        UI_Guncelle();

        // --- MENÜDEN SEÇİLEN KATEGORİYİ BAŞLAT ---
        int gelenID = PlayerPrefs.GetInt("SecilenKategoriID", 1);
        Debug.Log("Menüden Gelen Kategori ID: " + gelenID);
        KategoriBaslat(gelenID);
    }

    public void KategoriBaslat(int istenenID)
    {
        aktifSoruListesi = tumSorularHavuzu.FindAll(x => x.kategoriID == istenenID);

        if (aktifSoruListesi != null && aktifSoruListesi.Count > 0)
        {
            ListeyiKaristir(aktifSoruListesi);
            SoruGoster(0);
        }
        else
        {
            if(sonucText) sonucText.text = "Bu kategori için soru bulunamadı! (ID: " + istenenID + ")";
        }
    }

    public void SoruGoster(int index)
    {
        if (aktifSoruListesi == null || index >= aktifSoruListesi.Count)
        {
            if(sonucText) sonucText.text = "Bölüm Bitti!";
            Invoke("AnaMenuyeDon", 3f);
            return;
        }

        cevapVerildiMi = false; 
        Soru s = aktifSoruListesi[index];
        soruMetniText.text = s.soruMetni;
        secenekAText.text = "A) " + s.A;
        secenekBText.text = "B) " + s.B;
        secenekCText.text = "C) " + s.C;
        secenekDText.text = "D) " + s.D;
        dogruSik = s.dogruCevap;
        if(sonucText) sonucText.text = ""; 
    }

    void CevapVer(string secilenSik)
    {
        if (cevapVerildiMi) return;
        cevapVerildiMi = true;

        if (secilenSik == dogruSik)
        {
            // --- DOĞRU BİLİNCE ---
            int kazanilan = 50; 

            toplamPuan += kazanilan; 
            toplamCoin += kazanilan; 

            // --- PARAYI KAYDET ---
            PlayerPrefs.SetInt("ToplamCoin", toplamCoin);
            PlayerPrefs.Save();

            sonucText.text = "<color=green>TEBRİKLER!</color>";
            
            if (toplamPuan > 0 && toplamPuan % 500 == 0 && toplamPuan > sonKontrolPuani)
            {
                sonKontrolPuani = toplamPuan;
                Invoke("PaneliAc", 1.5f);
            }
            else Invoke("SonrakiSoru", 1.5f);
        }
        else
        {
            kalanCan--;
            Soru anlikSoru = aktifSoruListesi[0];
            string dogruMetin = (dogruSik=="A"?anlikSoru.A : (dogruSik=="B"?anlikSoru.B : (dogruSik=="C"?anlikSoru.C : anlikSoru.D)));
            sonucText.text = $"<color=red>YANLIŞ!</color>\nDoğru Cevap: <color=yellow>{dogruMetin}</color>";

            if (kalanCan <= 0) Invoke("MarketSahnesineGit", 4f);
            else Invoke("SonrakiSoru", beklemeSuresi);
        }
        UI_Guncelle();
    }

    public void SonrakiSoru()
    {
        if (aktifSoruListesi.Count > 0)
        {
            aktifSoruListesi.RemoveAt(0); 
            if (aktifSoruListesi.Count > 0) SoruGoster(0);
            else { sonucText.text = "Bölüm Bitti!"; Invoke("AnaMenuyeDon", 3f); }
        }
    }

    void PaneliAc() { if(kararPaneli) kararPaneli.SetActive(true); }
    void OyunaDevamEt() { if(kararPaneli) kararPaneli.SetActive(false); SonrakiSoru(); }
    void AnaMenuyeDon() { SceneManager.LoadScene("SampleScene"); } 
    void MarketSahnesineGit() { SceneManager.LoadScene("HospitalScene"); } 
    
    void UI_Guncelle() 
    { 
        if(puanText) puanText.text = "PUAN: " + toplamPuan; 
        if(canText) canText.text = "CAN: " + kalanCan;
        if(coinText) coinText.text = "COIN: " + toplamCoin;
    }
    
    void ListeyiKaristir(List<Soru> liste) { for (int i = 0; i < liste.Count; i++) { Soru temp = liste[i]; int rnd = Random.Range(i, liste.Count); liste[i] = liste[rnd]; liste[rnd] = temp; } }

    void VeritabaniniDoldur()
    {
        tumSorularHavuzu.Clear();

        // -------------------------------------------------------------
        // ID 1: SİNİR SİSTEMİ (Resimdeki 1. Sıra)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(1, "Beyin hangi kemik yapısının içinde yer alır?", "Kas", "Kalp", "Deri", "Kafatası", "D"));
        tumSorularHavuzu.Add(new Soru(1, "Vücut sıcaklığını düzenleyen beyin bölgesi hangisidir?", "Hipofiz", "Hipotalamus", "Hipokamp", "Epitalamus", "B"));
        tumSorularHavuzu.Add(new Soru(1, "Hafıza ve öğrenme süreçlerinde en çok rol oynayan beyin bölgesi hangisidir?", "Serebellum", "Hipotalamus", "Hipokampus", "Epitalamus", "C"));
        tumSorularHavuzu.Add(new Soru(1, "Denge ve koordinasyon hangi beyin bölgesiyle sağlanır?", "Hipokampus", "Hipotalamus", "Serebrum", "Serebellum", "D"));
        tumSorularHavuzu.Add(new Soru(1, "Vücutta ağrı algısını ileten sinir lifleri hangileridir?", "A lifleri", "B lifleri", "C lifleri", "D lifleri", "C"));
        tumSorularHavuzu.Add(new Soru(1, "Beyin-omurilik sıvısı nerede üretilir?", "Koroid Pleksus", "Korteks", "Substans Paryetalis", "Substans Nigra", "A"));
        tumSorularHavuzu.Add(new Soru(1, "Beynin görme ile ilgili bölgesi hangisidir?", "Frontal lop", "Parietal lop", "Oksipital lop", "Temporal lop", "C"));
        tumSorularHavuzu.Add(new Soru(1, "Beyin ve omuriliği çevreleyen koruyucu zarlar hangileridir?", "Pia, archoid, dura mater", "Pia, dura mater, meninx", "Pia, dura mater, arachnoid", "Pia, archoid, meninx", "A"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki bilgi aktarım hızını artıran yapı nedir?", "Akson ucu", "Dendirit", "Miyelin kılıf", "Sinirli kas", "C"));
        tumSorularHavuzu.Add(new Soru(1, "Beynin sol ve sağ yarım küresini bağlayan yapı nedir?", "Corpus callosum", "Splenum", "Pons", "Medulla oblongata", "A"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki duyusal bilgilerin toplanıp yönlendirildiği merkez hangisidir?", "Hipofiz", "Hipokamp", "Talamus", "Hipotalamus", "C"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki açlık ve tokluk merkezleri hangi yapıda bulunur?", "Hipofiz", "Hipokamp", "Talamus", "Hipotalamus", "D"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki hormon salgılayan bez hangisidir?", "Hipofiz", "Hipokamp", "Talamus", "Hipotalamus", "D"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki hafıza oluşumunda görev alan yapı hangisidir?", "Hipofiz", "Hipokampus", "Talamus", "Hipotalamus", "B"));
        tumSorularHavuzu.Add(new Soru(1, "Beyindeki işitme ve konuşma merkezleri hangi lobda yer alır?", "Frontal", "Occipital", "Parietal", "Temporal", "D"));

        // -------------------------------------------------------------
        // ID 2: DOLAŞIM SİSTEMİ (Resimdeki 2. Sıra)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(2, "Hangi organ oksijenli kanı vücuda pompalar?", "Kas", "Kalp", "Deri", "Akciğer", "B"));
        tumSorularHavuzu.Add(new Soru(2, "Oksijenli kanı vücuda pompalayan kalp bölümü hangisidir?", "Sol ventrikül", "Sağ ventrikül", "Aort kapağı", "Trakea", "A"));
        tumSorularHavuzu.Add(new Soru(2, "Kemik iliği hangi hücreleri üretir?", "Kas hücreleri", "Kan hücreleri", "Sperm hücreleri", "Kemik hücreleri", "B"));
        tumSorularHavuzu.Add(new Soru(2, "Kan basıncını düzenleyen hormon sistemi hangisidir?", "Adrenalin-Norepinefrin", "Renin-Angiotensin-Aldosteron", "Insulin-Glikojen", "Estradiol-Östrojen", "B"));
        tumSorularHavuzu.Add(new Soru(2, "Kanın vücutta dolaşımını sağlayan damar türü hangisidir?", "Arter", "Ven", "Arterioven", "Kapiller", "A"));
        tumSorularHavuzu.Add(new Soru(2, "Alyuvarların ömrü ortalama ne kadardır?", "30 gün", "60 gün", "90 gün", "120 gün", "D"));
        tumSorularHavuzu.Add(new Soru(2, "Kanın kalbe geri dönüşünü sağlayan damarlar hangileridir?", "Arterler", "Venler", "Arterioven", "Kapiller", "B"));
        tumSorularHavuzu.Add(new Soru(2, "Kalbin kas dokusu hangi özel kas türüdür?", "Sinirli kas", "İskelet kası", "Kalp kası", "Sarıkas", "C"));
        tumSorularHavuzu.Add(new Soru(2, "Kanın damar dışına çıkmasını engelleyen süreç nedir?", "Transkripsiyon", "Koagülasyon", "Metabolizma", "Oksidatif fosforilasyon", "B"));
        tumSorularHavuzu.Add(new Soru(2, "Kalbin sağ kulakçığına gelen kan hangi damardan gelir?", "Vena Cava", "Vena Subclavia", "Vena Jugularis", "Vena Subclavia", "A"));
        tumSorularHavuzu.Add(new Soru(2, "Kalbin kasılma sırasında kanı pompaladığı evreye ne ad verilir?", "Diastol", "Sistol", "Eksol", "Vetrikül", "B"));

        // -------------------------------------------------------------
        // ID 3: İSKELET SİSTEMİ (Resimdeki 3. Sıra)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(3, "İnsan vücudunda kaç kemik vardır (yetişkinlerde)?", "206", "200", "204", "208", "A"));
        tumSorularHavuzu.Add(new Soru(3, "Kemik iliği hangi hücreleri üretir?", "Kas Hücreleri", "Kan hücreleri", "Sperm hücreleri", "Kemik hücreleri", "B"));
        tumSorularHavuzu.Add(new Soru(3, "Kemiklerin uç kısımlarında bulunan ve büyümeyi sağlayan yapı nedir?", "Osteoblast", "Osteoklast", "Epifiz plağı", "Osteokondrit", "C"));
        tumSorularHavuzu.Add(new Soru(3, "Kemik dokusunun yıkımını gerçekleştiren hücreler hangileridir?", "Osteoklastlar", "Osteoblastlar", "Osteokondritler", "Osteofiller", "A"));
        tumSorularHavuzu.Add(new Soru(3, "Kemik dokusunun yeniden yapılandırılmasında rol oynayan hücreler hangileridir?", "Osteoklastlar", "Osteoblastlar", "Osteokondritler", "Osteofiller", "B"));
        tumSorularHavuzu.Add(new Soru(3, "Kemiklerin sertliğini sağlayan mineral nedir?", "Kalsiyum fosfat", "Kalsiyum klorür", "Kalsiyum karbonat", "Kalsiyum sitrat", "A"));
        tumSorularHavuzu.Add(new Soru(3, "Kasların kemiğe bağlandığı yapılar hangileridir?", "Kemik", "Kas", "Tendon", "Miyofibril", "C"));
        tumSorularHavuzu.Add(new Soru(3, "Kıkırdak dokunun beslenmesi nasıl gerçekleşir?", "Difüzyonla", "Endokrin yolla", "Kolonizasyonla", "Oksidatif fosforilasyonla", "A"));

        // -------------------------------------------------------------
        // ID 4: KAS SİSTEMİ (Resimdeki 4. Sıra)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(4, "Kas kasılması sırasında hangi iyon hücre içine girerek süreci başlatır?", "Kalsiyum", "Potasyum", "Sodyum", "Klor", "A"));
        tumSorularHavuzu.Add(new Soru(4, "Solunum sırasında diyafram kasıldığında ne olur?", "Hava dışarı çıkar", "Göğüs boşluğu daralır", "Akciğerler küçülür", "Akciğerlere hava girer", "D"));
        tumSorularHavuzu.Add(new Soru(4, "Mide hangi tür kaslarla kasılır?", "Sinirli Kas", "Düz Kas", "İskelet Kası", "Sarıkas", "B"));
        tumSorularHavuzu.Add(new Soru(4, "Kasların oksijen ihtiyacını karşılayan pigment hangisidir?", "Eritrosit", "Hemoglobin", "Mioglobin", "Klorofil", "C"));
        tumSorularHavuzu.Add(new Soru(4, "Kalbin kas dokusu hangi özel kas türüdür?", "Sinirli kas", "İskelet kası", "Kalp kası", "Sarıkas", "C"));
        tumSorularHavuzu.Add(new Soru(4, "Kas dokusunun hücresel yapı taşlarından biri olan miyofibrillerin içindeki proteinler hangisidir?", "Hemoglobin ve mioglobin", "Kollajen ve elastin", "Eritrosit ve trombosit", "Aktin ve miyozin", "D"));

        // -------------------------------------------------------------
        // ID 5: BAĞIŞIKLIK SİSTEMİ (Senin verdiğin ID: 9, Resimdeki Sıra: 5)
        // (Kodda ID 5 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(5, "Hangi sistem vücudu hastalıklara karşı korur?", "Sinir sistemi", "Bağışıklık sistemi", "Sindirim sistemi", "Endokrin sistem", "B"));
        tumSorularHavuzu.Add(new Soru(5, "İmmün sistemde antikor üreten hücreler hangileridir?", "T lenfositler", "B lenfositler", "T ve B lenfositler", "Makrofajlar", "B"));
        tumSorularHavuzu.Add(new Soru(5, "Bağışıklık sisteminde fagositoz yapan hücreler hangileridir?", "Makrofajlar", "T lenfositler", "B lenfositler", "Eritrositler", "A"));

        // -------------------------------------------------------------
        // ID 6: SİNDİRİM SİSTEMİ (Senin verdiğin ID: 5, Resimdeki Sıra: 6)
        // (Kodda ID 6 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(6, "Sindirim sisteminde besinlerin emildiği ana organ hangisidir?", "Kas", "Kalp", "Deri", "Bağırsak", "D"));
        tumSorularHavuzu.Add(new Soru(6, "Hangi organ hem endokrin hem de ekzokrin işlev görür?", "Pankreas", "Beyin", "Kalp", "Orta Beyin", "A"));
        tumSorularHavuzu.Add(new Soru(6, "Sindirim sisteminde yağların emilimi esas olarak nerede gerçekleşir?", "Kolosterol", "Mide", "Kalın bağırsak", "İnce bağırsak", "D"));
        tumSorularHavuzu.Add(new Soru(6, "Mide hangi tür kaslarla kasılır?", "Sinirli Kas", "Düz Kas", "İskelet Kası", "Sarıkas", "B"));
        tumSorularHavuzu.Add(new Soru(6, "Mide mukozasını koruyan madde hangisidir?", "Hemoglobin", "Müsin", "Klorofil", "Eritrosit", "B"));
        tumSorularHavuzu.Add(new Soru(6, "Sindirim sisteminde enzimlerin etkili olabilmesi için gerekli pH ortamı nerede asidiktir?", "Pankreas", "Kalın bağırsak", "İnce bağırsak", "Mide", "D"));

        // -------------------------------------------------------------
        // ID 7: HÜCRE BİYOLOJİSİ (Senin verdiğin ID: 6, Resimdeki Sıra: 7)
        // (Kodda ID 7 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(7, "İnsan vücudunda en küçük hücre hangisidir?", "Kas hücresi", "Kan hücresi", "Sperm hücresi", "Kemik hücresi", "C"));
        tumSorularHavuzu.Add(new Soru(7, "Genetik materyalin bulunduğu hücre bölümü hangisidir?", "Sitoplazma", "Lizozom", "Çekirdek", "Mitokondri", "C"));
        tumSorularHavuzu.Add(new Soru(7, "Hücrede proteinlerin paketlenip taşınmasını sağlayan organel nedir?", "Mitokondri", "Lizozom", "Golgi aygıtı", "Ribozom", "C"));
        tumSorularHavuzu.Add(new Soru(7, "Hücrede genetik bilginin okunmasını sağlayan süreç nedir?", "Transkripsiyon", "Translasyon", "Metabolizma", "Oksidatif fosforilasyon", "A"));
        tumSorularHavuzu.Add(new Soru(7, "Hücrelerde enerji üreten enzim hangisidir?", "ATPaz", "Kloroz", "Klorofil", "Eritrosit", "A"));
        tumSorularHavuzu.Add(new Soru(7, "Hücre zarının seçici geçirgenliğini sağlayan özellik nedir?", "Fosfolipid çift tabaka", "Translasyon", "Metabolizma", "Oksidatif fosforilasyon", "A"));
        tumSorularHavuzu.Add(new Soru(7, "Hücre bölünmesinde kromozomların ayrıldığı evre nedir?", "Anafaz", "İnterfaz", "Metafaz", "Telofaz", "A"));
        tumSorularHavuzu.Add(new Soru(7, "Hücrede enerji üretiminde oksijenin kullanıldığı süreç nedir?", "Fermantasyon", "Aerobik solunum", "Anaerobik solunum", "Kolik asit sentezi", "C"));

        // -------------------------------------------------------------
        // ID 8: GENEL FİZYOLOJİ (Senin verdiğin ID: 10, Resimdeki Sıra: 8)
        // (Kodda ID 8 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(8, "İnsan vücudunda en büyük organ hangisidir?", "Kas", "Kalp", "Deri", "Akciğer", "C"));
        tumSorularHavuzu.Add(new Soru(8, "Vücutta pH dengesini koruyan tampon sistemlerden biri hangisidir?", "Sodyum-potasyum sistem", "Glikoz metobolizması", "Karbonik asit-bikarbonat sistem", "Kalsiyum-potasyum sistem", "C"));
        tumSorularHavuzu.Add(new Soru(8, "Vücutta suyun en fazla bulunduğu yer neresidir?", "Kan", "Hücre içi sıvı", "Hücre zarı", "Hücre dışı sıvı", "B"));

        // -------------------------------------------------------------
        // ID 9: ENDOKRİN SİSTEM (Senin verdiğin ID: 7, Resimdeki Sıra: 9)
        // (Kodda ID 9 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(9, "İnsan vücudunda hangi sistem hormon üretir?", "Sinir sistemi", "Endokrin sistem", "Sindirim sistemi", "Endokrin sistem", "B"));
        tumSorularHavuzu.Add(new Soru(9, "Hangi organ hem endokrin hem de ekzokrin işlev görür?", "Pankreas", "Beyin", "Kalp", "Orta Beyin", "A"));
        tumSorularHavuzu.Add(new Soru(9, "Kan basıncını düzenleyen hormon sistemi hangisidir?", "Adrenalin-Norepinefrin", "Renin-Angiotensin-Aldosteron", "Insulin-Glikojen", "Estradiol-Östrojen", "B"));
        tumSorularHavuzu.Add(new Soru(9, "Beyindeki açlık ve tokluk merkezleri hangi yapıda bulunur?", "Hipofiz", "Hipokamp", "Talamus", "Hipotalamus", "D"));
        tumSorularHavuzu.Add(new Soru(9, "Vücutta stresle mücadelede görev alan hormon hangisidir?", "Adrenalin", "Kortizol", "Testosteron", "Östrojen", "B"));

        // -------------------------------------------------------------
        // ID 10: SOLUNUM SİSTEMİ (Senin verdiğin ID: 8, Resimdeki Sıra: 10)
        // (Kodda ID 10 olarak düzeltildi)
        // -------------------------------------------------------------
        tumSorularHavuzu.Add(new Soru(10, "Vücutta oksijen alışverişi nerede gerçekleşir?", "Kas", "Kalp", "Deri", "Akciğerler", "D"));
        tumSorularHavuzu.Add(new Soru(10, "Akciğerlerde gaz değişimi hangi yapıda gerçekleşir?", "Alveol", "Kanallar", "Kemik", "Kas", "A"));
        tumSorularHavuzu.Add(new Soru(10, "Solunum sırasında diyafram kasıldığında ne olur?", "Hava dışarı çıkar", "Göğüs boşluğu daralır", "Akciğerler küçülür", "Akciğerlere hava girer", "D"));

        Debug.Log("✅ Tüm sorular (Resimdeki Sıraya Göre) yüklendi.");
    }
}