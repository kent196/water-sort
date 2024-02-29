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

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool CheckWinCondition()
    {
        // Check if all the bottles have been filled with the correct colors or are empty
        foreach (BottleController bottle in FindObjectsOfType<BottleController>())
        {
            if (bottle.numberOfTopColorLayers != 4 && bottle.numberOfTopColorLayers != 0)
            {
                // If any bottle has not been filled with the correct colors or is not empty, return false
                return false;
            }
        }

        // If all bottles have been filled with the correct colors or are empty, return true
        return true;
    }

}
