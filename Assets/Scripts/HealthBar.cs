using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ATTACHED TO: Canvas - UI

public class HealthBar : MonoBehaviour
{
    public PlayerManager player;
    //deplete rhombus1 -> rect1 -> rhombus2 -> etc
    public Image rect1, rect2, rect3, rect4, rect5, rect6, kite1, kite2, kite3, kite4, kite5, kite6, kite7;
    Color healthColor;
    Renderer colorRender;
    float health;
    float runningTotal = 0f;

    //https://www.youtube.com/watch?v=ZzkIn41DFFo&ab_channel=MousawiDev
    void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        health = player.playerHealth / 100f;
        runningTotal = 0f;
        //healthColor = new Color(Mathf.Lerp(1f, 0f, (Mathf.Clamp(health, 0.5f, 1f) - .5f) / .5f), Mathf.Lerp(0f, 1f, Mathf.Clamp(health, 0f, .5f) / .5f), 0f);
        //0-120/360, .75, .85

        healthColor = Color.HSVToRGB(Mathf.Lerp(0f, .33f, Mathf.Clamp(health, 0f, 1f)), .75f, .85f);

        /*
         * each rect is worth 13.84% of health
         * each kite is worth 2.83% of health. Radial fill from [0, .71], except for the first and last (which both simulate half of a kite)
         * kite1 is [0, .378] and kite7 is [0, .6]
         * lerps and clamps
         * Mathf.Clamp(health, runningTotal, runningTotal + X) gives the player's health in terms that each section (which holds some portion of health) can use.
         * (Mathf.Clamp() - runningTotal) / [section%] gives how much of a section should be filled
        */

        kite7.fillAmount = Mathf.Lerp(0f, 0.6f, Mathf.Clamp(health, runningTotal, runningTotal + 0.01415f) / 0.01415f);//kite7 is half of a full kite's percentage
        kite7.color = healthColor;
        runningTotal += 0.01415f;//0.01415

        rect6.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect6.color = healthColor;
        runningTotal += 0.1384f;//0.15255

        kite6.fillAmount = Mathf.Lerp(0f, 0.71f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.0283f) - runningTotal) / 0.0283f);
        kite6.color = healthColor;
        runningTotal += 0.0283f;//0.18085

        rect5.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect5.color = healthColor;
        runningTotal += 0.1384f;//.31925

        kite5.fillAmount = Mathf.Lerp(0f, 0.71f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.0283f) - runningTotal) / 0.0283f);
        kite5.color = healthColor;
        runningTotal += 0.0283f;//0.34755

        rect4.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect4.color = healthColor;
        runningTotal += 0.1384f;//0.48595

        kite4.fillAmount = Mathf.Lerp(0f, 0.71f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.0283f) - runningTotal) / 0.0283f);
        kite4.color = healthColor;
        runningTotal += 0.0283f;//0.51425

        rect3.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect3.color = healthColor;
        runningTotal += 0.1384f;//0.65265

        kite3.fillAmount = Mathf.Lerp(0f, 0.71f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.0283f) - runningTotal) / 0.0283f);
        kite3.color = healthColor;
        runningTotal += 0.0283f;//0.68095

        rect2.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect2.color = healthColor;
        runningTotal += 0.1384f;//0.81935

        kite2.fillAmount = Mathf.Lerp(0f, 0.71f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.0283f) - runningTotal) / 0.0283f);
        kite2.color = healthColor;
        runningTotal += 0.0283f;//0.84765

        rect1.fillAmount = (Mathf.Clamp(health, runningTotal, runningTotal + 0.1384f) - runningTotal) / 0.1384f;
        rect1.color = healthColor;
        runningTotal += 0.1384f;//0.98605

        kite1.fillAmount = Mathf.Lerp(0f, 0.378f, (Mathf.Clamp(health, runningTotal, runningTotal + 0.01415f) - runningTotal) / 0.01415f);
        kite1.color = healthColor;
    }
}
