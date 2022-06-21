using UnityEngine;

public class PlayerLocation : MonoBehaviour
{

    public bool TestMode;
    public bool ItsPlayer;
    [SerializeField] private Transform ritualCircle;
    [SerializeField] public CurrentRoom playerCurrentRoom = CurrentRoom.MainRoom;

    public bool isSecondFloor;

    private void Awake()
    {
        try { ritualCircle = GameObject.Find("RitualCircle").transform; }
        catch { Debug.Log("Ritual circle Missing"); }
    }

    private void Start()
    {
        if (ritualCircle != null)
        {
            if (!TestMode && ItsPlayer)
            {
                transform.position = ritualCircle.transform.position;
                playerCurrentRoom = CurrentRoom.RitualRoom;
            }
        }
        else
            return;

    }

    private void Update()
    {
        if (playerCurrentRoom == CurrentRoom.RitualRoom ||
            playerCurrentRoom == CurrentRoom.Corridor ||
            playerCurrentRoom == CurrentRoom.SecondFloorMain ||
            playerCurrentRoom == CurrentRoom.OwnBedroom)
            isSecondFloor = true;
        else
            isSecondFloor = false;
    
    }


}
