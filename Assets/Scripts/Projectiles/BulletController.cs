using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] GameObject hitEffect;
    public PlayerShooting shooter; // is set from PlayerShooting class itself

    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct() {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other) {
        // If another player is hit...
        if (other.gameObject.tag == "Player") {
            // Particles
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            // References Player hit back to shooter
            shooter.OnPlayerHit(other.gameObject);
            // Delete bullet
            Destroy(gameObject);
        }
    }

    
}
