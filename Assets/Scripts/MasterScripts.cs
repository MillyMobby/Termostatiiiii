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

    [SerializeField] GameObject draggablePrefab; 

    private GameObject dragPreview; // Preview object while dragging
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

                // Add EventTrigger for drag on button
                AddDragEventsToButton(button, monster);

               
                RectTransform rt = button.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(350, 80); // Force size
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

    private void AddDragEventsToButton(GameObject button, Monster monster)
    {
        // Add EventTrigger for drag
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.AddComponent<EventTrigger>();
        }

        // Begin Drag Event
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDragButton(monster, button); });
        trigger.triggers.Add(beginDragEntry);

        // Drag Event
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDragButton(data); });
        trigger.triggers.Add(dragEntry);

        // End Drag Event
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDragButton(data); });
        trigger.triggers.Add(endDragEntry);

        // Also keep the click functionality
        //button.GetComponent<Button>().onClick.AddListener(() => OnClickButton(monster));
    }


    // Replace your current OnBeginDragButton with this simpler version:
    void OnBeginDragButton(Monster monster, GameObject button)
    {
        if (draggablePrefab == null)
        {
            Debug.LogError("draggablePrefab is not assigned!");
            return;
        }

        // Get mouse position in world space at a visible z-position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from camera (in front of everything)

        // Convert to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Create the draggable at the mouse position
        GameObject draggableObj = Instantiate(draggablePrefab, worldPos, Quaternion.identity);

        // Get the DraggableAsset component
        DraggableAsset draggable = draggableObj.GetComponent<DraggableAsset>();

        if (draggable != null)
        {
            // Set the monster reference
            draggable.Monster = monster;

            // Force the z-position to be visible (in front of grid)
            draggableObj.transform.position = new Vector3(worldPos.x, worldPos.y, -5f); // Visible z-position

            // Add to manager
            Manager.Instance.AddDraggableAsset(draggable);

            // Set as the object being dragged
            dragPreview = draggableObj;

            Debug.Log($"Created draggable at {draggableObj.transform.position} for monster {monster.Name}");
        }
        else
        {
            Debug.LogError("Failed to get DraggableAsset component!");
            Destroy(draggableObj);
        }
    }
    void OnDragButton(BaseEventData data)
    {
        //if (dragPreview != null)
        //{
        //    PointerEventData pointerData = (PointerEventData)data;

        //    // Update position based on mouse
        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(pointerData.position);
        //    worldPos.z = -8;
        //    dragPreview.transform.position = worldPos;
        //}
    }

    void OnEndDragButton(BaseEventData data)
    {
            dragPreview = null;
           
        
        
    }
}
