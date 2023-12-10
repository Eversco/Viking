using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public Transform orientation;
    public AnimationControl animationControl;

    private float lastAttackTime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        // Shoot a raycast attackRange distance from player, if it hits an enemy deal damage.
        RaycastHit hit;
        animationControl.TriggerAttack();

        if (Physics.Raycast(orientation.position, orientation.TransformDirection(Vector3.forward), out hit, attackRange))
        {
            Debug.DrawRay(orientation.position, orientation.TransformDirection(Vector3.forward) * attackRange, Color.cyan, 2f);

            // Add logic here to deal damage to the enemy
        }
        else
        {
            Debug.DrawRay(orientation.position, orientation.TransformDirection(Vector3.forward) * attackRange, Color.green, 2f);
        }
    }
}
