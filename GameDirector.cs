using UnityEngine;
using UnityEngine.UI; // UIを扱うために必要

public class GameDirector : MonoBehaviour
{
    // シングルトン（どこからでも呼べるようにする）
    public static GameDirector Instance;

    [Header("UI Groups (ヒエラルキーからセットする)")]
    public GameObject titleGroup;
    public GameObject gameGroup;
    public GameObject resultGroup;

    [Header("Game Settings")]
    public bool isPlaying = false; // ゲーム中かどうか
    public float gameSpeed = 1.0f; // 現在のゲームスピード
    public float baseSpeed = 5.0f; // 基本スピード
    public float speedUpRate = 0.1f; // 1秒ごとの加速量

    private float scoreTime = 0f; // 経過時間

    void Awake()
    {
        // 自分自身をセット
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 最初はタイトル画面へ
        ShowTitle();
    }

    void Update()
    {
        // ゲーム中の処理
        if (isPlaying)
        {
            // 1. スコア（時間）を加算
            scoreTime += Time.deltaTime;

            // 2. 時間経過で徐々にスピードアップ
            //    (例: 10秒でスピードが +1 される)
            gameSpeed = baseSpeed + (scoreTime * speedUpRate);

            // ここにUIのスコア更新処理などを後で書く
            // Debug.Log("Score: " + scoreTime.ToString("F1") + " / Speed: " + gameSpeed);
        }
    }

    // --- 画面切り替え用関数 ---

    public void ShowTitle()
    {
        isPlaying = false;
        titleGroup.SetActive(true);
        gameGroup.SetActive(false);
        resultGroup.SetActive(false);
    }

    public void StartGame()
    {
        // ゲーム開始（リセット）
        scoreTime = 0f;
        gameSpeed = baseSpeed;
        isPlaying = true;

        titleGroup.SetActive(false);
        gameGroup.SetActive(true);
        resultGroup.SetActive(false);
        
        Debug.Log("ゲームスタート！");
    }

    // ゲームオーバー（プレイヤーが当たったら呼ぶ）
    public void GameOver()
    {
        isPlaying = false;
        
        // 少し待ってからリザルトを出すなどの演出もここでできる
        titleGroup.SetActive(false);
        gameGroup.SetActive(true); // 背景として残すならTrue
        resultGroup.SetActive(true);
        
        Debug.Log("ゲームオーバー...");
    }

    // 広告を見て復活（後で中身を書く）
    public void ReviveGame()
    {
        resultGroup.SetActive(false);
        isPlaying = true;
        Debug.Log("復活！");
        
        // 無敵時間の処理などを後で追加
    }
}
