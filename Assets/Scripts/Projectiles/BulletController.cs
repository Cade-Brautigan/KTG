using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletController : NetworkBehaviour
{
    [SerializeField] GameObject hitEffect;
    public PlayerShooting shooter; // Is set from PlayerShooting class

    void Start()
    {
        if (!isServer) return;

        StartCoroutine(SelfDestruct());
    }

    [Server]
    IEnumerator SelfDestruct() {
        yield return new WaitForSeconds(2f);
        NetworkServer.Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (!isServer) return;

        if (other.gameObject.tag == "Player") 
        {
            Debug.Log("Bullet collision detected");
            RpcShowHitEffect(transform.position);
            PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
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
