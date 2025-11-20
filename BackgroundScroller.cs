using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeedFactor = 0.5f; // 床より少し遅くすると遠近感が出る（0.1〜1.0）
    public float width = 30f; // 背景画像の幅（Scale Xと同じか、画像の幅に合わせる）

    void Update()
    {
        // ゲーム中以外は動かない
        if (!GameDirector.Instance.isPlaying) return;

        // 1. 左に動かす
        // GameDirectorのスピードに合わせて動く
        float speed = GameDirector.Instance.gameSpeed * scrollSpeedFactor;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. 画面左端（-width）を越えたら、右端（width）にワープさせる
        // ※自分自身のX座標が -width より小さくなったら
        if (transform.position.x < -width)
        {
            // 右に2個分（width * 2）移動させて、先頭につなげる
            transform.position = new Vector2(transform.position.x + (width * 2), transform.position.y);
        }
    }
}
