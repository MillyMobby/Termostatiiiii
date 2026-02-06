using UnityEngine;

public class ClickOutsideToClose : MonoBehaviour
{
    public GameObject popupPrefab;


    public void ClosePopup()
    {
        Debug.Log("Trying to destroy " + popupPrefab.name);
        Destroy(popupPrefab);
    }
}
