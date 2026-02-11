using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UserPopup : MonoBehaviour
{
    private Character currentPlayer;
    private int color = 1; 

    public class Entity //bottone
    {
        public Sprite icon;
        public string name;

        public Entity(string iconPath, string name)
        {
            icon = Resources.Load<Sprite>(iconPath);
            this.name = name;
        }
    }

    [SerializeField] private Transform canvas;
    [SerializeField] private EntityPopup entityPrefab;
    private string path = "Sprites/Icons/enemyIcon"; //non so come michele ha creato le sprite in quel formato

    //[SerializeField] GameObject draggablePrefab;
    private List<Entity> allEntities = new List<Entity>();


    public async Task SetCurrentPlayerAsync()
    {
        currentPlayer = await Requester.RequestRandomCharacter();
        Debug.Log($"Current player set to: {currentPlayer?.Name}");

        List<CreatureEntity.Action> actions = await Requester.RequestActions(currentPlayer.Name);
        List<CreatureEntity.Action> spells = await Requester.RequestSpells(currentPlayer.Name);
        actions.AddRange(spells);
        currentPlayer.Actions = actions;
    }

    private async void Start()
    {
        if (currentPlayer == null) { await SetCurrentPlayerAsync(); }
        
            if (currentPlayer?.Actions != null)
            {
                foreach (CreatureEntity.Action m in currentPlayer.Actions)
                {
                    allEntities.Add(new Entity(path, m.actionName));
                }
                Debug.Log($"Added {allEntities.Count} entities from {currentPlayer.Actions.Count} actions");
            }        
                 
        GenerateList();
        MapManager.Instance.AssignCharacterInGrid(color, currentPlayer);
    }

    public void GenerateList()
    {
        Debug.Log("=== GENERATE LIST ===");

        if (currentPlayer == null || currentPlayer.Actions == null)
        {
            Debug.LogError("Cannot generate list - no player or actions!");
            return;
        }

        for (int i = 0; i < currentPlayer.Actions.Count; i++)
        {
            if (i >= allEntities.Count)
            {
                Debug.LogWarning($"Index {i} out of range for allEntities");
                continue;
            }

            EntityPopup newEntity = Instantiate(entityPrefab, canvas);
            newEntity.Init(newSprite: allEntities[i].icon, newName: allEntities[i].name);

            
            Button actionButton = newEntity.GetComponent<Button>();
            if (actionButton == null)
            {
                Debug.Log("Adding Button component to EntityPopup...");
                actionButton = newEntity.gameObject.AddComponent<Button>();

                Image buttonImage = newEntity.GetComponent<Image>();
                if (buttonImage != null)
                {
                    actionButton.targetGraphic = buttonImage;
                }


            }

            CreatureEntity.Action currentAction = currentPlayer.Actions[i];

            actionButton.onClick.AddListener(() =>
            {
                Debug.Log($"Action button clicked: {currentAction.actionName}");
                OnActionButtonClicked(currentAction);
            });

        }

        Debug.Log($"Created {currentPlayer.Actions.Count} action buttons");
    }

    void OnActionButtonClicked(CreatureEntity.Action action)
    {    
        Debug.Log($"Range: {action.range}\n");
        DisplayRange(action.range);
    }

    public void DisplayRange(int range) {
        if (range != 101) { MapManager.Instance.HighlightArea(color, range); }

    }
}