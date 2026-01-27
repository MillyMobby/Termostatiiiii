using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void MasterScene()
    {
        SceneManager.LoadScene("MasterScene");
        PlayerPrefs.SetString("character", "master");
        PlayerPrefs.Save();
    }

    public void RedScene() 
    {
        SceneManager.LoadScene("PlayerScene");
        PlayerPrefs.SetString("character", "Karina");
        PlayerPrefs.Save();
    }

    public void BlueScene() 
    {
        SceneManager.LoadScene("PlayerScene");
        PlayerPrefs.SetString("character", "Kabo");
        PlayerPrefs.Save();
    }

    public void Back() 
    {
        SceneManager.LoadScene("Login");
        PlayerPrefs.SetString("character", "");
        PlayerPrefs.Save();
    }
}