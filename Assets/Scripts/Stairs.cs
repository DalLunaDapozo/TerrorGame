using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public Transform teleportpoint;

    public Transform GetDestination()
    {
        return teleportpoint;
    }
}
