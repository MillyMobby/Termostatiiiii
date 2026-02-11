using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text bioText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text classLevelText;

    private RectTransform rectTransform;
    private Canvas canvas;
    private float displayDuration = 3f;
    private float currentTimer = 0f;
    private bool isVisible = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();

        // Configure the canvas
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Force overlay mode for simplicity
            canvas.sortingOrder = 999; // Make sure it renders on top
        }

        // Make sure the GameObject is active
        gameObject.SetActive(true);
    }

    void Start()
    {
        // Force a canvas update
        Canvas.ForceUpdateCanvases();
    }

    void Update()
    {
        if (isVisible)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ShowDisplay(Character character, Vector2 screenPosition)
    {
        if (character == null) return;

        Debug.Log($"=== SHOWING DISPLAY FOR {character.Name} ===");
        Debug.Log($"Screen position: {screenPosition}");

        // Update text fields
        if (nameText != null)
        {
            nameText.text = character.Name ?? "Unknown";
            nameText.color = Color.white;
            Debug.Log($"Name text set to: {nameText.text}");
        }

        if (hpText != null)
        {
            hpText.text = $"HP: {character.Curr_Pf}/{character.Max_Pf}";
            Debug.Log($"HP text set to: {hpText.text}");
        }

        if (bioText != null)
            bioText.text = character.Bio ?? "No description available";

        if (classLevelText != null)
            classLevelText.text = $"{character.Class} Lvl.{character.Level} | {character.Race}";

        if (statsText != null)
        {
            statsText.text = $"STR:{character.Str} DEX:{character.Dex} CON:{character.Con}\n" +
                           $"INT:{character.Int} WIS:{character.Wis} CHA:{character.Cha}";
        }

        // Force text to update
        if (nameText != null) nameText.ForceMeshUpdate();
        if (hpText != null) hpText.ForceMeshUpdate();
        if (bioText != null) bioText.ForceMeshUpdate();
        if (classLevelText != null) classLevelText.ForceMeshUpdate();
        if (statsText != null) statsText.ForceMeshUpdate();

        // Position the display
        PositionDisplay(screenPosition);

        // Force the background to be visible
        Transform background = transform.Find("Background");
        if (background != null)
        {
            Image bgImage = background.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0, 0, 0, 0.8f);
                bgImage.enabled = true;
                Debug.Log("Background enabled");
            }
        }

        // Make sure everything is active
        gameObject.SetActive(true);

        // Force canvas update
        Canvas.ForceUpdateCanvases();

        isVisible = true;
        currentTimer = displayDuration;

        Debug.Log($"Display shown at position: {rectTransform.position}");
    }

    private void PositionDisplay(Vector2 screenPosition)
    {
        if (rectTransform == null) return;

        // For ScreenSpaceOverlay, we can position directly in screen space
        rectTransform.position = screenPosition + new Vector2(100, 100);

        // Keep on screen
        Vector3 pos = rectTransform.position;
        float width = rectTransform.rect.width * rectTransform.lossyScale.x;
        float height = rectTransform.rect.height * rectTransform.lossyScale.y;

        pos.x = Mathf.Clamp(pos.x, width / 2, Screen.width - width / 2);
        pos.y = Mathf.Clamp(pos.y, height / 2, Screen.height - height / 2);

        rectTransform.position = pos;

        Debug.Log($"Positioned display at: {rectTransform.position}");
        Debug.Log($"Display size: {width}x{height}");
    }

    public void HideDisplay()
    {
        isVisible = false;
        Destroy(gameObject);
    }

    public void SetDisplayDuration(float duration)
    {
        displayDuration = duration;
    }
}