using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] Targeter targeter;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] float fireRange = 5f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] Animator animator;

    private float lastFireTime = 0;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target == null) return;

        if (!CanFireAtTarget()) return;
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1/fireRate) + lastFireTime)
        {
            Quaternion projectialRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position) ;
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectialRotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            animator.ResetTrigger("walk");
            animator.SetTrigger("fire");

            lastFireTime = Time.time;
        }

    }

    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }
}
