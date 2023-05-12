using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    
    private CinemachineVirtualCamera vcam;
    private PhotonView photonView;

    private void Start() {
        vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        photonView = GetComponent<PhotonView>();

        // If this is the local player, set the Cinemachine Virtual Camera follow target to this player
        if (photonView.IsMine) {
            vcam.Follow = transform;
        }
    }

    private void Update() {
        // Only the local player should control the Cinemachine Virtual Camera
        if (photonView.IsMine) {
            // Update the position of the Cinemachine Virtual Camera to follow the player
            vcam.transform.position = transform.position;
        }
    }

}
