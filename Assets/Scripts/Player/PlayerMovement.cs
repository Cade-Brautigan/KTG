using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json.Linq;
using System;

public class PlayerMovement : NetworkBehaviour
{
    #region Data
    // Internal data
    [SerializeField][SyncVar] float moveSpeed;
    Vector2 movement = Vector2.zero; // Stores joystick values between Update and FixedUpdate
    Vector2 rotation = Vector2.zero; // ""
    [SyncVar] bool canMove = true;

    // Joysticks
    Joystick movementJoystick;
    Joystick shootingJoystick;

    // Reference components and gameobjects
    Rigidbody2D playerBody;
    GameObject map; // TODO move to separate script
    Transform firepoint;
    [SyncVar] float firepointRadius = 1f; // TODO get firepoint radius directly from firepoint somehow

    // Sister components
    PlayerShooting shooting;
    PlayerLeveling leveling;
    #endregion

    #region Start & Update
    void Awake()
    {
        // Reference components and gameobjects
        playerBody = transform.GetComponent<Rigidbody2D>();
        firepoint = transform.Find("Firepoint").GetComponent<Transform>();
        map = GameObject.Find("Map");

        // Sister components
        shooting = transform.GetComponent<PlayerShooting>();
        leveling = transform.GetComponent<PlayerLeveling>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        // Joysticks
        movementJoystick = GameObject.Find("Canvas/LeftJoystick").GetComponent<Joystick>();
        shootingJoystick = GameObject.Find("Canvas/RightJoystick").GetComponent<Joystick>();

        // Update Internal data
        CmdRequestUpdateMoveSpeed();
        CmdRequestUpdateScale();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (movementJoystick)
        {
            movement = movementJoystick.Direction;
        }

        if (shootingJoystick)
        {
            rotation = shootingJoystick.Direction;
        }
        
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        if (movement != Vector2.zero)
        {
            // Move
            Move(moveSpeed * Time.fixedDeltaTime * movement);
        }

        if (rotation != Vector2.zero)
        {
            // Rotate
            float radians = Mathf.Atan2(rotation.y, rotation.x);
            Vector3 relativePos = new Vector3(Mathf.Cos(radians) * firepointRadius, Mathf.Sin(radians) * firepointRadius, 0);
            Rotate(radians, relativePos);

            // Shoot
            shooting.Shoot();
        }

        ConstrainToCircle();
    }
    #endregion

    #region Position
    // Client side prediction
    private void Move(Vector2 move)
    {
        // We only want the own player's gameobject
        if (!isLocalPlayer) return;
        
        if (canMove)
        {
            transform.position += (Vector3)(move);
            CmdMove(move);
        }
    }

    [Command]
    private void CmdMove(Vector2 move)
    {
        // If player is host, don't apply movement twice
        if (isLocalPlayer) return;

        // TODO: Check if movement vector is valid

        // NetworkTransform syncs position to clients when position changes
        if (canMove)
        {
            transform.position += (Vector3)(move);
        }
    }

    private void ConstrainToCircle()
    {
        if (!isLocalPlayer) return;

        if (Vector2.Distance(transform.position, map.transform.position) > map.GetComponent<CircleCollider2D>().radius)
        {
            // Calculate the direction back to the center of the circle
            Vector2 direction = map.transform.position - transform.position;
            // Move the game object back to the edge of the circle
            transform.position = (Vector2)map.transform.position - direction.normalized * map.GetComponent<CircleCollider2D>().radius;
        }

        CmdConstrainToCircle();
    }

    [Command]
    void CmdConstrainToCircle()
    {
        if (isLocalPlayer) return;
        
        if (Vector2.Distance(transform.position, map.transform.position) > map.GetComponent<CircleCollider2D>().radius)
        {
            // Calculate the direction back to the center of the circle
            Vector2 direction = map.transform.position - transform.position;
            // Move the game object back to the edge of the circle
            transform.position = (Vector2)map.transform.position - direction.normalized * map.GetComponent<CircleCollider2D>().radius;
        }
        
    }

    [Server]
    public void ServerDisableMovement()
    {
        canMove = false;
    }

    [Server]
    public void ServerEnableMovement()
    {
        canMove = true;
    }
    #endregion

    #region Rotation
    private void Rotate(float radians, Vector3 relativePos)
    {
        if (!isLocalPlayer) return;
        
        firepoint.position = relativePos + transform.position;
        firepoint.eulerAngles = new Vector3(0f, 0f, radians * Mathf.Rad2Deg);

        CmdRotate(radians, relativePos);
    }

    [Command]
    private void CmdRotate(float radians, Vector3 relativePos)
    {
        if (!isLocalPlayer)
        {
            firepoint.position = relativePos + transform.position;
            firepoint.eulerAngles = new Vector3(0f, 0f, radians * Mathf.Rad2Deg);
        }
        RpcRotate(radians, relativePos);
    }

    [ClientRpc]
    private void RpcRotate(float radians, Vector3 relativePos)
    {
        if (isLocalPlayer) return;

        firepoint.position = relativePos + transform.position;
        firepoint.eulerAngles = new Vector3(0f, 0f, radians * Mathf.Rad2Deg);
    }
    #endregion

    #region OnLevelUp
    [Command]
    public void CmdRequestUpdateScale() 
    {
        ServerUpdateScale();
    }

    [Server]
    public void ServerUpdateScale()
    {
        if (leveling != null)
        {
            // NetworkTransform syncs scale to clients
            transform.localScale = new Vector3(0.5f, 0.5f, 0f) + (new Vector3(0.1f, 0.1f, 0f) * (leveling.Level - 1));
        }
        
    }

    [Command]
    public void CmdRequestUpdateMoveSpeed()
    {
        ServerUpdateMoveSpeed();
    }

    [Server]
    public void ServerUpdateMoveSpeed()
    {
        if (leveling != null)
        {
            moveSpeed = 5f - (0.25f * (leveling.Level - 1));
        }
    }
    #endregion
           
}
