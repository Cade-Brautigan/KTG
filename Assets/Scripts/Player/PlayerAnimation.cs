using UnityEngine;
using Mirror;
using Mirror.Examples.AdditiveScenes;
using UnityEngine.Assertions.Must;

public class PlayerAnimation : NetworkBehaviour
{
    Animator animator;

    SpriteRenderer firepoint;

    PlayerMovement playerMovement;
    PlayerShooting playerShooting;

    void Awake()
    {
        animator = GetComponent<Animator>();

        firepoint = transform.Find("Firepoint").GetComponent<SpriteRenderer>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            UpdateAnimation(playerMovement.Movement, playerShooting.Rotation);
        }
        else
        {
            UpdateAnimation(playerMovement.NetworkedMovement, playerShooting.NetworkedRotation);
        }
    }

    void UpdateAnimation(Vector2 movement, Vector2 rotation)
    {
        animator.SetFloat("MovementHorizontal", movement.x);
        animator.SetFloat("MovementVertical", movement.y);

        animator.SetFloat("RotationHorizontal", rotation.x);
        animator.SetFloat("RotationVertical", rotation.y);

        animator.SetBool("Moving", movement != Vector2.zero);
        animator.SetBool("Shooting", rotation != Vector2.zero);

        float radians = Mathf.Atan2(rotation.y, rotation.x);
        float degrees = radians * Mathf.Rad2Deg;

        if (/*(movement == Vector2.zero) &&*/ (rotation == Vector2.zero)) // Idle
        {
            firepoint.enabled = false;
        }
        else
        {
            firepoint.enabled = true;
        }

        if (degrees >= -90f && degrees <= 90f) // Right 180 degrees
        {
            firepoint.flipY = false;
            
        } 
        else
        {
            firepoint.flipY = true;
        }

        if (degrees >= -157.5 && degrees <= -22.5) // When facing Southwest, South, or Southeast
        {
            firepoint.sortingOrder = 1;
        }
        else
        {
            firepoint.sortingOrder = -1;
        }
    }

}
