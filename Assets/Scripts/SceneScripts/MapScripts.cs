using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapScripts : MonoBehaviour
{
    [SerializeField] GameObject entitiesContent;
    [SerializeField] ScrollRect entitiesScroll;
    private List<Action> listActions = new List<Action>();
    private string name;

    public async void OnClick()
    {
        foreach (Transform child in entitiesContent.transform)
        {
            Destroy(child.gameObject);
        }
        name = PlayerPrefs.GetString("character");
        var buttonClicked = EventSystem.current.currentSelectedGameObject;
        string label = buttonClicked.GetComponentInChildren<TextMeshProUGUI>().text;
        if (label == name)
        {
            _ = PopulateActionScroll(name);
        }
        else if (label== "Beholder")
        {
            Monster m = await Search.SearchMonster(label);
            GameObject text = new GameObject("MonsterData");
            text.transform.SetParent(entitiesContent.transform, false);
            TextMeshProUGUI textText = text.AddComponent<TextMeshProUGUI>();
            textText.text = "--TYPE--\n"+m.Type + "\n--MAX PF--\n" + m.Max_Pf + "\n--AC--\n" + m.AC + "\n--INITIATIVE--\n" + m.Initiative + "\n--CR--\n" + m.CR;
            textText.alignment = TextAlignmentOptions.Center;
            textText.fontSize = 22;
            Canvas.ForceUpdateCanvases();
            entitiesScroll.verticalNormalizedPosition = 1f;
        }
        else
        {
            Character c = await Search.SearchCharacter(label);
            GameObject text = new GameObject("CharData");
            text.transform.SetParent(entitiesContent.transform, false);
            TextMeshProUGUI textText = text.AddComponent<TextMeshProUGUI>();
            textText.text = "--NAME--\n"+c.Name + "\n--CURR PF--\n" + c.Curr_Pf + "\n--AC--\n" + c.AC + "\n--INITIATIVE--\n" + c.Initiative + "\n--CLASS--\n" + c.Class + "\n--Level--\n" + c.Level + "\n--RACE--\n" + c.Race + "\n--PASS PERC--\n" + c.Pass_Perc;
            textText.alignment = TextAlignmentOptions.Center;
            textText.fontSize = 22;
            Canvas.ForceUpdateCanvases();
            entitiesScroll.verticalNormalizedPosition = 1f;
        }
    }

    public async Task PopulateActionScroll(string name)
    {
        listActions = await DBManager.RequestActions(name);
        if (!listActions.Any())
        {
            GameObject textObj = new GameObject("NoActionsText");

            textObj.transform.SetParent(entitiesContent.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

            text.text = "There are no actions.";

            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 22;
        }
        else
        {
            foreach (Action action in listActions)
            {
                GameObject textName = new GameObject("ActionName");
                GameObject textDescription = new GameObject("ActionDescription");
                textName.transform.SetParent(entitiesContent.transform, false);
                textDescription.transform.SetParent(entitiesContent.transform, false);
                TextMeshProUGUI textNameText = textName.AddComponent<TextMeshProUGUI>();
                TextMeshProUGUI textDescriptionText = textDescription.AddComponent<TextMeshProUGUI>();
                textNameText.text = action.Name;
                textDescriptionText.text = action.Description;
                textNameText.alignment = TextAlignmentOptions.Center;
                textNameText.fontSize = 22;
                textDescriptionText.alignment = TextAlignmentOptions.Center;
                textDescriptionText.fontSize = 22;
            }
            Canvas.ForceUpdateCanvases();
            entitiesScroll.verticalNormalizedPosition = 1f;
        }  
    }
}
