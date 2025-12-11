using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderButton : MonoBehaviour
{
    public string targetSceneName;
    private ObjectDestroyer objectDestroyer;

    void Start()
    {
        objectDestroyer = FindObjectOfType<ObjectDestroyer>(); // ゲーム内のスコア管理クラスを取得
    }

    public void LoadTargetScene()
    {
        if (objectDestroyer != null)
        {
            objectDestroyer.SaveHighScore(); // ハイスコアを保存
            Debug.Log("ハイスコア保存: " + PlayerPrefs.GetInt("HighScore", 0)); // 確認用
        }
        SceneManager.LoadScene(targetSceneName);
    }
}
