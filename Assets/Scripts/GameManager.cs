using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static PathManager;

//hi!

public class GameManager : MonoBehaviour
{
    private InspectorController inspectorController => InspectorController.Instance;
    [SerializeField] private List<Level> levels;
    [SerializeField] private string mapScreen;

    public string CurrentLevel { get; private set; }
    public List<string> ClearedLevels { get; private set; } = new();
    [field: SerializeField]
    public List<string> Upgrades { get; private set; } = new();
    public Dictionary<LevelTile, LevelStatus> LevelStatuses { get; set; } = new();


    // TODO: PC should control these values

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static PlayerController Player { get { return PlayerSpawn.Player;  } }

    public bool DoPrimarySpawn { get; private set; } = true;

    public Vector2 MapSpawnPoint { get; set; }

    public enum GameState
    {
        Playing, Paused, Cutscene
    }

    // this is a property with default getters/setters
    public GameState CurrentGameState { get; private set; }

    // events for the game being paused or resumed
    public UnityEvent OnGameStart;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameWin;
    public UnityEvent OnGameLoss;
    public UnityEvent OnGameQuit;
    public UnityEvent OnPlayerHurt;
    public UnityEvent OnPlayerHeal;

    private string currentScene;
    private IEnumerator DamageCoroutineObject;


    private bool isPlayerDamageable = true;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            currentScene = SceneManager.GetActiveScene().name;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Remove if we have a title screen
    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        
        OnGameStart.Invoke();
        TogglePause();
        TogglePause();
    }

    public void PauseGame()
    {
        if (CurrentGameState == GameState.Cutscene) 
            return;
        CurrentGameState = GameState.Paused;
        OnGamePaused.Invoke();

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Cutscene) 
            return;
        CurrentGameState = GameState.Playing;
        OnGameResumed.Invoke();

        Time.timeScale = 1;
    }

    public void EnterCutscene()
    {
        CurrentGameState = GameState.Cutscene;
        Time.timeScale = 0;

    }
    public void ExitCutscene()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        OnGameQuit.Invoke();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Call this to reset everything
    public void ResetScene()
    {
        if (currentScene == mapScreen) return;
        LoadScene(currentScene);
    }

    public void StartLevel(string levelName)
    {
        foreach(Level level in levels)
        {
            if (level.name == levelName)
            {
                CurrentLevel = levelName;
                LoadScene(level.firstScene);
            }
        }
    }

    public void ClearLevel()
    {
        if (CurrentLevel != null)
        {
            Debug.Log("Level cleared: " + CurrentLevel);
            ClearedLevels.Add(CurrentLevel);
        }
        LoadScene(mapScreen);
    }

    public void ExitLevel()
    {
        LoadScene(mapScreen);
    }

    public void AddUpgrade(string upgradeName)
    {
        Upgrades.Add(upgradeName);
    }

    public void LoadScene(string sceneName, bool primarySpawn = true)
    {
        if (sceneName is null) return;

        DoPrimarySpawn = primarySpawn;

        SceneManager.LoadScene(sceneName);
        currentScene = sceneName;
        if (inspectorController)
        {
            inspectorController.StopDisplaying();
        }
    }

    public void TogglePause()
    {
        if (CurrentGameState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (CurrentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResumeGame();
    }
}

[System.Serializable]
public class Level
{
    public string name;
    public string firstScene;
}