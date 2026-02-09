using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Requester
{
    public static async Task<List<Character>> RequestCharacters() {
        List<Character> characters = new List<Character>();
        UnityWebRequest www = UnityWebRequest.Get("https://o9wbc90xbl.execute-api.eu-north-1.amazonaws.com/characters");
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Couldn't send GET request: " + www.error);
            return characters;
        }
        else
        {
            var json = www.downloadHandler.text;
            JArray array = JArray.Parse(json);
            foreach (JObject obj in array.Children<JObject>())
            {
                var character = new Character
                {
                    Class = obj["Class"]?.ToString(),
                    Level = obj["Level"].Value<int>(),
                    Race = obj["Race"]?.ToString(),
                    Name = obj["Name"]?.ToString(),
                    AC = obj["AC"].Value<int>(),
                    Initiative = obj["Initiative"].Value<int>(),
                    Pass_Perc = obj["Pass_Perc"].Value<int>(),
                    Hit_Dice = obj["Hit_Dice"]?.ToString(),
                    Str = obj["Str"].Value<int>(),
                    Dex = obj["Dex"].Value<int>(),
                    Con = obj["Con"].Value<int>(),
                    Int = obj["Int"].Value<int>(),
                    Wis = obj["Wis"].Value<int>(),
                    Cha = obj["Cha"].Value<int>(),
                    Curr_Pf = obj["Curr_Pf"].Value<int>(),
                    Max_Pf = obj["Max_Pf"].Value<int>(),
                    Bio = obj["Bio"]?.ToString()
                 };
                 characters.Add(character);
            }
            return characters;
        }
    }


    public static async Task<List<Monster>> RequestMonsters() {
        List<Monster> listMonsters = new List<Monster>();
        UnityWebRequest www = UnityWebRequest.Get("https://o9wbc90xbl.execute-api.eu-north-1.amazonaws.com/monsters");
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Couldn't send GET request: " + www.error);
            return listMonsters;
        }
        else
        {
            var json = www.downloadHandler.text;
            JArray array = JArray.Parse(json);
            foreach (JObject obj in array.Children<JObject>())
            {
                var monster = new Monster
                {
                    Name = obj["Name"]?.ToString(),
                    CR = obj["CR"]?.Value<float?>(),
                    Type = obj["Type"]?.ToString(),
                    Max_Pf = obj["Max_Pf"].Value<int>(),
                    AC = obj["AC"].Value<int>(),
                    Initiative = obj["Initiative"].Value<int>(),
                    Bio = obj["Bio"]?.ToString()
                 };
                 listMonsters.Add(monster);
            }
            return listMonsters;
        }
    }


    public static async Task<List<Obstacle>> RequestObstacles() {
        List<Obstacle> listObstacles = new List<Obstacle>();
        UnityWebRequest www = UnityWebRequest.Get("https://o9wbc90xbl.execute-api.eu-north-1.amazonaws.com/obstacles");
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Couldn't send GET request: " + www.error);
            return listObstacles;
        }
        else
        {
            var json = www.downloadHandler.text;
            JArray array = JArray.Parse(json);
            foreach (JObject obj in array.Children<JObject>())
            {
                var obstacle = new Obstacle
                {
                    Name = obj["Name"]?.ToString(),
                    Bio = obj["Bio"]?.ToString(),
                    Max_Pf = obj["Max_Pf"].Value<int>(),
                    Curr_Pf = obj["Current_Pf"].Value<int>()
                 };
                 listObstacles.Add(obstacle);
            }
            return listObstacles;
        }
    } 


}