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
    public TextMeshProUGUI scoreText;

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

    void UpdateScoreText()
    {
        scoreText.text = currentScore.ToString();
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
