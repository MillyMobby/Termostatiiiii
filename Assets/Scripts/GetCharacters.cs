using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;


public class GetCharacters : MonoBehaviour
{
    [SerializeField] GameObject contentPlayer;
    [SerializeField] ScrollRect scrollPlayer;
    [SerializeField] TMP_Text statsCharacter;
    [SerializeField] TMP_Text charData;
    public GameObject buttonPrefab;
    private List<Character> characters = new List<Character>();
    void Start()
    {
        StartCoroutine(RequestCharacters());
    }

    [System.Serializable]
    public class Character
    {
        public int AC { get; set; }
        public int Initiative { get; set; }
        public int Pass_Perc { get; set; }
        public string Hit_Dice { get; set; }
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Con { get; set; }
        public int Int { get; set; }
        public int Wis { get; set; }
        public int Cha { get; set; }
        public int Curr_Pf { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public string Race { get; set; }
        public string Name { get; set; }
    }

    public IEnumerator RequestCharacters()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://o9wbc90xbl.execute-api.eu-north-1.amazonaws.com/characters");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Couldn't send GET request: " + www.error);
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
                    Curr_Pf = obj["Curr_Pf"].Value<int>()
                 };
                 characters.Add(character);
            }
            if (!characters.Any())
            {
                GameObject textObj = new GameObject("NoSheetsText");

                textObj.transform.SetParent(contentPlayer.transform, false);
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

                text.text = "There are no characters.";

                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 22;
            }
            else
            {
                Character result = characters.Find(x => x.Name == "Karina");
                charData.SetText("--NAME--\n"+result.Name + "\n--CURR PF--\n" + result.Curr_Pf + "\n--AC--\n" + result.AC + "\n--INITIATIVE--\n" + result.Initiative + "\n--CLASS--\n" + result.Class + "\n--Level--\n" + result.Level + "\n--RACE--\n" + result.Race + "\n--PASS PERC--\n" + result.Pass_Perc + "\n--HIT DICE--\n" + result.Hit_Dice);
                GameObject button = (GameObject)Instantiate(buttonPrefab);
                button.transform.SetParent(contentPlayer.transform, false);
                button.GetComponentInChildren<TextMeshProUGUI>().text = "Stats";
                button.GetComponent<Button>().onClick.AddListener(OnClick);
            }  
            Canvas.ForceUpdateCanvases();
            scrollPlayer.verticalNormalizedPosition = 1f;
        }
    }

    void OnClick()
    {
        var buttonClicked = EventSystem.current.currentSelectedGameObject;
        Character result = characters.Find(x => x.Name == "Karina");
        statsCharacter.SetText("--STR--\n"+result.Str + "\n--DEX--\n" + result.Dex + "\n--Con--\n" + result.Con + "\n--INT--\n" + result.Int + "\n--WIS--\n" + result.Wis + "\n--CHA--\n" + result.Cha);
    }
}
