using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // player variables
    public GameObject player;
    private Animator anim;

    // variables for melee attack
    public int attackNum = 0;
    //public bool isAttacking = false;
    bool canAttack;
    public bool alreadyHit = false;

    // variables for heal
    public ParticleSystem healParticles;
    public Image healthUI;
    public Image healCD;
    public float playerHealth = 100f;
    public float playerTargetHealth = 100f;
    float playerMaxHealth = 100f;
    float healAmount = 20f;
    float healCDTime = 0f;
    public bool isDead;

    // variables for attack spell
    public GameObject projectile;
    public Transform spellSpawnLoc;

    // variables for stun
    public GameObject stunPart;
    private float stunCDTime = 0f;
    private Vector3 pointToLook;
    private float rayLength;
    public Image stunCD;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        canAttack = true;
    }

    void Update()
    {
        // player dies
        if (playerHealth <= 0)
        {
            isDead = true;
            anim.SetTrigger("Died");
        }

        if (!isDead)
        {
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
                if (stunCDTime <= 0f)
                {
                    if (attackNum == 0) player.GetComponent<CharacterMovementIsometric>().lookAtMouse();
                    Stun();
                }
            }

            // spawn attack spell
            if (Input.GetKeyDown(KeyCode.Mouse1) && canAttack)
            {
                if (attackNum == 0) player.GetComponent<CharacterMovementIsometric>().lookAtMouse();
                StartCoroutine(spellAttack());
            }

            // swing attack
            if (Input.GetMouseButtonDown(0))
            {
                player.GetComponent<CharacterMovementIsometric>().isAttacking = true;
                if (attackNum == 0) player.GetComponent<CharacterMovementIsometric>().lookAtMouse();
                startCombo();
            }
        }
    }
    IEnumerator spellAttack()
    {
        yield return new WaitForSecondsRealtime(0.01f);

        GameObject lightBlast = Instantiate(projectile, spellSpawnLoc.position, player.transform.rotation, null) as GameObject;
        Rigidbody rb = lightBlast.GetComponent<Rigidbody>();
        rb.velocity = player.transform.forward * 20;
        lightBlast.GetComponent<SpellInteraction>().spellType = "attack";

        StopAllCoroutines();
    }

    void startCombo()
    {
        if (canAttack && attackNum == 0)
        {
            attackNum++;
        }
        else if (canAttack && attackNum == 1)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("initialSwing")) attackNum++;
        }
        else if (canAttack && attackNum == 2)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("secondSwingLonger")) attackNum++;
        }

        if (attackNum == 1)
        {
            anim.SetInteger("swingCount", 1);
        }

        StopAllCoroutines();
    }

    public void checkCombo()
    {
        canAttack = false;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("initialSwing") && attackNum == 1)
        {
            anim.SetInteger("swingCount", 0);
            canAttack = true;
            attackNum = 0;
            player.GetComponent<CharacterMovementIsometric>().isAttacking = false;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("initialSwing") && attackNum >= 2)
        {
            anim.SetInteger("swingCount", 2);
            canAttack = true;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("secondSwingLonger") && attackNum == 2)
        {
            anim.SetInteger("swingCount", 0);
            canAttack = true;
            attackNum = 0;
            player.GetComponent<CharacterMovementIsometric>().isAttacking = false;

        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("secondSwingLonger") && attackNum >= 3)
        {
            anim.SetInteger("swingCount", 3);
            canAttack = true;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("finalSwing"))
        {
            anim.SetInteger("swingCount", 0);
            canAttack = true;
            attackNum = 0;
            player.GetComponent<CharacterMovementIsometric>().isAttacking = false;
        }

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].alreadyHitByPlayer = false;
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
        pointToLook = new Vector3(pointToLook.x, pointToLook.y + .5f, pointToLook.z);
        GameObject stun = Instantiate(stunPart, pointToLook, Quaternion.Euler(-90, 0, 0)) as GameObject;
        stun.GetComponent<SpellInteraction>().spellType = "stun";
    }

    void Attack()
    {

    }
}
