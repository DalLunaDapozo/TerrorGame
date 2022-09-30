using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openDoor;

    private void Start()
    {
        openDoor.SetActive(false);
        closedDoor.SetActive(true);
    }

    private void Update()
    {
        if (!closedDoor.activeInHierarchy)
        {
            openDoor.SetActive(true);
        }
    }
}
