using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conditions : MonoBehaviour
{
    public void HealthCheck(int aValue)
    {
        if (!(References.SendingAbilitySystem.GetComponent<StatSystem>().GetValue(StatSystem.StatType.Health) >= aValue))
        {
            References.SendingAbilitySystem.failedChecks = true;
        }
    }
    public void GoldCheck(int aValue)
    {
        if (!(References.ClientStatSystem.GetValue(StatSystem.StatType.Gold) >= aValue))
        {
            References.SendingAbilitySystem.failedChecks = true;
        }
    }
}
