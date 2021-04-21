using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeManager : MonoBehaviour
{
    public GameObject weapon;

    public bool startAttack = false;
    public bool isAttacking = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (startAttack == true)
        {
            isAttacking = true;
            SwingWeapon();
            startAttack = false;
        }
        // if isAttacking check sword hit box collision
    }

    void SwingWeapon()
    {
        print("swish");
        

        Vector3 euler2 = weapon.transform.localEulerAngles;

        weapon.transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f);



        isAttacking = false;
    }
}
