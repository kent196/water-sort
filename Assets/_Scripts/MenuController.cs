using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu, endLevelMenu;
    [SerializeField] private TMP_Text text;
    bool isPause = false;
    public static MenuController instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int levelText = SceneManager.GetActiveScene().buildIndex + 1;
        text.text = "Level " + levelText;
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
        if (Input.GetMouseButtonDown(0))
        {
            CheckResumeOnClick();
        }
        CheckWinningCondition();
    }

    void CheckResumeOnClick()
    {
        // Create a layer mask to ignore UI elements
        LayerMask ignoreUILayers = LayerMask.GetMask("UI");

        // Raycast using the layer mask
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, ignoreUILayers))
        {
            Resume();
        }
    }

    public void Resume()
    {
        isPause = false;
        pauseMenu?.SetActive(false);
        GameManager.Instance.Resume();
    }

    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }

    public void Pause()
    {
        isPause = true;
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
        //GameManager.Instance.NextLevel();
    }

    public void LevelComplete()
    {
        endLevelMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CheckWinningCondition()
    {
        //if (GameManager.Instance.CheckWinCondition())
        //{
        //    //if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        //    //{
        //    //    Invoke("LevelComplete", 3f);
        //    //}
        //}
    }

    public void PlayAgain()
    {
        Debug.Log("Clicked");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
