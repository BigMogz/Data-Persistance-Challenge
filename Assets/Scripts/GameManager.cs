using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    private Button StartGame;
    public Button highScoreButton;
    private Button Quit;
    private Button ScoreButton;
    private Button closeScore;

    private GameObject scoreDisplayPanel;

    public AudioSource Music;

    [HideInInspector]
    public string playerName = "Name";

    public TextMeshProUGUI nameIsThis;
    private TextMeshProUGUI scoreDisplay;

    public TMP_InputField inputField;

    public int[] scoreBoard = new int[3];

    [HideInInspector]
    public string[] scoreNames = new string[3];


    public int score;
    public int highScoreText;
    public int hScore;


    public static GameManager instance;


    public void ScoreBoard()
    {
        string scores = "";
        for(int i = 0; i < scoreBoard.Length; i++)
        {
            scores += "High Score " + (i + 1) + ": \n   " + scoreNames[i] + " - " + scoreBoard[i] +  " \n";
            Debug.Log(scores);

        }
 
        scoreDisplayPanel.SetActive(true);
       
        closeScore = GameObject.Find("CloseScore").GetComponent<Button>();
        closeScore.onClick.AddListener(CloseScore);

        scoreDisplay = GameObject.Find("ScoreBoard").GetComponent<TextMeshProUGUI>();

        scoreDisplay.text = scores;

    }
    //======This method is called in order to find the disabled scoreboard overlay on entry screen
    GameObject FindObjectInScene(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }

    //-----Method to change name that is called when input field listener detects text change
    public void EnterName(string nameIs)
    {
        playerName = nameIs;
        nameIsThis.text = nameIs;
        Debug.Log("Player name: " + nameIs);

    }
    //---on awake, turn into singleton and delete any subsequent GameManagers
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            scoreNames = new string[3];

            LoadGame();

            return;
        }
        else if (gameObject != this)
        { Destroy(gameObject); }

    }
    //-------On enable, add OnSceneLoaded method to sceneLoaded sequene for GameManager
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene s, LoadSceneMode mode) //method to return references to scene objects on title screen when going back from main game
    {
        if(s.name == "StartPage")
        {
            nameIsThis = GameObject.Find("NameIs").GetComponent<TextMeshProUGUI>();

            //----listeners must be re-added when scene restarts

            inputField = GameObject.Find("Input").GetComponent<TMP_InputField>();
            inputField.onValueChanged.AddListener(EnterName);

            StartGame = GameObject.Find("Start").GetComponent<Button>();
            StartGame.onClick.AddListener(StartGameButton);

            Quit = GameObject.Find("Quit").GetComponent<Button>();
            Quit.onClick.AddListener(OnQuit);

            ScoreButton = GameObject.Find("ScoreButton").GetComponent<Button>();
            ScoreButton.onClick.AddListener(ScoreBoard);

            //---here the findobject method is called because the score panel is disabled by default and can't be found otherwise
            scoreDisplayPanel = FindObjectInScene("ScoreOverlay");

            nameIsThis = GameObject.Find("NameIs").GetComponent<TextMeshProUGUI>();
            nameIsThis.text = playerName;

            Music = GameObject.Find("Music").GetComponent<AudioSource>();





        }
    }

    public void CloseScore()
    {
        scoreDisplayPanel.SetActive(false);
    }
    public void HomeButton()
    {
        SceneManager.LoadScene(0);
        SaveGame();
    }

    public void StartGameButton()
    { 
        SceneManager.LoadScene("main");
        Debug.Log("Start button pressed");
    }

    [System.Serializable]
    public class HighScore
    {
        public int highScore;
        public string nameSave;
        public int[] scoreBoardData = new int[3];
        public string[] _scoreNames = new string[3];

    }
    
    public void OnQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        SaveGame();
    }
    void SaveGame()
    {
        HighScore scoreToSave = new HighScore();

        scoreToSave.highScore = instance.hScore;
        scoreToSave.nameSave = playerName;
        scoreToSave.scoreBoardData = scoreBoard;
        scoreToSave._scoreNames = new string[3];
        scoreToSave._scoreNames = scoreNames;

        string json = JsonUtility.ToJson(scoreToSave);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HighScore data = JsonUtility.FromJson<HighScore>(json);

            hScore = data.highScore;
            playerName = data.nameSave;
            scoreBoard = data.scoreBoardData;
            scoreNames = data._scoreNames;
            
        }



    }

    
    public void VolumeChanger(float music)
    {
        Music.volume = music;
    }



}
