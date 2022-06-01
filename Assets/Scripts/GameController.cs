using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private MonsterIA monsterIA;
    public GameObject player;
    public bool isGameOver;

    public float timeBeforeRestart;

    [SerializeField] private GameObject gameOverText;

    private void Awake()
    {
        if(monsterIA != null)
        {
            monsterIA = GameObject.Find("Monster").GetComponent<MonsterIA>();
            monsterIA.OnPlayerCatched += GameOver;
        }
            
        player.GetComponent<PlayerController>();
        player.GetComponent<PlayerMovement>();
    }

    private void GameOver(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine("GameOverSequence");
    }

    private IEnumerator GameOverSequence()
    {
        gameOverText.SetActive(true);

        yield return new WaitForSeconds(timeBeforeRestart);

        SceneManager.LoadScene("Prueba");
    }

    
}
