using UnityEngine;

public class PopupManager : MonoBehaviour
{

[SerializeField] private GameObject popupPrefab;
[SerializeField] private Transform canvasTransform;

private GameObject activePopup;

    public void OpenPopup()
    {
        if (activePopup != null) { activePopup.SetActive(true); return; }
        activePopup = Instantiate(popupPrefab, canvasTransform);

        RectTransform rect = activePopup.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.pivot = new Vector2(1f, 1f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }
    }


    public void DestroyPopup()
    {
        if (activePopup == null) return;
        activePopup.SetActive(false);
        //Destroy(activePopup);
    }
}