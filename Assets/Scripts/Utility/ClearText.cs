using TMPro;
using UnityEngine;

public class ClearText : MonoBehaviour
{
    public TMP_Text textField;

    public void Clear()
    {
        textField.SetText("");
    }
}
