using UnityEngine;

public class PopupManager : MonoBehaviour
{

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform canvasTransform;

    private GameObject activePopup;

    public void OpenPopup()
    {
        if (activePopup != null) return;
        activePopup = Instantiate(popupPrefab, canvasTransform);
    }
}
