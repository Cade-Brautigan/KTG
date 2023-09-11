using UnityEngine;
using Cinemachine;
using Mirror;

public class PlayerCameraController : NetworkBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Start()
    {
        if (!isLocalPlayer) return;

        vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        vcam.Follow = transform;
        
    }
}

