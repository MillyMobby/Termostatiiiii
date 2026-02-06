using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour
{

    private static List<Character> characters;
    private static List<Monster> monsters;
    private static List<Obstacle> obstacles;

    async void Awake()
    {
        DontDestroyOnLoad(gameObject);

        characters = await Requester.RequestCharacters();
        monsters = await Requester.RequestMonsters();
        obstacles = await Requester.RequestObstacles();
    }


    public static void Printette()
    {
        if (characters == null) return;
        foreach (Character c in characters) Debug.Log(c.Name);
    }


    public static List<Character> GetCharacters() { return characters; }
    public static List<Monster> GetMonsters() { return monsters; }
    
    public static List<Obstacle> GetObstacles() { return obstacles; }

}
