using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonSpawner : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private UserButton userButtonPrefab;
    [SerializeField] private Transform contentContainer;


    [Header("Testing data")]
    [SerializeField] private string dndClass = "bard";
    private string[] dndClasses = { "bard", "thief", "warrior" };
    private string[] btnNames = { "Red", "Green", "Blue" };
    private Color[] colors = { Color.red, Color.green, Color.blue };
    [SerializeField] private Color btnColor = Color.red;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < dndClasses.Length; i++) 
            CreateButton(btnNames[i], dndClasses[i], colors[i]);
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
