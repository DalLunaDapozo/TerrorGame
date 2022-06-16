using Cinemachine;
using UnityEngine;
using System.Collections;


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

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeRate;

    [SerializeField] private float maxTimeWithLightsOff;
    public float timeElapsedWithLightsOff;

    CinemachineBasicMultiChannelPerlin cameraShake;

    public event System.EventHandler HeartAttackEvent;


    public bool canDie;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        cameraShake = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        
        childCamera = cam.GetComponentInChildren<Camera>();
        playerLocation = player.GetComponent<PlayerLocation>();
    
    }

    private void Update()
    {

        cameraShake.m_AmplitudeGain = shakeIntensity;
        currentDistace = childCamera.orthographicSize;
        roomWherePlayerIs = playerLocation.playerCurrentRoom;

        if (!player.lighterIsOn)
        {
            cam.Follow = player.transform;

            timeElapsedWithLightsOff += Time.deltaTime;

            if (!PlayerShouldDie())
            {
                if(childCamera.orthographicSize > minDistance)
                childCamera.orthographicSize -= lerpRate;
                
                if(canDie)
                    shakeIntensity += shakeRate;
               
            }
            else
            {

                if (canDie)
                {
                    StartCoroutine(HeartAttack());
                }
                
                timeElapsedWithLightsOff = 0;
            }
               
        }
        else
        {
            cam.Follow = GameObject.Find(roomWherePlayerIs.ToString() + "CameraPoint").transform;
            shakeIntensity = 0;
            timeElapsedWithLightsOff = 0;
            if (childCamera.orthographicSize < maxDistance)
            {
                childCamera.orthographicSize += lerpRate * lerpMultiplier;
            }   
        }
    }

    IEnumerator HeartAttack()
    {
        
        gameController.isGameOver = true;
        player.HeartAttackAnimation();
       
        yield return new WaitForSeconds(1f);
      
        canDie = false;
        HeartAttackEvent?.Invoke(this, System.EventArgs.Empty);
    }

    private bool PlayerShouldDie()
    {
        return timeElapsedWithLightsOff >= maxTimeWithLightsOff;
        //return childCamera.orthographicSize > minDistance;
    }
}
