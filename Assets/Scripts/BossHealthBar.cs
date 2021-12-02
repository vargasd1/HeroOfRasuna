using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//ATTACHED TO: Canvas - UI

public class BossHealthBar : MonoBehaviour
{
    public GameObject healthUI;
    public Image healthFill;
    SuravAI surav;
    //Color healthColor;
    float health = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "ThirdFloor")
        {
            surav = FindObjectOfType<SuravAI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(surav != null && surav.startFight)
        {
            if(surav.state == SuravAI.State.Defeated)
            {
                healthUI.SetActive(false);
            }
            else
            {
                healthUI.SetActive(true);
                health = surav.health / 800f;
                //healthColor = new Color(Mathf.Lerp(1f, 0f, (Mathf.Clamp(health, 0.5f, 1f) - .5f) / .5f), Mathf.Lerp(0f, 1f, Mathf.Clamp(health, 0f, .5f) / .5f), 0f);

                healthFill.fillAmount = Mathf.Clamp(health, 0f, 1f);
                //healthFill.color = healthColor;
            }
        }
    }
}
