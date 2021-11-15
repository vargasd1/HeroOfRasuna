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
    public int enemiesKilled = 0;

    // variables for melee attack (Left Click)
    public int attackNum = 0;
    public bool canAttack;
    public bool isInvinc = false;
    public float invincTimer = 0;
    public bool isCheckingForClick = false;

    // variables for heal (E)
    public ParticleSystem healParticles;
    //public Image healthUI;
    public Image healCD;
    public float playerHealth = 100f;
    public float playerTargetHealth = 100f;
    float playerMaxHealth = 100f;
    float healAmount = 25f;
    float healCDTime = 0f;
    public bool isDead;

    // variables for attack spell (Right Click)
    public GameObject projectile;
    public Transform spellSpawnLoc;
    public Image attackSpellCD;
    private float attackSpellCDTime = 0f;
    public Vector3 normalForward;
    public Quaternion normalRotation;

    // variables for stun (Q)
    public GameObject stunGren;
    private float stunCDTime = 0f;
    private Vector3 pointToLook;
    private float rayLength;
    public Image stunCD;
    public LayerMask floorMask;

    //audioManager
    private AudioManager audioScript;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        playerMove = gameObject.GetComponent<PlayerMovement>();
        audioScript = FindObjectOfType<AudioManager>();
        canAttack = true;

        // Ignore Collision Boxes/Spheres/Etc. of specified Layers
        Physics.IgnoreLayerCollision(10, 11); // Props ignore XP
        Physics.IgnoreLayerCollision(10, 10); // Props ignore other Props
        Physics.IgnoreLayerCollision(10, 15); // Props ignore PropFrac

        Physics.IgnoreLayerCollision(11, 12); // XP ignores Player
        Physics.IgnoreLayerCollision(11, 11); // XP ignores other XP
        Physics.IgnoreLayerCollision(11, 15); // XP ignores PropFrac

        Physics.IgnoreLayerCollision(15, 15); // PropFrac ignore other PropFrac
        Physics.IgnoreLayerCollision(15, 12); // PropFrac ignore Player
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

            if (!playerMove.isAttacking && !playerMove.isCasting)
            {
                // activate heal
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (healCDTime <= 0f && playerHealth != playerMaxHealth)
                    {
                        audioScript.PlayUninterrupted("Heal");
                        Heal();
                        canAttack = false;
                        playerMove.isCasting = true;
                        anim.SetTrigger("castHeal");
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
                        playerMove.isCasting = true;
                        if (attackNum == 0) playerMove.lookAtMouse();
                        anim.SetTrigger("castAttack");
                    }
                }

                // swing attack
                if (Input.GetMouseButtonDown(0))
                {
                    if (canAttack && attackNum == 0)
                    {
                        playerMove.lookAtMouse();
                        attackNum++;
                        anim.SetInteger("swingCount", attackNum);
                        audioScript.PlayUninterrupted("Hit 1");
                    }
                    else if (attackNum > 0 && isCheckingForClick && canAttack)
                    {
                        attackNum++;
                        isCheckingForClick = false;
                        anim.SetInteger("swingCount", attackNum);
                    }
                }
            }
        }
    }

    public void checkCombo()
    {
        isCheckingForClick = true;
    }

    void FixedUpdate()
    {
        if (!isDead || !pauseMenu.GamePaused || !isCutScene)
        {
            // update player UI
            playerHealth = Mathf.Clamp(playerHealth, 0f, playerMaxHealth);//prevents timeScale from going above 1/below 0
            playerTargetHealth = Mathf.Clamp(playerTargetHealth, 0f, playerMaxHealth);
            //healthUI.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, 290, playerHealth / playerMaxHealth), 40);

            // edit the cooldown shadows/effects for abilities
            healCDTime -= Time.fixedUnscaledDeltaTime;
            if (healCDTime < 0f) healCDTime = 0f;
            //healCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, healCDTime / 30f));
            healCD.fillAmount = Mathf.Clamp(healCDTime / 30f, 0f, 1f);

            // decrement stun time and edit the CD shadows/effect for ability
            stunCDTime -= Time.fixedUnscaledDeltaTime;
            if (stunCDTime < 0f) stunCDTime = 0f;
            //stunCD.rectTransform.sizeDelta = new Vector2(70, Mathf.Lerp(0, 70, stunCDTime / 30f));
            stunCD.fillAmount = Mathf.Clamp(stunCDTime / 30f, 0f, 1f);

            attackSpellCDTime -= Time.fixedUnscaledDeltaTime;
            //if (stunCDTime < 0f) stunCDTime = 0f;
            if (attackSpellCDTime < 0f) attackSpellCDTime = 0f;
            attackSpellCD.fillAmount = Mathf.Clamp(attackSpellCDTime / 10f, 0f, 1f);
        }
    }

    void Heal()
    {
        // heal player
        playerTargetHealth += healAmount;
        Instantiate(healParticles, player.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        healCDTime = 30f;
    }
    void Stun()
    {
        // Finding location so spawn stun
        //RaycastHit hit;
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
        GameObject lightBlast = Instantiate(projectile, spellSpawnLoc.position, Quaternion.Euler(0, -90, 0), null) as GameObject;

        // make y same height, so it doesn't fall up or down
        pointToLook = new Vector3(pointToLook.x, spellSpawnLoc.position.y, pointToLook.z);

        // make lightBlast prefab rotate towards click
        lightBlast.transform.LookAt(pointToLook);
        // addForce in the forward direction so the lightBlast moved towards click
        //lightBlast.GetComponent<Rigidbody>().AddForce(lightBlast.transform.forward * 20);
        lightBlast.GetComponent<Rigidbody>().AddForce(transform.forward * 20);

        // set spell type
        lightBlast.GetComponent<SpellInteraction>().spellType = "attack";
    }
}
