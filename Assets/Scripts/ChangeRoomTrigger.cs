using Cinemachine;
using UnityEngine;

public class ChangeRoomTrigger : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>();     
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("ASD");
            TransitionRoom();
        }
    }

    private void TransitionRoom()
    {
        cam.Follow = target;  
    }
}
