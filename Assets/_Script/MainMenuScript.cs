 using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    // Start butonuna tıklanınca çalışacak fonksiyon
    public void StartGame()
    {
        SceneManager.LoadScene("biologyscane");
    }
    public void farmscane()
    {
        SceneManager.LoadScene("AssetScene");
    }

    public void hospitalcane()
        {
            SceneManager.LoadScene("HospitalScene");
        }

    // Oyunu kapatmak için (QuitButton için)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatıldı."); // Editör içinde test amaçlı
    }
    public void Settings()
    {
        if(settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
}
