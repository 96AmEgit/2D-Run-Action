using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Unity Ads")]
    [SerializeField] private string androidGameId;
    [SerializeField] private string rewardedAdId = "Rewarded_Android";
    private string gameId;
    private bool adLoaded = false;

    void Start()
    {
#if UNITY_ANDROID
        gameId = androidGameId;
#endif
        Advertisement.Initialize(gameId, false, this);
    }

    public void LoadAd()
    {
        Advertisement.Load(rewardedAdId, this);
    }

    public void ShowRewardAd()
    {
        if (adLoaded)
        {
            Advertisement.Show(rewardedAdId, this);
        }
        else
        {
            LoadAd();
        }
    }

    // Init callbacks
    public void OnInitializationComplete()
    {
        LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Init Fail: {error} - {message}");
    }

    // Load callbacks
    public void OnUnityAdsAdLoaded(string placementId)
    {
        adLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        adLoaded = false;
    }

    // Show callbacks
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }
    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == rewardedAdId &&
            showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            // ⭐ここで復活処理⭐
            PlayerController player = FindObjectOfType<PlayerController>();
            player.gameObject.SetActive(true);
            GameManager.Instance.StartGame();
        }

        LoadAd(); // 次の広告をロード
    }
}