using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public void LowerGold(int aValue)
    {
        References.ClientStatSystem.AddValue(StatSystem.StatType.Gold, -aValue);
    }
    public void SpawnPrefab(string aName)
    {
        GameObject temp = References.ClientNet.Instantiate(aName, References.SendingAbilitySystem.gameObject.transform.position, Quaternion.identity);
        temp.GetComponent<NetworkSync>().AddToArea(1);
    }
}
