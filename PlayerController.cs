using UnityEngine;
using System.Collections; // コルーチン（時間を数える機能）を使うために必要

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10.0f; // ジャンプ力
    public LayerMask groundLayer;   // 床判定レイヤー

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // 無敵中の色変更用
    private bool isGrounded = false; // 地面にいるか？
    private bool isInvincible = false; // 無敵モード中か？

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ゲーム中じゃなければ動かない（重力も止める）
        if (!GameDirector.Instance.isPlaying)
        {
            rb.velocity = Vector2.zero; 
            rb.isKinematic = true; 
            return;
        }
        else
        {
            rb.isKinematic = false; 
        }

        // 1. 走る処理（GameDirectorが決めたスピードで右へ）
        float currentSpeed = GameDirector.Instance.gameSpeed;
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        // 2. ジャンプ処理（地面にいて、画面タップorクリックされたら）
        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false; // ジャンプした瞬間は空中扱い
        }
        
        // 落下死対策（もし床から落ちたらゲームオーバー）
        if (transform.position.y < -10)
        {
            GameDirector.Instance.GameOver();
            gameObject.SetActive(false); // プレイヤーを消す
        }
    }

    // --- 外部（GameDirector）から呼ばれる機能 ---

    // ゲームを最初からやり直すときの位置リセット
    public void ResetPosition()
    {
        // 初期位置（X=0, Y=0付近）に戻す ※ステージに合わせて調整してください
        transform.position = new Vector2(0, 0); 
        rb.velocity = Vector2.zero;
        gameObject.SetActive(true); // 消えていたプレイヤーを表示
        isInvincible = false;       // 無敵は解除
        if(spriteRenderer != null) spriteRenderer.color = Color.white; // 色も戻す
    }

    // 広告を見て復活するときの処理
    public void Revive()
    {
        // 1. 落下死対策：Y座標を少し持ち上げる（または安全な高さにする）
        transform.position = new Vector2(transform.position.x, 0); 
        rb.velocity = Vector2.zero;
        
        // 2. 無敵モードを開始
        StartCoroutine(InvincibleTime());
    }

    // 3秒間の無敵タイム（コルーチン）
    IEnumerator InvincibleTime()
    {
        isInvincible = true;
        
        // 半透明にして「無敵感」を出す
        if(spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); 
        }

        yield return new WaitForSeconds(3.0f); // 3秒待つ

        // 元に戻す
        if(spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        isInvincible = false;
    }

    // --- 地面判定 & 衝突判定 ---
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 床に着地したか？
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // 敵に当たったか？
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ★無敵モード中ならスルーする（死なない）
            if (isInvincible) return;

            Debug.Log("敵に当たった！");
            GameDirector.Instance.GameOver();
            gameObject.SetActive(false); // プレイヤーを消す
        }
    }
}
