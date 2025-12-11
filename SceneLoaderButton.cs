using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; // 広告用

public class SceneLoaderButton : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
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

    // ゲームを遊んだ回数（staticなのでシーン移動しても数字が残る）
    private static int gameCount = 0;

    void Awake()
    {
        InitializeAds();
    }

    void Start()
    {
        objectDestroyer = FindObjectOfType<ObjectDestroyer>(); // スコア管理クラスを取得
    }

    // --- 1. 広告の初期化 ---
    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
        else
        {
            LoadAd();
        }
    }

    public void LoadAd()
    {
        Advertisement.Load(_adUnitId, this);
    }

    // --- 2. ボタンから呼ばれるメイン処理 ---
    public void LoadTargetScene()
    {
        // ① まずハイスコアを保存（広告前に確定させる）
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
            Debug.Log("広告を表示します");
            ShowAd();
        }
        else
        {
            // 奇数のときはそのまま移動
            ExecuteSceneLoad();
        }
    }

    // 広告表示の実行
    public void ShowAd()
    {
        // 読み込みが完了していれば表示
        Advertisement.Show(_adUnitId, this);
    }

    // --- 3. 実際のシーン移動処理 ---
    private void ExecuteSceneLoad()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    // --- 4. 広告コールバック（見終わった後の処理） ---
    
    // 広告が閉じられたら呼ばれる
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("広告視聴完了。シーンへ移動します。");
            ExecuteSceneLoad(); // ★ここでも移動を実行
            
            LoadAd(); // 次回のためにロード
        }
    }

    // 広告エラー時やスキップ時も、閉じ込められないように移動させる
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"広告エラー: {message} -> 強制移動します");
        ExecuteSceneLoad();
    }

    // 初期化・ロード関連のコールバック（ログ出しのみ）
    public void OnInitializationComplete() { LoadAd(); }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message) { Debug.Log($"Init失敗: {message}"); }
    public void OnUnityAdsAdLoaded(string adUnitId) { Debug.Log("Adロード完了"); }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) { Debug.Log($"Load失敗: {message}"); }
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
