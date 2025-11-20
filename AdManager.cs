using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("ID Settings")]
    [SerializeField] private string _androidGameId = "5988835"; // あなたのID
    [SerializeField] private string _iOSGameId = "5988834";
    [SerializeField] private bool _testMode = true; // ★リリース時は必ず false にする！
    
    private string _gameId;
    private string _rewardedVideoId = "Rewarded_Android"; // 決まり文句

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    // --- 読み込み ---
    public void LoadAd()
    {
        Debug.Log("広告読み込み開始...");
        Advertisement.Load(_rewardedVideoId, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("広告初期化完了");
        LoadAd(); // すぐに読み込んでおく
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"初期化失敗: {message}");
    }

    // --- 表示（ボタンから呼ぶやつ） ---
    public void ShowRewardAd()
    {
        Advertisement.Show(_rewardedVideoId, this);
    }

    // --- 結果の受け取り ---
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_rewardedVideoId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("広告視聴完了！復活させます。");
            // ★ここでGameDirectorに指令を出す
            GameDirector.Instance.ReviveGame();
            
            // 次回のためにまた読み込んでおく
            LoadAd();
        }
    }

    // エラー処理などは省略（ログ出しのみ）
    public void OnUnityAdsAdLoaded(string adUnitId) { Debug.Log("広告準備OK"); }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) { Debug.Log($"読み込み失敗: {message}"); }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) { Debug.Log($"表示失敗: {message}"); }
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
