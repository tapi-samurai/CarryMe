using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 座標が一定以下になったらリスポーンする

public class RespawnItem : MonoBehaviour
{
    static readonly float RespawnPosY = 12.0f;
    static readonly float TriggerPosY = -30.0f;

    Vector3 m_spawnPos;

    private void OnEnable()
    {
        // オブジェクトが有効化した時の座標を生成
        m_spawnPos = transform.position;
        m_spawnPos.y = RespawnPosY;
    }

    public void Respawn()
    {
        transform.position = m_spawnPos;
        transform.rotation = Quaternion.identity;

        // 物理的な挙動をリセット
        if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 座標が一定以下であればリスポーン位置に設定
        if (transform.position.y > TriggerPosY) return;

        Respawn();
    }
}
