using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; // 広告用

// Unity Ads 4.x 対応版
public class SceneLoaderButton : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Scene Settings")]
    public string targetSceneName; // 移動先のシーン名
    private ObjectDestroyer objectDestroyer;

    [Header("Ad Settings")]
    [SerializeField] private string _androidGameId = "5988835";
    [SerializeField] private string _iOSGameId = "5988834";
    [SerializeField] private bool _testMode = true; // ★リリース時は false

    private string _gameId;
    private string _adUnitId = "Interstitial_Android"; // 全画面広告用ID

    // ゲームを遊んだ回数
    private static int gameCount = 0;

    void Awake()
    {
        InitializeAds();
    }

    void Start()
    {
        objectDestroyer = FindObjectOfType<ObjectDestroyer>();
    }

    // --- 1. 初期化 ---
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

    // --- 2. ボタンから呼ばれる処理 ---
    public void LoadTargetScene()
    {
        // ① ハイスコア保存
        if (objectDestroyer != null)
        {
            objectDestroyer.SaveHighScore();
            Debug.Log("ハイスコア保存完了");
        }

        // ② 回数をカウント
        gameCount++;
        Debug.Log($"プレイ回数: {gameCount}");

        // ③ 2回に1回（偶数回）広告を出す
        if (gameCount % 2 == 0)
        {
            Debug.Log("広告を表示します");
            Advertisement.Show(_adUnitId, this);
        }
        else
        {
            ExecuteSceneLoad();
        }
    }

    // --- 3. シーン移動 ---
    private void ExecuteSceneLoad()
    {
        SceneManager.LoadScene(targetSceneName);
    }

    // --- 4. 広告の結果受け取り（IUnityAdsShowListener） ---
    
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("広告視聴完了。移動します。");
            ExecuteSceneLoad();
            LoadAd(); // 次回用にロード
        }
    }

    // エラーやスキップ時も移動させる
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"広告表示エラー: {message}");
        ExecuteSceneLoad();
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }


    // --- 5. 初期化とロードのコールバック（エラー回避のためフルネーム記述） ---

    public void OnInitializationComplete() 
    { 
        LoadAd(); 
    }

    public void OnInitializationFailed(UnityEngine.Advertisements.UnityAdsInitializationError error, string message) 
    { 
        Debug.Log($"初期化失敗: {message}"); 
    }

    public void OnUnityAdsAdLoaded(string adUnitId) 
    { 
        Debug.Log("Adロード完了"); 
    }

    // ★ここがエラーの原因だった場所。フルネーム(UnityEngine.Advertisements.~)で記述して回避
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityEngine.Advertisements.UnityAdsLoadError error, string message)
    {
        Debug.Log($"ロード失敗: {message}");
    }
}
