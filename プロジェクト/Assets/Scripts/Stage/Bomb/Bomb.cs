using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    const float GraceTime = 3.0f;
    const float MinColorBlinkInterval = 1.0f;
    const float MaxColorBlinkInterval = 20.0f;

    [SerializeField] GameObject m_explosionEffect;
    [SerializeField] SphereCollider m_triggerCollider;
    [SerializeField] float m_power;
    [SerializeField] Material m_material;
    [SerializeField] AudioClip m_se;
    [SerializeField] bool m_enabled;

    List<GameObject> m_explosionObjectList = new List<GameObject>();
    List<GameObject> m_deleteList = new List<GameObject>();
    float m_timeLimit;

    private void Awake()
    {
        Initialize();
    }

    // 初期化
    void Initialize()
    {
        m_timeLimit = GraceTime;
        m_enabled = false;

        m_material.SetColor("_BaseColor", Color.black);
        GetComponent<CarryObject>().enabled = true;
    }

    public void OnIgnition()
    {
        m_enabled = true;
    }

    private void FixedUpdate()
    {
        if (m_enabled == false) return;

        // 猶予時間を減らす
        m_timeLimit -= Time.deltaTime;

        // マテリアルの点滅処理
        float interval = Mathf.Lerp(MinColorBlinkInterval, MaxColorBlinkInterval, 1 - m_timeLimit / GraceTime);
        float x = Mathf.PI * (1 - (m_timeLimit / GraceTime)) * interval;
        while (x > Mathf.PI)
        {
            x -= Mathf.PI;
        }
        float red = Mathf.Sin(x);
        Color color = new Color(red, 0, 0, 1);
        m_material.SetColor("_BaseColor", color);

        // 猶予時間が無くなったら
        if (m_timeLimit > 0) return;

        // 爆発
        // 吹き飛ぶ速度は相手との距離で決まる
        foreach (GameObject gameObject in m_explosionObjectList)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if(gameObject.TryGetComponent<CarryObject>(out CarryObject carryObject)) { carryObject.enabled = true; }

            // 吹き飛ばす
            if (rigidbody.isKinematic)
            {
                // おおよそ半分のコライダーを停止する
                if(gameObject.TryGetComponent<BoxCollider>(out BoxCollider boxCollider))
                {
                    int value = Random.Range(0, 2);
                    boxCollider.enabled = value == 0 ? true : false;
                }

                rigidbody.isKinematic = false;
                rigidbody.AddExplosionForce(m_power, transform.position, m_triggerCollider.radius, 0, ForceMode.Impulse);

                // 吹き飛ばしたオブジェクトは責任をもって回収
                StageSceneManager.instance.AddStage(gameObject);
            }

            // 爆弾を使ってステージを変化させたことを報告
            StageCounter.OnUseItem((int)StageCounter.Item.Bomb);
        }

        // エフェクトを生成
        Instantiate(m_explosionEffect, transform.position, Quaternion.Euler(-90, 0, 0));

        // SE
        SoundEffect.Play25D(m_se, null, 0.3f, 1, false);

        // 初期化して再配置
        GetComponent<RespawnItem>().Respawn();
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 条件を満たさないオブジェクトは吹き飛ばすリストに追加しない
        // ・Rigidbodyを持っていない
        // ・担げないオブジェクト
        // ・プレイヤー
        if(
            other.TryGetComponent<CarryObject>(out CarryObject carryObject) == false ||
            other.TryGetComponent<Rigidbody>(out Rigidbody rigidbody) == false ||
            other.TryGetComponent<Player>(out Player player)
            )
        {
            return;
        }

        m_explosionObjectList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // 吹き飛ばすリストに入っていないならスキップ
        if (m_explosionObjectList.Contains(other.gameObject) == false) return;

        m_explosionObjectList.Remove(other.gameObject);
    }
}
