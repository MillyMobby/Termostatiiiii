using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private SceneAsset targetScene;

    public void LoadScene()
    {
        if (targetScene == null) return;
        SceneManager.LoadScene(targetScene.name);
    }

}
