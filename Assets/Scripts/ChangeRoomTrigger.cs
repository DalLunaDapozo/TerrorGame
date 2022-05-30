using Cinemachine;
using UnityEngine;

public class ChangeRoomTrigger : MonoBehaviour
{
    public Transform target;
    public CurrentRoom roomName;
    
    private CinemachineVirtualCamera cam;
    private PlayerLocation playerLocation;

    

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
    private void TransitionRoom()
    {
        cam.Follow = target;
        playerLocation.playerCurrentRoom = roomName;
    }
}
