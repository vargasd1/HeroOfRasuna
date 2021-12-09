using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToWallCollision : MonoBehaviour
{

    public Outline enemyOutline;
    public GameObject targetEnemy;
    private EnemyAI enemAI;
    private EnemyRangedAI enemRAI;
    private SuravAI boss;
    private bool doOnce = true;
    private List<WallFade> walls = new List<WallFade>();

    private void Start()
    {
        transform.rotation = Quaternion.Euler(-45, 225, 0);
        enemyOutline = targetEnemy.GetComponentInChildren<Outline>();
    }

    private void Update()
    {
        if (doOnce)
        {
            if (targetEnemy.GetComponent<EnemyAI>() != null) enemAI = targetEnemy.GetComponent<EnemyAI>();
            else if (targetEnemy.GetComponent<EnemyRangedAI>() != null) enemRAI = targetEnemy.GetComponent<EnemyRangedAI>();
            else if (targetEnemy.GetComponent<SuravAI>() != null) boss = targetEnemy.GetComponent<SuravAI>();
            doOnce = false;
        }

        transform.position = targetEnemy.transform.position + transform.forward * 4;

        if (enemAI != null)
        {
            if (enemAI.health <= 0)
            {
                foreach (WallFade w in walls)
                {
                    w.fadeOut = false;
                    w.fadeIn = true;
                }
                walls = null;
                enemyOutline.enabled = false;
                Destroy(gameObject);
            }

        }
        else if (enemRAI != null)
        {
            if (enemRAI.health <= 0)
            {
                foreach (WallFade w in walls)
                {
                    w.fadeOut = false;
                    w.fadeIn = true;
                }
                walls = null;
                enemyOutline.enabled = false;
                Destroy(gameObject);
            }
        }
        else if (boss != null)
        {
            if (boss.health <= 0)
            {
                foreach (WallFade w in walls)
                {
                    w.fadeOut = false;
                    w.fadeIn = true;
                }
                walls = null;
                enemyOutline.enabled = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If the object collided with is a wall start to fade it out and turn on enemy outline
        if (other.gameObject.tag == "Wall")
        {
            enemyOutline.enabled = true;
            other.gameObject.GetComponent<WallFade>().SetMaterialTransparent();
            walls.Add(other.gameObject.GetComponent<WallFade>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the object that is no longer collided with is a wall disable the enemy's outine and fade wall back in
        WallFade wall = other.gameObject.GetComponent<WallFade>();
        if (other.gameObject.tag == "Wall")
        {
            enemyOutline.enabled = false;
            wall.fadeOut = false;
            wall.fadeIn = true;
            walls.Remove(wall);
        }
    }
}
