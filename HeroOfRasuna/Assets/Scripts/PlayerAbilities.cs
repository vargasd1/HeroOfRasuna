using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public GameObject player;
    public ParticleSystem healParticles;
    public Image healthUI;
    public Image healCD;
    float playerHealth = 0f;
    float playerMaxHealth = 50f;
    float healAmount = 10f;
    float healCDTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<CharacterMovementIsometric>().health;
        playerMaxHealth = player.GetComponent<CharacterMovementIsometric>().maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //activate overclock
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (healCDTime <= 0f && playerHealth != playerMaxHealth) Heal();
        }
    }

    void FixedUpdate()
    {
        //update player UI
        playerHealth = Mathf.Clamp(playerHealth, 0f, playerMaxHealth);//prevents timeScale from going above 1/below 0
        healthUI.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 290, playerHealth / playerMaxHealth), 40);

        //edit the cooldown shadows/effects for abilities
        healCDTime -= Time.fixedUnscaledDeltaTime;
        if (healCDTime < 0f) healCDTime = 0f;
        healCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, healCDTime / 60f));
    }

    void Heal()
    {
        //heal player
        playerHealth += healAmount;
        Debug.Log(playerHealth);
        Instantiate(healParticles, player.gameObject.transform);
        healCDTime = 60f;
    }
}
