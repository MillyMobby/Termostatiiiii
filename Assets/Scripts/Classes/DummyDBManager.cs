using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


public class DummyDBManager
{
    public static async Task<List<Monster>> RequestMonsters()
    {
        await Task.Delay(100); // fake async delay

        return new List<Monster>
        {
            new Monster
            {
                Name = "Goblin",
                Max_Pf = 7,
                AC = 13,
                Initiative = 2,
                CR = 0.25f,
                Type = "Humanoid"
            },
            new Monster
            {
                Name = "Orc",
                Max_Pf = 15,
                AC = 13,
                Initiative = 1,
                CR = 0.5f,
                Type = "Humanoid"
            }
        };
    }

    public static async Task<List<Obstacle>> RequestObstacles()
    {
        await Task.Delay(100);

        return new List<Obstacle>
        {
            new Obstacle
            {
                Name = "Crate",
                Description = "Wooden crate",
                HP = 10
            },
            new Obstacle
            {
                Name = "Stone Wall",
                Description = "Heavy stone wall",
                HP = 50
            }
        };
    }
}

