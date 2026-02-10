using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonSpawner : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private UserButton userButtonPrefab;
    [SerializeField] private Transform contentContainer;


    [Header("Testing data")]
    private Color[] colors = { Color.red, Color.green, Color.blue };


    void Start()
    {
        if (Persist.IsLoaded) InitializeCharacterButtons();
        else Persist.OnDataLoaded += InitializeCharacterButtons;
    }


    void InitializeCharacterButtons()
    {
        List<Character> characters = Persist.GetCharacters();

        for (int i = 0; i < Mathf.Min(characters.Count, colors.Length); i++)
        {
            CreateButton(text: characters[i].Name, selectedClass: characters[i].Class, col: colors[i]);
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
        return;
    }
}