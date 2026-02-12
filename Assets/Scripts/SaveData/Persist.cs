using System;
using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour
{

    private static List<Character> characters;
    public static List<Character> Characters => characters;
    private static List<Monster> monsters;
    public static List<Monster> Monsters => monsters;
    private static List<Obstacle> obstacles;
    public static List<Obstacle> Obstacles => obstacles;

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
        IsLoaded = true;
        OnDataLoaded?.Invoke();
        monsters = await Requester.RequestMonsters();
        obstacles = await Requester.RequestObstacles();

        foreach (var character in characters)
        {
            List<CreatureEntity.Action> actions = await Requester.RequestActions(character.Name);
            List<CreatureEntity.Action> spells = await Requester.RequestSpells(character.Name);
            actions.AddRange(spells);
            character.Actions = actions;
        }

        foreach (var monster in monsters)
        {
            List<CreatureEntity.Action> actions = await Requester.RequestMonsterActions(monster.Name);
            monster.Actions = actions;
        }
        Debug.Log("Finito");
    }


    public static void Printette()
    {
        if (characters == null) {
            Debug.Log("No characters loaded.");
            return;
        }
        foreach (Character c in characters)
        {
            Debug.Log(c.Name);
            foreach (CreatureEntity.Action a in c.Actions)
            {
                Debug.Log(a.actionName);
            }
        }

        if (monsters == null)
        {
            Debug.Log("No monsters loaded.");
            return;
        }
        foreach (Monster m in monsters)
        {
            Debug.Log(m.Name);
            foreach (CreatureEntity.Action a in m.Actions)
            {
                Debug.Log(a.actionName);
            }
        }
    }

    public static List<Character> GetCharacters() { return characters; }
    public static List<Obstacle> GetObstacles() { return obstacles; }
    public static List<Monster> GetMonsters() { return monsters; }
 
}
