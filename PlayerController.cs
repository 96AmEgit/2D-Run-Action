using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Run & Jump")]
    public float runSpeed = 4.5f;  // ランゲーム向け
    public float jumpForce = 7.5f; // 重めのジャンプ感
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 常に右に一定速度で走る
        rb.velocity = new Vector2(runSpeed, rb.velocity.y);

        // 接地判定
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, groundLayer);

        // ジャンプ（空中では不可）
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
            gameObject.SetActive(false);
        }
    }
}
