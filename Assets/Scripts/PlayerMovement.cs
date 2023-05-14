using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Joystick movementJoystick;
    Joystick shootingJoystick;

    public float moveSpeed;
    Rigidbody2D playerBody;
    public Vector2 movement = Vector2.zero; // stores joystick values between Update and FixedUpdate
    Vector2 rotation = Vector2.zero; // ""

    PlayerShooting shooting;
    PlayerLeveling leveling;
    GameObject map;

    Transform firepoint;
    float firepointRadius = 1f;

    public float MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = 5f - (0.25f * (value - 1)); }
    }

    void Awake() {
        movementJoystick = GameObject.Find("Canvas/LeftJoystick").GetComponent<Joystick>();
        shootingJoystick = GameObject.Find("Canvas/RightJoystick").GetComponent<Joystick>();
        playerBody = transform.GetComponent<Rigidbody2D>();
        shooting = transform.GetComponent<PlayerShooting>();
        leveling = transform.GetComponent<PlayerLeveling>();
        map = GameObject.Find("Map");
        firepoint = transform.Find("Firepoint").GetComponent<Transform>();

        MoveSpeed = leveling.Level;
        UpdateScale();

    }

    void Update() {
        if (movementJoystick != null) {
            movement.x = movementJoystick.Horizontal;
            movement.y = movementJoystick.Vertical;
        }
        if (shootingJoystick != null && shootingJoystick.Direction != Vector2.zero) {
            rotation.x = shootingJoystick.Horizontal;
            rotation.y = shootingJoystick.Vertical;
            shooting.Shoot();
        }
        
    }

    void FixedUpdate() {
        // Move
        playerBody.MovePosition(playerBody.position + movement * moveSpeed * Time.fixedDeltaTime);
        // Rotate
        float radians = Mathf.Atan2(rotation.y, rotation.x); // radians
        var x = Mathf.Cos(radians) * firepointRadius;
        var y = Mathf.Sin(radians) * firepointRadius;
        Vector3 relativePos = new Vector3(x, y, 0);
        firepoint.position = relativePos + transform.position;
        firepoint.eulerAngles = new Vector3(0f, 0f, radians * Mathf.Rad2Deg); //Quaternion.AngleAxis(radians * Mathf.Rad2Deg, axis);
        
        ConstrainToCircle(); // need to move to separate object
        
    }

    // updates localScale based on level
    public void UpdateScale() {
        transform.localScale = new Vector3(0.5f, 0.5f, 0f) + (new Vector3(0.1f, 0.1f, 0f) * (leveling.Level - 1));
    }

    void ConstrainToCircle() {
        if (Vector2.Distance(transform.position, map.transform.position) > map.GetComponent<CircleCollider2D>().radius) {
            // Calculate the direction back to the center of the circle
            Vector2 direction = map.transform.position - transform.position;
            // Move the game object back to the edge of the circle
            transform.position = (Vector2)map.transform.position - direction.normalized * map.GetComponent<CircleCollider2D>().radius;
        }
    }

    
}
