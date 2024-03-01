using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private BottleController[] bottleList;
    private const string LEVEL1 = "Level1";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        AudioManager.Instance.PauseAllSound();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(LEVEL1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        AudioManager.Instance.UnPauseAllSound();
        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (nextScene > sceneCount - 1)
        {
            Debug.Log("Game over");
            //Invoke("Quit", 3f);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public bool CheckWinCondition()
    {
        // Check if all the bottles have been filled with the correct colors or are empty
        foreach (BottleController bottle in FindObjectsOfType<BottleController>())
        {
            if (bottle.numberOfTopColorLayers != 4 && bottle.numberOfTopColorLayers != 0)
            {
                // If any bottle has not been filled with the correct colors or is not empty, return false
                Debug.Log("Not win yet");
                return false;
            }
        }

        // If all bottles have been filled with the correct colors or are empty, return true
        Debug.Log("win");
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {

            Invoke("EndGame", 3f);
        }
        return true;
    }

    public void EndGame()
    {
        MenuController.instance.LevelComplete();
    }

}
