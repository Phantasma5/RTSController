using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour
{
    [SerializeField] private Image healthbar;
    private StatSystem myStatSystem;
    private void Start()
    {
        myStatSystem = GetComponent<StatSystem>();
        myStatSystem.AddCallback(StatSystem.StatType.Health, UpdateHealthbar);
        myStatSystem.RunCallback(StatSystem.StatType.Health);
    }
    private void UpdateHealthbar(StatSystem.StatType aStat, float aValue, float aMaxValue)
    {
        Vector3 sca = healthbar.transform.localScale;
        sca.x = aValue / aMaxValue;
        healthbar.transform.localScale = sca;
    }
}
