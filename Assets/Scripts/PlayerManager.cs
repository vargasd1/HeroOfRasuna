using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // player variables
    public GameObject player;
    public Animator anim;
    public PlayerMovement playerMove;
    public Transform stunSpawnLoc;

    public bool isCutScene = false;

    // variables for melee attack
    public int attackNum = 0;
    public bool canAttack;
    public bool isInvinc = false;
    public float invincTimer = 0;

    // variables for heal
    public ParticleSystem healParticles;
    public Image healthUI;
    public Image healCD;
    public float playerHealth = 100f;
    public float playerTargetHealth = 100f;
    float playerMaxHealth = 100f;
    float healAmount = 25f;
    float healCDTime = 0f;
    public bool isDead;

    // variables for attack spell
    public GameObject projectile;
    public Transform spellSpawnLoc;
    private float attackSpellCDTime = 0f;
    public Vector3 normalForward;
    public Quaternion normalRotation;

    // variables for stun
    public GameObject stunGren;
    private float stunCDTime = 0f;
    private Vector3 pointToLook;
    private float rayLength;
    public Image stunCD;
    public LayerMask floorMask;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        playerMove = gameObject.GetComponent<PlayerMovement>();
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

        isCutScene = playerMove.isCutScene;

        if (!isDead && !pauseMenu.GamePaused && !isCutScene)
        {
            // Lerps health
            playerHealth = AnimMath.Lerp(playerHealth, playerTargetHealth, 0.05f);
            if (playerHealth > 99.8f) playerHealth = 100;

            // Lowers invinc tiemr
            if (isInvinc) invincTimer -= Time.unscaledDeltaTime;

            // If timer = 0, invinc gone
            if (invincTimer <= 0)
            {
                invincTimer = 0;
                isInvinc = false;
            }

            if (!playerMove.isAttacking)
            {
                // activate heal
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (healCDTime <= 0f && playerHealth != playerMaxHealth)
                    {
                        FindObjectOfType<AudioManager>().PlayUninterrupted("Heal");
                        canAttack = false;
                        playerMove.isAttacking = true;
                        anim.SetTrigger("castHeal");
                        Heal();
                    }
                }

                // activate stun spell
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    RaycastHit hit;
                    Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(cameraRay, out hit, 1000, floorMask))
                    {
                        if (hit.transform.gameObject.layer == 9)
                        {
                            if (stunCDTime <= 0f) Stun();
                        }
                    }
                }

                // spawn attack spell
                if (Input.GetKeyDown(KeyCode.Mouse1) && canAttack)
                {
                    if (attackSpellCDTime <= 0f)
                    {
                        canAttack = false;
                        playerMove.isAttacking = true;
                        if (attackNum == 0) playerMove.lookAtMouse();
                        anim.SetTrigger("castAttack");
                    }
                }

                // swing attack
                if (Input.GetMouseButtonDown(0))
                {
                    if (attackNum == 0) playerMove.lookAtMouse();
                    startCombo();
                }
            }
        }
    }

    void startCombo()
    {
        if (canAttack && attackNum == 0)
        {
            attackNum++;
            FindObjectOfType<AudioManager>().PlayUninterrupted("Hit 1");
        }
        else if (canAttack && attackNum == 1)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("initialSwing"))
            {
                attackNum++;
                FindObjectOfType<AudioManager>().PlayUninterrupted("Hit 2 (clip 3)");
            }
        }
        else if (canAttack && attackNum == 2)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("secondSwingLonger"))
            {
                attackNum++;
                FindObjectOfType<AudioManager>().PlayInSeconds("Hit 3 (clip 2)", 0.25f);
            }
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
        }
    }

    void FixedUpdate()
    {
        if (!isDead || !pauseMenu.GamePaused || !isCutScene)
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

            attackSpellCDTime -= Time.fixedUnscaledDeltaTime;
            if (stunCDTime < 0f) stunCDTime = 0f;
        }
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
        RaycastHit hit;
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(cameraRay, out rayLength)) pointToLook = cameraRay.GetPoint(rayLength);


        Vector3 startPos = new Vector3(player.transform.position.x, player.transform.position.y + 4, player.transform.position.z);

        GameObject stun = Instantiate(stunGren, startPos, Quaternion.identity) as GameObject;
        SpellInteraction stunSpell = stun.GetComponent<SpellInteraction>();
        stunSpell.spellType = "grenade";
        stunSpell.positionB = pointToLook;
        stunSpell.positionA = stunSpawnLoc.position;

        stunSpell.handle = stunSpell.positionA + (stunSpell.positionB - stunSpell.positionA) / 2;
        stunSpell.handle += new Vector3(0, 5, 0);

        stunCDTime = 30f;
    }

    public void spellAttack()
    {
        // Finding location of click
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(cameraRay, out rayLength)) pointToLook = cameraRay.GetPoint(rayLength);

        // reset attack CD
        attackSpellCDTime = 10f;

        // Spawn prefab
        GameObject lightBlast = Instantiate(projectile, spellSpawnLoc.position, Quaternion.identity, null) as GameObject;

        // make y same height, so it doesn't fall up or down
        pointToLook = new Vector3(pointToLook.x, spellSpawnLoc.position.y, pointToLook.z);

        // make lightBlast prefab rotate towards click
        lightBlast.transform.LookAt(pointToLook);
        // addForce in the forward direction so the lightBlast moved towards click
        lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 20);

        // set spell type
        lightBlast.GetComponent<SpellInteraction>().spellType = "attack";
    }
}
