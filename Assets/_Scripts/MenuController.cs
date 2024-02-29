using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu, endLevelMenu, endBtn, nextBtn;
    [SerializeField] private TMP_Text text;
    public static MenuController instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int levelText = SceneManager.GetActiveScene().buildIndex + 1;
        text.text = "Level " + levelText;
        endBtn.SetActive(false);
        pauseMenu.SetActive(false);
        endLevelMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.active)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (SceneManager.GetActiveScene().name == "Level4")
        {
            endBtn.SetActive(true);
            nextBtn.SetActive(false);
        }
    }

    public void Resume()
    {
        pauseMenu?.SetActive(false);
        GameManager.Instance.Resume();
    }

    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }

    public void Pause()
    {
        pauseMenu?.SetActive(true);
        GameManager.Instance.Pause();
    }

    public void Replay()
    {
        pauseMenu?.SetActive(false);
        GameManager.Instance.ReplayLevel();
    }

    public void NextLevel()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.NextLevel();
    }

    public void LevelComplete()
    {
        endLevelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CheckWinningCondition()
    {
        if (GameManager.Instance.CheckWinCondition())
        {
            Debug.Log(GameManager.Instance.CheckWinCondition());
            LevelComplete();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
