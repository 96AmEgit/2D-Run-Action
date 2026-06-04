using UnityEngine;
using UnityEngine.UI; // ★追加：Buttonコンポーネントを操作するために必要
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System.Collections; // ★追加：コルーチン（IEnumerator）を使うために必要

// Unity Ads 4.x 対応版
public class SceneLoaderButton : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Scene Settings")]
    public string targetSceneName; // 移動先のシーン名
    private ObjectDestroyer objectDestroyer;

    [Header("Ad Settings")]
    [SerializeField] private string _androidGameId = "5988835";
    [SerializeField] private string _iOSGameId = "5988834";
    [SerializeField] private bool _testMode = false; // ★本番なのでfalseにしています

    private string _gameId;
    private string _adUnitId = "Interstitial_Android"; // 全画面広告用ID

    // ゲームを遊んだ回数
    private static int gameCount = 0;

    // ★追加：自分自身（リトライボタン）のButtonコンポーネント
    private Button myButton;

    void Awake()
    {
        InitializeAds();
    }

    void Start()
    {
        objectDestroyer = FindObjectOfType<ObjectDestroyer>();
        
        // ★追加：このスクリプトがアタッチされているオブジェクトのButtonコンポーネントを取得
        myButton = GetComponent<Button>();
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

    // --- 2. ボタンから呼ばれる処理（ここをコルーチンに変更） ---
    public void LoadTargetScene()
    {
        // ボタンを押したら、ディレイ付きの処理をスタートする
        StartCoroutine(ExecuteLoadWithDelay());
    }

    // ★追加：0.5秒待ってから処理を行うコルーチン
    private IEnumerator ExecuteLoadWithDelay()
    {
        // ① ボタンを連打できないように無効化する
        if (myButton != null)
        {
            myButton.interactable = false;
        }

        // ② 0.5秒間待機する（この間に広告の裏側の準備などを稼ぐ）
        yield return new WaitForSeconds(0.5f);

        // ③ 元々の「ハイスコア保存」以降の処理を実行する
        if (objectDestroyer != null)
        {
            objectDestroyer.SaveHighScore();
            Debug.Log("ハイスコア保存完了");
        }

        gameCount++;
        Debug.Log($"プレイ回数: {gameCount}");

        if (gameCount % 2 == 0)
        {
            Debug.Log("広告を表示します");
            Advertisement.Show(_adUnitId, this);
            
            // 広告を表示した場合は、広告が終わった後にシーン移動が呼ばれるため、
            // ボタンは移動先のシーンで新しく作られる。ここでinteractableを戻す必要は基本的にない。
        }
        else
        {
            ExecuteSceneLoad();
        }
        
        // （シーン遷移しない場合に備えてボタンを有効に戻す場合はここに myButton.interactable = true; を書きますが、
        // 　今回は必ずシーン遷移するか広告が出るので不要です）
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

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityEngine.Advertisements.UnityAdsLoadError error, string message)
    {
        Debug.Log($"ロード失敗: {message}");
    }
}
