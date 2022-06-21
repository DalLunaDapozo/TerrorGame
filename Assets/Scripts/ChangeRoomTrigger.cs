using Cinemachine;
using UnityEngine;

public class ChangeRoomTrigger : MonoBehaviour
{
    public Transform cameraMovesTo;
    public CurrentRoom roomName;
    
    private CinemachineVirtualCamera cam;
    private PlayerMovement player;
    private PlayerLocation playerLocation;

    public float cameraDistance;
    private float cameraDistanceMax;

    public float lerpRate;

    public bool isTeleport;
    public Transform teleportpoint;
    private void Awake()
    {
        cam = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerLocation = player.GetComponent<PlayerLocation>();

        player.OnUsingStairs += PlayerUsingStairs;
    }
//ERNESTO TE AMO
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
        if (player.lighterIsOn && playerLocation.playerCurrentRoom != roomName)
        {
            cam.Follow = cameraMovesTo;
            cam.GetComponentInChildren<Camera>().orthographicSize = cameraDistance;
        }
          
        playerLocation.playerCurrentRoom = roomName;
    }
    
    public void PlayerUsingStairs(object sender, System.EventArgs e)
    {
        TransitionRoom();
    }
}
