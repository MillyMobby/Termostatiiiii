using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScripts : MonoBehaviour
{
    [SerializeField] GameObject contentPlayer;
    [SerializeField] ScrollRect scrollPlayer;
    [SerializeField] TMP_Text statsCharacter;
    [SerializeField] TMP_Text charData;
    [SerializeField] GameObject myButton;
    [SerializeField] GameObject otherButton;
    [SerializeField] Toggle statsToggle;
    [SerializeField] Toggle playerToggle;
    private string characterName;
    private Character c;

    async void Start()
    {
        characterName = PlayerPrefs.GetString("character");
        await PopulateSheetScroll();
    }

    public async Task PopulateSheetScroll()
    {
        List<Character> listCharacters = await DBManager.RequestCharacters();
        c = listCharacters.Find(x => x.Name == characterName);

        if (c == null)
        {
            GameObject textObj = new GameObject("NoCharacterText");

            textObj.transform.SetParent(contentPlayer.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

            text.text = "You do not have a character.";

            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 22;
        }
        else
        {
            myButton.GetComponentInChildren<TextMeshProUGUI>().text = characterName;
            otherButton.GetComponentInChildren<TextMeshProUGUI>().text = listCharacters.Find(x => x.Name != characterName).Name;  //da rivedere
            charData.SetText("--NAME--\n"+c.Name + "\n--CURR PF--\n" + c.Curr_Pf + "\n--AC--\n" + c.AC + "\n--INITIATIVE--\n" + c.Initiative + "\n--CLASS--\n" + c.Class + "\n--Level--\n" + c.Level + "\n--RACE--\n" + c.Race + "\n--PASS PERC--\n" + c.Pass_Perc + "\n--HIT DICE--\n" + c.Hit_Dice);
            statsToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate{OnValueChangedT2(statsToggle);});
            playerToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate{OnValueChangedT1(playerToggle);});
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentPlayer.GetComponent<RectTransform>());
        }  
        Canvas.ForceUpdateCanvases();
        scrollPlayer.verticalNormalizedPosition = 1f;
    }

    void OnValueChangedT1(Toggle toggle)
    {

        if (!toggle.isOn)
        {
            statsToggle.isOn = false;
        }
    }

    void OnValueChangedT2(Toggle toggle)
    {

        if (toggle.isOn)
        {
            statsCharacter.SetText("--STR--\n"+c.Str + "\n--DEX--\n" + c.Dex + "\n--Con--\n" + c.Con + "\n--INT--\n" + c.Int + "\n--WIS--\n" + c.Wis + "\n--CHA--\n" + c.Cha);
        }
        else 
        {
            statsCharacter.SetText("");
        }
    }
}
