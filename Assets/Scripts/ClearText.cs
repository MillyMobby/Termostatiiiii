using TMPro;
using UnityEngine;

public class ClearTextOnClose : MonoBehaviour
{
    public TMP_Text textField;

    public void Clear()
    {
        textField.SetText("");
    }
}
