using Cinemachine;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] public Transform pointToGo;
    public Transform target;
    public CurrentRoom roomName;
    private PlayerLocation playerLocation; 
    private CinemachineVirtualCamera cam;
    
    private void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        playerLocation = GameObject.Find("Player").GetComponent<PlayerLocation>();
    }

    private void TransitionRoom()
    {
        cam.Follow = target;
        playerLocation.playerCurrentRoom = roomName;
    }
}
