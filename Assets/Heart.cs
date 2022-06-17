using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter emitter;
    [SerializeField] private Transform ritualCircle;

    public float distance;
    public float heartbeatspeed;

    private void Update()
    {
        emitter.SetParameter("Heartbeat Proximity", heartbeatspeed);
        distance = Vector2.Distance(transform.position, ritualCircle.position);

        if (distance < 86)
            heartbeatspeed = -distance + 85;
        else
            heartbeatspeed = 0;

    }

}
