using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MonsterAI monsterIa;
    private PlayerMovement movement;

    private void Awake()
    {
        monsterIa = GameObject.Find("Monster").GetComponent<MonsterAI>();
        monsterIa.OnPlayerCatched += OnBeingCatched;
        movement = GetComponent<PlayerMovement>();
        
    }
    
    private void OnBeingCatched(object sender, System.EventArgs e)
    {
        
    }
}
