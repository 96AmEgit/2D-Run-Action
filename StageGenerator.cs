using UnityEngine;
using System.Collections.Generic; // リストを使うために必要

public class StageGenerator : MonoBehaviour
{
    [Header("Generated Objects")]
    public GameObject groundPrefab; // 床のプレハブ
    public GameObject enemyPrefab;  // 敵のプレハブ

    [Header("Settings")]
    public int startGroundNum = 5;  // 最初に作っておく床の数
    public float groundWidth = 20f; // 床1枚の幅（GroundのScale Xと同じくらいに）
    
    private Transform player;       // プレイヤーの位置用
    private float spawnPointX = 0f; // 次に床を置くX座標
    
    // 作った床を覚えておくリスト（後で消すため）
    private List<GameObject> currentGrounds = new List<GameObject>();

    void Start()
    {
        // プレイヤーを探す
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 最初に何枚か床を作っておく
        for (int i = 0; i < startGroundNum; i++)
        {
            SpawnGround(i > 1); // 最初の2枚くらいは敵を出さない
        }
    }

    void Update()
    {
        // ゲーム中だけ動く
        if (!GameDirector.Instance.isPlaying) return;

        // プレイヤーが生成ポイントに近づいたら（差が床2枚分くらいになったら）
        if (player.position.x > spawnPointX - (groundWidth * 2))
        {
            SpawnGround(true); // 新しい床と敵を作る
            DeleteOldGround(); // 古い床を消す
        }
    }

    // 床と敵を生成する関数
    void SpawnGround(bool spawnEnemy)
    {
        // 1. 床を作る
        GameObject newGround = Instantiate(groundPrefab);
        newGround.transform.position = new Vector2(spawnPointX, -2f); // Yは床の高さに合わせて調整
        spawnPointX += groundWidth; // 次の生成位置をずらす
        
        currentGrounds.Add(newGround); // リストに追加

        // 2. 敵を作る（確率で、かつspawnEnemyフラグがONなら）
        if (spawnEnemy && Random.Range(0, 100) < 50) // 50%の確率で出現
        {
            GameObject enemy = Instantiate(enemyPrefab);
            // 床の上のどこかランダムな位置に置く
            float randomX = Random.Range(spawnPointX - groundWidth + 2f, spawnPointX - 2f);
            enemy.transform.position = new Vector2(randomX, -0.5f); // Yは敵の高さに合わせて調整
        }
    }

    // 通り過ぎた古い床を消す関数
    void DeleteOldGround()
    {
        // リストの最初の床が、プレイヤーよりはるか後ろに行ったら
        if (currentGrounds.Count > 0)
        {
            GameObject oldGround = currentGrounds[0];
            if (player.position.x - oldGround.transform.position.x > groundWidth * 2)
            {
                Destroy(oldGround);       // ゲームから削除
                currentGrounds.RemoveAt(0); // リストから削除
            }
        }
    }
}
