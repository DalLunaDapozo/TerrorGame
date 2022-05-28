using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : MonoBehaviour
{
    [SerializeField] private float minTimeBetweenEvent;
    [SerializeField] private float maxTimeBetweenEvent;

    [SerializeField] private float timer;
    private bool eventOn;

    [SerializeField] private Animator[] windowEffects;
    [SerializeField] private Animator[] stormLights;

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

    private void StormEventTest()
    {
        eventOn = true;

        for (int i = 0; i < windowEffects.Length;i++)
        {
            windowEffects[i].SetTrigger("Event");
            
        }
        for (int i = 0; i < stormLights.Length; i++)
        {
            stormLights[i].SetTrigger("Storm");
        }
        eventOn = false;
       
        GenerateRandomNumber();
        
    }

    private IEnumerator StormEvent()
    {
        eventOn = true;

        windowEffects[0].SetTrigger("Event");

        /*for (int i = 0; i < windowEffects.Length;i++)
        {
            windowEffects[i].SetTrigger("Event");
            
        }
        for (int i = 0; i < stormLights.Length; i++)
        {
            stormLights[i].SetTrigger("Storm");
        }*/

        yield return new WaitForSeconds(.1f);

        GenerateRandomNumber();
        eventOn = false;
    }

}
