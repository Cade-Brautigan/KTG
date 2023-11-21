using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json.Linq;
using System;

public class PlayerMovement : NetworkBehaviour
{
    #region Data
    // Internal Data
    [SerializeField][SyncVar] float moveSpeed;
    [SyncVar] bool canMove = true;
    Joystick movementJoystick;
    Vector2 movement = Vector2.zero; // Stores joystick values between Update and FixedUpdate
    [SyncVar] Vector2 networkedMovement = Vector2.zero;

    // Accessors
    public Vector2 Movement => movement;
    public Vector2 NetworkedMovement => networkedMovement;

    // Sister Components
    PlayerShooting shooting;
    PlayerLeveling leveling;

    GameObject map; // TODO move to separate script
    #endregion

    #region Start & Update
    void Awake()
    {
        // Reference components and gameobjects
        map = GameObject.Find("Map");

        // Sister components
        shooting = transform.GetComponent<PlayerShooting>();
        leveling = transform.GetComponent<PlayerLeveling>();
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        movementJoystick = GameObject.Find("Canvas/LeftJoystick").GetComponent<Joystick>();

        CmdRequestUpdateMoveSpeed();
        CmdRequestUpdateScale();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (movementJoystick)
        {
            movement = movementJoystick.Direction;
            CmdUpdateNetworkedMovement(movement);
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

        ConstrainToCircle();
    }
    #endregion

    #region Position
    // Client side prediction
    private void Move(Vector2 move)
    {
        if (!isLocalPlayer) return;
        
        if (canMove)
        {
            transform.position += (Vector3)(move);
            CmdMove(transform.position);
        }
    }

    [Command]
    private void CmdMove(Vector3 position)
    {
        // TODO: Check if position update is valid.
        // If movement is not valid, RpcMove the player to last valid position

        if (canMove)
        {
            // If player is host, don't apply movement twice
            if (!isLocalPlayer) 
            {
                transform.position = position;
            }
            RpcMove(position);
        }
    }

    // Use custom movement Rpc instead of Mirror's for more responsive movement
    [ClientRpc]
    private void RpcMove(Vector3 position)
    {
        if (isLocalPlayer) return;

        transform.position = position;
    }

    [Command]
    void CmdUpdateNetworkedMovement(Vector2 newMovement)
    {
        networkedMovement = newMovement;
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
            transform.localScale = new Vector3(0.5f, 0.5f, 0f) + (new Vector3(0.05f, 0.05f, 0f) * (leveling.Level - 1));
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
            if (leveling.Level < 20)
            {
                moveSpeed = 5f - (0.225f * (leveling.Level - 1));
            }
            else
            {
                moveSpeed = 0.5f;
            }
            
        }
    }
    #endregion
           
}
