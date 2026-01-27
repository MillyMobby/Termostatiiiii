using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MasterScripts : MonoBehaviour
{
    [SerializeField] GameObject contentMaster;
    [SerializeField] ScrollRect scrollMaster;
    [SerializeField] TMP_Text monsterInfo;
    public GameObject buttonPrefab;
    private List<Monster> listMonsters = new List<Monster>();

    async void Start()
    {
        await PopulateMasterScroll();
    }

    public async Task PopulateMasterScroll()
    {
        listMonsters = await DBManager.RequestMonsters();
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
