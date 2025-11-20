using UnityEngine;
using UnityEngine.UI; // UIを扱うために必要

public class GameDirector : MonoBehaviour
{
    // シングルトン（どこからでも呼べるようにする）
    public static GameDirector Instance;

    [Header("UI Groups (ヒエラルキーからセットする)")]
    public GameObject titleGroup;
    public GameObject gameGroup;
    public GameObject resultGroup;

    [Header("Game Settings")]
    public bool isPlaying = false; // ゲーム中かどうか
    public float gameSpeed = 1.0f; // 現在のゲームスピード
    public float baseSpeed = 5.0f; // 基本スピード
    public float speedUpRate = 0.1f; // 1秒ごとの加速量

    private float scoreTime = 0f; // 経過時間

    void Awake()
    {
        // 自分自身をセット
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 最初はタイトル画面へ
        ShowTitle();
    }

    void Update()
    {
        // ゲーム中の処理
        if (isPlaying)
        {
            // 1. スコア（時間）を加算
            scoreTime += Time.deltaTime;

            // 2. 時間経過で徐々にスピードアップ
            //    (例: 10秒でスピードが +1 される)
            gameSpeed = baseSpeed + (scoreTime * speedUpRate);

            // ここにUIのスコア更新処理などを後で書く
            // Debug.Log("Score: " + scoreTime.ToString("F1") + " / Speed: " + gameSpeed);
        }
    }

    // --- 画面切り替え用関数 ---

    public void ShowTitle()
    {
        isPlaying = false;
        titleGroup.SetActive(true);
        gameGroup.SetActive(false);
        resultGroup.SetActive(false);
    }

    // 変数を追加：一度復活したかどうかのフラグ
    private bool hasRevived = false; 

    public void StartGame()
    {
        scoreTime = 0f;
        gameSpeed = baseSpeed;
        isPlaying = true;
        hasRevived = false; // フラグもリセット

        titleGroup.SetActive(false);
        gameGroup.SetActive(true);
        resultGroup.SetActive(false);

        // プレイヤーを表示して位置を戻す（プレイヤー側の関数を呼ぶ）
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null) player.GetComponent<PlayerController>().ResetPosition();
    }

    public void GameOver()
    {
        isPlaying = false;
        titleGroup.SetActive(false);
        gameGroup.SetActive(true);
        resultGroup.SetActive(true);

        // ★復活ボタンの制御：もう復活済みならボタンを隠す
        GameObject adButton = resultGroup.transform.Find("AdButton").gameObject; 
        // ↑名前が違う場合はInspectorで確認して合わせるか、public変数で持たせてもOK
        if(adButton != null)
        {
            adButton.SetActive(!hasRevived); // まだ復活してないなら表示
        }
    }

    // 広告視聴後に呼ばれる本当の復活処理
    public void ReviveGame()
    {
        hasRevived = true; // 復活済みフラグON
        resultGroup.SetActive(false); // リザルト消す
        isPlaying = true; // 時間を動かす

        // プレイヤーを復活（無敵モード付きで）
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(true); // 表示
            player.GetComponent<PlayerController>().Revive(); // 復活処理
        }
    }
