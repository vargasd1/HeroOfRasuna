using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;

    // variables for heal
    public ParticleSystem healParticles;
    public Image healthUI;
    public Image healCD;
    public float playerHealth = 100f;
    public float playerTargetHealth = 100f;
    float playerMaxHealth = 100f;
    float healAmount = 20f;
    float healCDTime = 0f;

    // variables for attack spell
    public GameObject projectile;
    public Transform spellSpawnLoc;

    // variables for stun
    public GameObject stunPart;
    private float stunCDTime = 0f;
    private Vector3 pointToLook;
    private float rayLength;
    public Image stunCD;

    void Start() { }

    void Update()
    {
        // player dies
        if (playerHealth <= 0) Destroy(gameObject);

        // Lerps health
        if (playerTargetHealth < playerHealth) playerHealth--;
        else if (playerTargetHealth > playerHealth) playerHealth++;
        else playerHealth = playerTargetHealth;

        // activate heal
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (healCDTime <= 0f && playerHealth != playerMaxHealth) Heal();
        }

        // activate stun spell
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (stunCDTime <= 0f) Stun();
        }

        // spawn attack spell
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameObject lightBlast = Instantiate(projectile, spellSpawnLoc.position, player.transform.rotation, null) as GameObject;
            Rigidbody rb = lightBlast.GetComponent<Rigidbody>();
            rb.velocity = player.transform.forward * 20;
            lightBlast.GetComponent<SpellInteraction>().spellType = "attack";
        }
    }

    void FixedUpdate()
    {
        // update player UI
        playerHealth = Mathf.Clamp(playerHealth, 0f, playerMaxHealth);//prevents timeScale from going above 1/below 0
        playerTargetHealth = Mathf.Clamp(playerTargetHealth, 0f, playerMaxHealth);
        healthUI.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 290, playerHealth / playerMaxHealth), 40);

        // edit the cooldown shadows/effects for abilities
        healCDTime -= Time.fixedUnscaledDeltaTime;
        if (healCDTime < 0f) healCDTime = 0f;
        healCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, healCDTime / 30f));

        // decrement stun time and edit the CD shadows/effect for ability
        stunCDTime -= Time.fixedUnscaledDeltaTime;
        if (stunCDTime < 0f) stunCDTime = 0f;
        stunCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, stunCDTime / 30f));
    }

    void Heal()
    {
        // heal player
        playerTargetHealth += healAmount;
        Instantiate(healParticles, player.gameObject.transform.position, Quaternion.Euler(-90, 0, 0));
        healCDTime = 30f;
    }

    void Stun()
    {
        // Finding location so spawn stun
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(cameraRay, out rayLength)) pointToLook = cameraRay.GetPoint(rayLength);

        // spawning the stun and resetting CD
        stunCDTime = 30f;
        GameObject stun = Instantiate(stunPart, pointToLook, Quaternion.identity) as GameObject;
        stun.GetComponent<SpellInteraction>().spellType = "stun";
    }
}
