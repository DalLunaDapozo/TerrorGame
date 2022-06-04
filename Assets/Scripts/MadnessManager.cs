using Cinemachine;
using UnityEngine;

public class MadnessManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private CinemachineVirtualCamera cam;
    private PlayerLocation playerLocation;
    private GameController gameController;
    private Camera childCamera;

    public CurrentRoom roomWherePlayerIs;

    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    public float currentDistace;

    [SerializeField] private float lerpRate;
    [SerializeField] private float lerpMultiplier;
    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void Start()
    {
        childCamera = cam.GetComponentInChildren<Camera>();
        playerLocation = player.GetComponent<PlayerLocation>();
    
    }

    private void Update()
    {
        currentDistace = childCamera.orthographicSize;
        roomWherePlayerIs = playerLocation.playerCurrentRoom;

        if (!player.lighterIsOn)
        {
            cam.Follow = player.transform;
            
            if (childCamera.orthographicSize > minDistance)
            {
                childCamera.orthographicSize -= lerpRate;
            }
            /*else if (childCamera.orthographicSize <= minDistance)
                gameController.isGameOver = true;*/
        }
        else
        {

            cam.Follow = GameObject.Find(roomWherePlayerIs.ToString() + "CameraPoint").transform;
            
            if (childCamera.orthographicSize < maxDistance)
            {
                childCamera.orthographicSize += lerpRate * lerpMultiplier;
            }
            else if (childCamera.orthographicSize >= maxDistance)
            {
                return;
            }
        }

    }

    
}
