using Cinemachine;
using UnityEngine;

public class ChangeRoomTrigger : MonoBehaviour
{
    public Transform cameraMovesTo;
    public CurrentRoom roomName;
    
    private CinemachineVirtualCamera cam;
    private PlayerLocation playerLocation;

    public float cameraDistance;
    private float cameraDistanceMax;

    public bool isTeleport;
    public Transform teleportpoint;
    
    private void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        playerLocation = GameObject.Find("Player").GetComponent<PlayerLocation>();

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            TransitionRoom();
        }
        

    }
    
    public Transform GetDestination()
    {
        return teleportpoint;
    }
    
    private void TransitionRoom()
    {
        cam.Follow = cameraMovesTo;
        cam.GetComponentInChildren<Camera>().orthographicSize = cameraDistance;
        playerLocation.playerCurrentRoom = roomName;
    }
}
