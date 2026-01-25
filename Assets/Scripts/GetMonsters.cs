using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;


public class GetMonsters : MonoBehaviour
{
    [SerializeField] GameObject contentMaster;
    [SerializeField] ScrollRect scrollMaster;
    [SerializeField] TMP_Text monsterInfo;
    public GameObject buttonPrefab;
    private List<Monster> listMonsters = new List<Monster>();
    void Start()
    {
        StartCoroutine(RequestMonsters());
    }

    [System.Serializable]
    class Monster
    {
        public string Name { get; set; }
        public int Max_Pf { get; set; }
        public int AC { get; set; }
        public int Initiative { get; set; }
        public float? CR { get; set; }
        public string Type { get; set; }
    }

    public IEnumerator RequestMonsters()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://o9wbc90xbl.execute-api.eu-north-1.amazonaws.com/monsters");
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
                var monster = new Monster
                {
                    Name = obj["Name"]?.ToString(),
                    CR = obj["CR"]?.Value<float?>(),
                    Type = obj["Type"]?.ToString(),
                    Max_Pf = obj["Max_Pf"].Value<int>(),
                    AC = obj["AC"].Value<int>(),
                    Initiative = obj["Initiative"].Value<int>()
                 };
                 listMonsters.Add(monster);
            }
            foreach (Transform child in contentMaster.transform)
            {
                Destroy(child.gameObject);
            }
            if (!listMonsters.Any())
            {
                GameObject textObj = new GameObject("NoSheetsText");

                textObj.transform.SetParent(contentMaster.transform, false);
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

                text.text = "There are no monsters.";

                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 22;
            }
            else
            {
                foreach (Monster monster in listMonsters)
                {
                    GameObject button = (GameObject)Instantiate(buttonPrefab);
                    button.transform.SetParent(contentMaster.transform, false);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = monster.Name;
                    button.GetComponent<Button>().onClick.AddListener(OnClick);
                }
            }  
            Canvas.ForceUpdateCanvases();
            scrollMaster.verticalNormalizedPosition = 1f;
        }
    }

    void OnClick()
    {
        var buttonClicked = EventSystem.current.currentSelectedGameObject;
        Monster result = listMonsters.Find(x => x.Name == buttonClicked.GetComponentInChildren<TextMeshProUGUI>().text);
        monsterInfo.SetText("--TYPE--\n"+result.Type + "\n--MAX PF--\n" + result.Max_Pf + "\n--AC--\n" + result.AC + "\n--INITIATIVE--\n" + result.Initiative + "\n--CR--\n" + result.CR);
    }
}
