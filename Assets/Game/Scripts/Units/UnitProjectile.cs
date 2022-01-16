using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] float launchForce = 10f;
    [SerializeField] float destroyTime = 5f;
    [SerializeField] int damageAmount = 50;
    private void Start()
    {
        rigidbody.velocity = transform.forward * launchForce;
        
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyTime);
    }
    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }
        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageAmount);
        }
        DestroySelf();
    }
}
