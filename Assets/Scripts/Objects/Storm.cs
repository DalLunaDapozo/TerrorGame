using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : MonoBehaviour
{

    private PlayerLocation playerLocation;
    
    [SerializeField] private float minTimeBetweenEvent;
    [SerializeField] private float maxTimeBetweenEvent;

    [SerializeField] private float timer;
    
    private bool eventOn;

    public CurrentRoom playerCurrentRoom;

    [SerializeField] private GameObject[] windowEffects;
    [SerializeField] private GameObject[] stormLights;

    [SerializeField] private FMODUnity.StudioEventEmitter thunderEvent;

    private void Awake()
    {
        playerLocation = GameObject.Find("Player").GetComponent<PlayerLocation>();
    }

    private void Start()
    {
        eventOn = false;
        GenerateRandomNumber();
    }

    private void GenerateRandomNumber()
    {
        if(!eventOn)
            timer = Random.Range(minTimeBetweenEvent, maxTimeBetweenEvent);
    }
    private void Update()
    {
        TimerBetweenEvent();
        playerCurrentRoom = playerLocation.playerCurrentRoom;
    }
    private void TimerBetweenEvent()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
            StormEventTest();
    }

    private void ActivateDependingOnPlayerLocation()
    {

    }

    private void StormEventTest()
    {
        eventOn = true;

        thunderEvent.Play();

        for (int i = 0; i < windowEffects.Length;i++)
        {
            if (windowEffects[i].GetComponent<PlayerLocation>().playerCurrentRoom == playerCurrentRoom)
            windowEffects[i].GetComponent<Animator>().SetTrigger("Event");
            
        }
        for (int i = 0; i < stormLights.Length; i++)
        {
            if (stormLights[i].GetComponent<PlayerLocation>().playerCurrentRoom == playerCurrentRoom)
                stormLights[i].GetComponent<Animator>().SetTrigger("Storm");
        }
        eventOn = false;
       
        GenerateRandomNumber();
        
    }
}
