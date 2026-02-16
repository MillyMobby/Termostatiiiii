using UnityEngine;
using UnityEngine.UI;
using System;

public class TurnsControllerSimple : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;

    public enum Turn
    {
        Kabo,
        Karina,
        Cheese
    }

    public TurnSelected OnTurnSelected;

    private void Start()
    {
        if (toggleGroup == null)
            toggleGroup = GetComponentInChildren<ToggleGroup>();

        Toggle[] toggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in toggles)
        {
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener((isOn) => OnAnyToggleChanged(toggle, isOn));
        }
    }

    private void OnAnyToggleChanged(Toggle changedToggle, bool isOn)
    {
        if (isOn)
        {
            Text textComponent = changedToggle.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                Turn selectedTurn;
                if (Enum.TryParse(textComponent.text, out selectedTurn))
                {
                    OnTurnSelected?.Invoke(selectedTurn);
                    Debug.Log($"Turn selected: {selectedTurn}");
                }
            }
        }
    }

    public Turn GetCurrentTurn()
    {
        Toggle activeToggle = toggleGroup.GetFirstActiveToggle();
        if (activeToggle != null)
        {
            Text textComponent = activeToggle.GetComponentInChildren<Text>();
            if (textComponent != null && Enum.TryParse(textComponent.text, out Turn currentTurn))
            {
                return currentTurn;
            }
        }

        return Turn.Kabo; // Default
    }

    [Serializable]
    public class TurnSelected : UnityEngine.Events.UnityEvent<Turn> { }
}