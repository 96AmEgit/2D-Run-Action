using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class TagScoreInfo
{
    public string tag;
    public int scoreToAdd;
}

public class ObjectDestroyer : MonoBehaviour
{
    public List<TagScoreInfo> tagScoreInfos = new List<TagScoreInfo>();

    // ★変更点: 1つの変数ではなく、リスト(複数)に変更しました
    [Header("UI Settings")]
    public List<TextMeshProUGUI> scoreTexts = new List<TextMeshProUGUI>();

    private int currentScore = 0;

    void Start()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tagScoreInfo in tagScoreInfos)
        {
            if (other.CompareTag(tagScoreInfo.tag))
            {
                Destroy(other.gameObject);
                AddScore(tagScoreInfo.scoreToAdd);
                break;
            }
        }
    }

    void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        UpdateScoreText();
    }

    // ★変更点: リストに入っている全てのテキストを更新します
    void UpdateScoreText()
    {
        foreach (var textMesh in scoreTexts)
        {
            // 空の要素があってもエラーにならないようにチェック
            if (textMesh != null)
            {
                textMesh.text = currentScore.ToString();
            }
        }
    }

    public void SaveHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public void OnGameEnd()
    {
        SaveHighScore(); // ゲーム終了時にスコアを保存
    }
}
