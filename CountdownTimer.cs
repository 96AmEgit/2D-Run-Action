using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Time Settings")]
    public TMP_Text timeText; // 時間のTextMeshPro
    public float countDuration = 10f; // カウントする時間（秒）

    [Header("UI Settings")]
    // ★追加: ここにUnityエディタから「結果ボードの親オブジェクト」をセットします
    public GameObject resultBoardUI; 

    private float timer; // タイマー
    private bool isCounting = true; // カウント中かどうかのフラグ
    private bool isPaused = false; // 一時停止中かどうかのフラグ

    void Start()
    {
        timer = countDuration; // タイマー初期化
        UpdateTimeText(); // 表示更新

        // ★追加: ゲーム開始時は、結果ボードを隠す
        if (resultBoardUI != null)
        {
            resultBoardUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isCounting && !isPaused)
        {
            timer -= Time.deltaTime; // 経過時間を減算

            if (timer <= 0f)
            {
                timer = 0f; // 負の値にならないように
                isCounting = false; // カウント終了
                PauseGame(); // ゲーム停止処理へ
            }

            UpdateTimeText(); // 表示更新
        }

        // デバッグ用: Yキーで再開機能（必要なければ削除してもOK）
        if (isPaused && Input.GetKeyDown(KeyCode.Y))
        {
            ResumeGame(); 
        }
    }

    void UpdateTimeText()
    {
        timeText.text = Mathf.CeilToInt(timer).ToString(); 
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // 時間を止める
        isPaused = true;

        // ★追加: 時間切れで結果ボードを表示する
        if (resultBoardUI != null)
        {
            resultBoardUI.SetActive(true);
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // 時間を再開
        isPaused = false;

        // ★追加: ゲーム再開時は結果ボードを再び隠す
        if (resultBoardUI != null)
        {
            resultBoardUI.SetActive(false);
        }
    }
}
