using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 10.0f; // ジャンプ力
    public LayerMask groundLayer;   // 床判定レイヤー（今回はタグ判定で簡易化も可）

    private Rigidbody2D rb;
    private bool isGrounded = false; // 地面にいるか？

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ゲーム中じゃなければ動かない
        if (!GameDirector.Instance.isPlaying)
        {
            rb.velocity = Vector2.zero; // 止まる
            rb.isKinematic = true; // 重力も止める（空中待機など用）
            return;
        }
        else
        {
            rb.isKinematic = false; // ゲーム中は物理演算ON
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

    // --- 地面判定 & 衝突判定 ---
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 床に着地したか？
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // 敵に当たったか？
        if (collision.gameObject.CompareTag("Enemy")) // 後で敵を作った時に使います
        {
            Debug.Log("敵に当たった！");
            GameDirector.Instance.GameOver();
            gameObject.SetActive(false); // プレイヤーを消す（演出次第）
        }
    }
}
