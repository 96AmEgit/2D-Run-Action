using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text timeText; // 時間のTextMeshProオブジェクトをInspectorウィンドウで関連付ける
    public float countDuration = 10f; // カウントする時間（秒）

    private float timer; // タイマー
    private bool isCounting = true; // カウント中かどうかのフラグ
    private bool isPaused = false; // 一時停止中かどうかのフラグ

    void Start()
    {
        timer = countDuration; // タイマーを設定した時間で初期化
        UpdateTimeText(); // 残り時間を表示するTextMeshProを更新
    }

    void Update()
    {
        if (isCounting && !isPaused)
        {
            timer -= Time.deltaTime; // 経過時間を減算

            if (timer <= 0f)
            {
                timer = 0f; // タイマーが負の値にならないように修正
                isCounting = false; // カウント終了
                PauseGame(); // ゲームを一時停止
            }

            UpdateTimeText(); // タイマーを更新
        }

        if (isPaused && Input.GetKeyDown(KeyCode.Y))
        {
            ResumeGame(); // キー入力でゲームを再開
        }
    }

    void UpdateTimeText()
    {
        timeText.text = Mathf.CeilToInt(timer).ToString(); // タイマーを整数に変換してTextMeshProに表示
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // ゲーム時間を停止
        isPaused = true; // 一時停止フラグを立てる
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // ゲーム時間を再開
        isPaused = false; // 一時停止フラグを解除
    }
}
