using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; // 広告用

// 古いバージョン(Legacy)に対応したインターフェースに変更しました
public class SceneLoaderButton : MonoBehaviour, IUnityAdsListener
{
    [Header("Scene Settings")]
    public string targetSceneName; // 移動先のシーン名
    private ObjectDestroyer objectDestroyer;

    [Header("Ad Settings")]
    [SerializeField] private string _androidGameId = "5988835"; // あなたのAndroid ID
    [SerializeField] private string _iOSGameId = "5988834";
    [SerializeField] private bool _testMode = true; // ★リリース時は false に！

    private string _gameId;
    private string _adUnitId = "Interstitial_Android"; // 全画面広告用ID

    // ゲームを遊んだ回数
    private static int gameCount = 0;

    void Start()
    {
        objectDestroyer = FindObjectOfType<ObjectDestroyer>();

        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;

        // ★重要：リスナー（見張り番）を登録
        Advertisement.AddListener(this);

        // 初期化（Legacy版の書き方）
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(_gameId, _testMode);
        }
    }

    // --- ボタンから呼ばれるメイン処理 ---
    public void LoadTargetScene()
    {
        // ① ハイスコア保存
        if (objectDestroyer != null)
        {
            objectDestroyer.SaveHighScore();
            Debug.Log("ハイスコア保存完了: " + PlayerPrefs.GetInt("HighScore", 0));
        }

        // ② 回数をカウントアップ
        gameCount++;
        Debug.Log($"プレイ回数: {gameCount}");

        // ③ 2回に1回（偶数のとき）広告を出す
        if (gameCount % 2 == 0)
        {
            // 広告の準備ができているか確認して表示
            if (Advertisement.IsReady(_adUnitId))
            {
                Debug.Log("広告を表示します");
                Advertisement.Show(_adUnitId);
            }
            else
            {
                Debug.Log("広告の準備ができていません。そのまま移動します。");
                ExecuteSceneLoad();
            }
        }
        else
        {
            // 奇数のときはそのまま移動
            ExecuteSceneLoad();
        }
    }

    // --- 実際のシーン移動処理 ---
    private void ExecuteSceneLoad()
    {
        // リスナー解除（エラー防止）
        Advertisement.RemoveListener(this);
        SceneManager.LoadScene(targetSceneName);
    }

    // --- IUnityAdsListener の必須機能（広告の結果を受け取る） ---

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // 広告を見終わった（またはスキップした）場合
        if (placementId == _adUnitId)
        {
            Debug.Log("広告終了。シーンへ移動します。");
            ExecuteSceneLoad();
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // 広告が始まった瞬間（今回は何もしない）
    }

    public void OnUnityAdsReady(string placementId)
    {
        // 広告の読み込みが完了した瞬間（今回は何もしない）
    }

    public void OnUnityAdsDidError(string message)
    {
        // エラーが起きた場合
        Debug.Log($"広告エラー: {message} -> 強制移動します");
        ExecuteSceneLoad();
    }
}
