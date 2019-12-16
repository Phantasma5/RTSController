using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField] public RectTransform selectionBox;
    [SerializeField] public Text goldDisplay;
    private void Start()
    {
        References.ClientStatSystem.AddCallback(StatSystem.StatType.Gold, UpdateGoldDisplay);
        References.ClientStatSystem.RunCallback(StatSystem.StatType.Gold);
    }
    private void UpdateGoldDisplay(StatSystem.StatType aType, float aValue, float aMaxValue)
    {
        goldDisplay.text = aValue.ToString();
    }
}
