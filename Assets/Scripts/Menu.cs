using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    private bool firstTime;

    private void Awake()
    {
        firstTime = false;
    }

    private void Start()
    {
        if (!firstTime)
        {
            PlayerPrefs.DeleteAll();
        }
        firstTime = true;
    }

    public void StartGame()
    {
        SceneController.LoadScene("BrightHouse", 1f ,1f );
    }

    public void Quit()
    {
        Application.Quit();
    }
}
