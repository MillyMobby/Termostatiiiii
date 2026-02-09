using System;
using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour
{

    private static List<Character> characters;
    private static List<Monster> monsters;
    private static List<Obstacle> obstacles;

    // * data load settings
    public static event Action OnDataLoaded;
    private static bool isLoaded = false;
    public static bool IsLoaded
    {
        get => isLoaded;
        private set => isLoaded = value;
    }


    async void Awake()
    {
        DontDestroyOnLoad(gameObject);

        characters = await Requester.RequestCharacters();
        monsters = await Requester.RequestMonsters();
        obstacles = await Requester.RequestObstacles();

        IsLoaded = true;
        OnDataLoaded?.Invoke();
    }


    public static void Printette()
    {
        if (characters == null) {
            Debug.Log("No characters loaded.");
            return;
        }
        foreach (Character c in characters) Debug.Log(c.Name);
    }


    public static List<Character> GetCharacters() => characters;
    public static List<Monster> GetMonsters() => monsters;
    public static List<Obstacle> GetObstacles() => obstacles;

}
