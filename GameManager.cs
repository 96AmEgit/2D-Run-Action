using UnityEngine;
using TMPro;

public enum GameState
{
    Title,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject titlePanel;
    public GameObject gameUIPanel;
    public GameObject gameOverPanel;

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;

    private float playTime = 0f;
    private GameState state;

    void Awake()
    {
        Instance = this;
    }

    // シーン開始時に必ず Title 状態へ
    void Start()
    {
        SetState(GameState.Title);
    }

    void Update()
    {
        if (state == GameState.Playing)
        {
            playTime += Time.deltaTime;
            scoreText.text = ((int)playTime).ToString();
        }
    }

    public void StartGame()
    {
        Debug.Log("誰かがStartGameを呼びました！ 呼び出し元: \n" + System.Environment.StackTrace);
        playTime = 0f;
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
        finalScoreText.text = ((int)playTime).ToString();
    }

    public void BackToTitle()
    {
        SetState(GameState.Title);
    }

    private void SetState(GameState newState)
    {
        state = newState;

        titlePanel.SetActive(state == GameState.Title);
        gameUIPanel.SetActive(state == GameState.Playing);
        gameOverPanel.SetActive(state == GameState.GameOver);
    }
}

