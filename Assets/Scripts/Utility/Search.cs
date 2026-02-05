using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class Search
{
    public static async Task<Monster> SearchMonster(string name)
    {
        List<Monster> listMonsters = await DBManager.RequestMonsters();
        return listMonsters.Find(x => x.Name == name);
    }

    public static async Task<Character> SearchCharacter(string name) 
    {
        List<Character> listCharacters = await DBManager.RequestCharacters();
        return listCharacters.Find(x => x.Name == name);
    }
}
