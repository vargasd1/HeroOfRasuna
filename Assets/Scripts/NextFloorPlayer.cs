using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextFloorPlayer
{
    public static float hp = 100f, healCooldown = 0f, attackSpellCooldown = 0f, starBurstCooldown = 0f, overclockFill = 0f;
    public static bool loadedValues = true;
    
    public static void SaveValues(GameObject player)
    {
        Debug.Log("Saving values from previous floor.");
        hp = player.GetComponent<PlayerManager>().playerTargetHealth;
        healCooldown = player.GetComponent<PlayerManager>().healCDTime;
        attackSpellCooldown = player.GetComponent<PlayerManager>().attackSpellCDTime;
        starBurstCooldown = player.GetComponent<PlayerManager>().stunCDTime;
        overclockFill = player.GetComponent<PlayerMovement>().overclockChargedAmt;
        loadedValues = false;
    }

    public static void FillValues(GameObject player)
    {
        Debug.Log("Filling values from previous floor.");
        player.GetComponent<PlayerManager>().playerTargetHealth = hp;
        player.GetComponent<PlayerManager>().healCDTime = healCooldown;
        player.GetComponent<PlayerManager>().attackSpellCDTime = attackSpellCooldown;
        player.GetComponent<PlayerManager>().stunCDTime = starBurstCooldown;
        player.GetComponent<PlayerMovement>().overclockChargedAmt = overclockFill;
        loadedValues = true;
    }

    public static void ResetValues()
    {
        hp = 100f;
        healCooldown = 0f;
        attackSpellCooldown = 0f;
        starBurstCooldown = 0f;
        overclockFill = 0f;
    }
}
