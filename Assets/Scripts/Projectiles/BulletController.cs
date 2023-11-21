using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletController : NetworkBehaviour
{
    [SerializeField] GameObject hitEffect;
    public GameObject shooterGameObject; // Set from PlayerShooting class

    void Start()
    {
        if (!isServer) return;

        StartCoroutine(SelfDestruct());
    }

    [Server]
    IEnumerator SelfDestruct() 
    {
        yield return new WaitForSeconds(2f);
        NetworkServer.Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.gameObject == shooterGameObject) return;

        if (other.gameObject.tag == "Player") 
        {
            RpcShowHitEffect(transform.position);
        
            PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
            PlayerShooting shooter = shooterGameObject.GetComponent<PlayerShooting>();

            health.ServerTakeDamage(50);
            shooter.ServerOnPlayerHit(other.gameObject);

            NetworkServer.Destroy(gameObject);
        }
    }

    [ClientRpc]
    void RpcShowHitEffect(Vector2 hitPosition)
    {
        Instantiate(hitEffect, hitPosition, Quaternion.identity);
    }

    
}
