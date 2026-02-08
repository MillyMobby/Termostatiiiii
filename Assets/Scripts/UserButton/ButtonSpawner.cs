using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonSpawner : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private UserButton userButtonPrefab;
    [SerializeField] private Transform contentContainer;


    [Header("Testing data")]
    private string[] btnNames = { "Red", "Green", "Blue" };
    private Color[] colors = { Color.red, Color.green, Color.blue };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Character c in Persist.GetCharacters())
        {
            Debug.Log($"CreateButton(text: {c.Name}, selectedClass: {c.Class}, col: {Color.red})");
            CreateButton(text: c.Name, selectedClass: c.Class, col: Color.red);
        }
    }

    public void CreateButton(string text, string selectedClass, Color col)
    {
        UserButton newBtn = Instantiate(userButtonPrefab, contentContainer);

        newBtn.Configure(
            text,
            selectedClass,
            col,
            () => OnButtonClicked()
        );
    }


    private void OnButtonClicked()
    {
        Debug.Log("Button clicked.");
    }
}
