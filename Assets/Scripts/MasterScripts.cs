using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

//DA PULIRE E SISTEMARE MA FUNZIONA
public class MasterScripts : MonoBehaviour
{
    // UI menu
    [SerializeField] GameObject contentMaster;
    [SerializeField] ScrollRect scrollMaster;
    [SerializeField] TMP_Text monsterInfo;
    public GameObject buttonPrefab;
    [SerializeField] GameObject draggablePrefab;

    //from DB
    private List<Monster> listMonsters = new List<Monster>();

    // Press-and-hold variables
    private Dictionary<GameObject, Monster> buttonMonsterMap = new Dictionary<GameObject, Monster>();
    private Dictionary<GameObject, float> buttonPressStartTime = new Dictionary<GameObject, float>();
    private const float HOLD_DURATION = 0.5f; 
    private GameObject currentDraggingButton;

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

        buttonMonsterMap.Clear();
        buttonPressStartTime.Clear();

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
                GameObject button = Instantiate(buttonPrefab);
                button.transform.SetParent(contentMaster.transform, false);
                button.GetComponentInChildren<TextMeshProUGUI>().text = monster.Name;


                buttonMonsterMap[button] = monster;
                AddPressAndHoldEvents(button);

                // click for info display
                button.GetComponent<Button>().onClick.AddListener(() => OnClickButton(button));

                RectTransform rt = button.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(350, 80); //Settato manualmente perchè sono stupida
            }
            Canvas.ForceUpdateCanvases();
            scrollMaster.verticalNormalizedPosition = 1f;
        }
    }

    private void AddPressAndHoldEvents(GameObject button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.AddComponent<EventTrigger>();
        }

        // Pointer Down - Start tracking hold
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnButtonPointerDown(button, data); });
        trigger.triggers.Add(pointerDownEntry);

        // Pointer Up - Cancel hold
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { release(button, data); });
        trigger.triggers.Add(pointerUpEntry);

        // Pointer Exit - Cancel hold if mouse leaves button
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((data) => { release(button, data); });
        trigger.triggers.Add(pointerExitEntry);
    }

    void OnButtonPointerDown(GameObject button, BaseEventData data)
    {
        buttonPressStartTime[button] = Time.time;
        currentDraggingButton = button;

        StartCoroutine(CheckForHoldCompletion(button));

        Debug.Log("Button pressed down - starting hold timer");
    }

    void release (GameObject button, BaseEventData data) // your inibithions feel the rain on your skin
    {

        ResetButtonVisual(button);

        buttonPressStartTime.Remove(button);
        currentDraggingButton = null;
    }

    private IEnumerator CheckForHoldCompletion(GameObject button)
    {
        float startTime = Time.time;

        // Show initial visual feedback
        UpdateButtonVisual(button, 0f);

        while (buttonPressStartTime.ContainsKey(button) &&
               Time.time - startTime < HOLD_DURATION)
        {
            float progress = (Time.time - startTime) / HOLD_DURATION;

            UpdateButtonVisual(button, progress);
            yield return null;
        }

        if (buttonPressStartTime.ContainsKey(button))
        {
            // Complete the visual
            UpdateButtonVisual(button, 1f);
            yield return new WaitForSeconds(0.05f); 

            CreateAndStartDragging(button);
            buttonPressStartTime.Remove(button);

            ResetButtonVisual(button);
        }
        else
        {
            // Hold was cancelled
            ResetButtonVisual(button);
        }
    }

    private void UpdateButtonVisual(GameObject button, float progress) // slightly changes the appearence when holding the button
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            //  blueish
            buttonImage.color = Color.Lerp(Color.white, new Color(0.7f, 0.8f, 1f, 1f), progress);
        }

    }

    private void ResetButtonVisual(GameObject button)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white;
        }

        button.transform.localScale = Vector3.one;
    }

    private void CreateAndStartDragging(GameObject button)
    {
        if (!buttonMonsterMap.ContainsKey(button))
            return;

        Monster monster = buttonMonsterMap[button];

        if (draggablePrefab == null)
        {
            Debug.LogError("draggablePrefab is not assigned!");
            return;
        }

        // Get current mouse position in world space
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from camera

        // Convert screen position to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Create the draggable object
        GameObject draggableObj = Instantiate(draggablePrefab, worldPos, Quaternion.identity);

        // Set position (ensure it's visible)
        draggableObj.transform.position = new Vector3(worldPos.x, worldPos.y, -8f);

        // Get the DraggableAsset component
        DraggableAsset draggable = draggableObj.GetComponent<DraggableAsset>();

        if (draggable != null)
        {
            // Initialize with monster data
            draggable.Monster = monster;
            StartCoroutine(StartDraggingNextFrame(draggable));

            // Add to manager
            Manager.Instance.AddDraggableAsset(draggable);

            Debug.Log($"Created draggable for {monster.Name} and started dragging");
        }
        else
        {
            Debug.LogError("Failed to get DraggableAsset component!");
            Destroy(draggableObj);
        }
    }

    //sta roba è sus sono due coroutine innestate ma va bene così
    private IEnumerator StartDraggingNextFrame(DraggableAsset draggable)
    {
        // Wait one frame for everything to initialize
        yield return null;

        if (draggable.GetComponent<Collider2D>() != null)
        {
            // Set it as the currently dragged object
            draggable.gameObject.transform.position = GetCurrentMouseWorldPos();

            // Manually trigger OnMouseDown by sending a message (if collider exists)
            draggable.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        }

        StartCoroutine(UpdateDraggablePosition(draggable));
    }

    private IEnumerator UpdateDraggablePosition(DraggableAsset draggable)
    {
        // Keep updating position until mouse is released
        while (Input.GetMouseButton(0))
        {
            Vector3 mousePos = GetCurrentMouseWorldPos();
            draggable.transform.position = mousePos;
            yield return null;
        }

        // Mouse released - trigger the drop
        draggable.SendMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
    }

    private Vector3 GetCurrentMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = -8f; // le z sono a cazzo di cane ma per ora basta che sono davanti alla griglia ops
        return worldPos;
    }

    void OnClickButton(GameObject button)
    {
        if (buttonMonsterMap.ContainsKey(button))
        {
            Monster monster = buttonMonsterMap[button];
            monsterInfo.SetText("--TYPE--\n" + monster.Type + "\n--MAX PF--\n" + monster.Max_Pf +
                              "\n--AC--\n" + monster.AC + "\n--INITIATIVE--\n" + monster.Initiative +
                              "\n--CR--\n" + monster.CR);
        }
    }


}