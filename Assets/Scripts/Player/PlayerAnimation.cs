using UnityEngine;
using Mirror;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;
    // Enum to represent the 8 movement directions
    enum Direction { None, Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight };
    Direction currentDirection = Direction.None;
    PlayerMovement playerMovement;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Get the horizontal and vertical input values
        //float radians = Mathf.Atan2(playerMovement.movement.y, playerMovement.movement.x);
        //float degrees = radians * Mathf.Rad2Deg;
        //Debug.Log(degrees);

        // Determine the current movement direction based on the input values
        /*
        if (playerMovement.movement == null) {
            currentDirection = Direction.None;
        }
        else if ((degrees >= 0 && degrees < 22.5) || (degrees <= 0 && degrees >= -22.5)) {
            currentDirection = Direction.Right;
        }
        else if (degrees >= 22.5 && degrees < 67.5) {
            currentDirection = Direction.UpRight;
        }
        else if (degrees >= 67.5 && degrees < 112.5) {
            currentDirection = Direction.Up;
        }
        else if (degrees >= 112.5 && degrees < 157.5) {
            currentDirection = Direction.UpLeft;
        }
        else if ((degrees >= 157.5 && degrees <= 180) || (degrees >= -180 && degrees < -157.5)) {
            currentDirection = Direction.Left;
        }
        else if (degrees >= -157.5 && degrees < -112.5) {
            currentDirection = Direction.DownLeft;
        }
        else if (degrees >= -112.5 && degrees < -67.5) {
            currentDirection = Direction.Down;
        }
        else if (degrees >= -67.5 && degrees < -22.5) {
            currentDirection = Direction.DownRight;
        }
        */
        //Debug.Log(currentDirection);

        // Update the animator's "Direction" parameter based on the current movement direction
        //animator.SetInteger("Direction", (int)currentDirection);
        //animator.SetFloat("Speed", Mathf.Abs(playerMovement.movement.magnitude * playerMovement.moveSpeed));
    }
}