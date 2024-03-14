using UnityEngine;
using System.Collections;
using NotSpaceInvaders;
using TMPro;
using UnityEngine.UI;

public class GameStateControllerScript : MonoBehaviour {
    public GameObject mainMenuCanvas;
    public GameObject playCanvas;
    public GameObject gameOverCanvas;

    public int score, top;

    [SerializeField] private TMP_Text[] highScores;
    
    
    private GameObject currentCanvas;
    private string state;

    public string filename = "top.txt";

    public void Start() {
        currentCanvas = null;
        MainMenu();

        foreach (var highScore in highScores)
        {
            highScore.text = Bridge.GetInstance().thisPlayerInfo.highScore.ToString();
        }
    }

    public void StartGame()
    {
        foreach (var highScore in highScores)
        {
            highScore.text = Bridge.GetInstance().thisPlayerInfo.highScore.ToString();
        }
    }

    public void Update() {
        if (state == "play") {
        }
        else if (state == "mainmenu") {
            if (Input.GetButtonDown("Cancel")) {
                Application.Quit();
            }
            else if (Input.anyKeyDown) {
            }
        }
        else if (state == "gameover") {
            if (Input.anyKeyDown) {
                Application.LoadLevel("Menu");
            }
        }
    }

    public void MainMenu() {
        CurrentCanvas = mainMenuCanvas;
        state = "mainmenu";

        GameObject.Find("LevelController").SendMessage("Reset");
        GameObject.FindGameObjectWithTag("Player").SendMessage("Reset");
        //GameObject.FindGameObjectWithTag("MainCamera").SendMessage("Reset");

    }

    public void Play() {
        CurrentCanvas = playCanvas;
        state = "play";
        score = 0;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>().canMove = true;
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovementScript>().moving = true;
    }

    public void GameOver() {
        CurrentCanvas = gameOverCanvas;
        state = "gameover";

        if (score > top) {
            PlayerPrefs.SetInt("Top", top);
        }
        
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovementScript>().moving = false;
    }

    private GameObject CurrentCanvas {
        get {
            return currentCanvas;
        }
        set {
            if (currentCanvas != null) {
                currentCanvas.SetActive(false);
            }
            currentCanvas = value;
            currentCanvas.SetActive(true);
        }
    }
}
