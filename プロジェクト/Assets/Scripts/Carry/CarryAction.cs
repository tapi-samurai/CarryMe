using System.Collections.Generic;
using UnityEngine;

// 担がれている時の処理の基底クラス
public class CarryAction
{
    protected GameObject m_owner;
    protected GameObject m_parent;
    protected Rigidbody m_rigidbody;

    public virtual void OnStart(GameObject owner, GameObject parent)
    {
        m_owner = owner;
        m_parent = parent;

        m_rigidbody = m_owner.GetComponent<Rigidbody>();

        // 物理的な挙動を止める
        m_rigidbody.isKinematic = true;

        // 自身を担いだオブジェクトの子にする
        m_owner.transform.parent = m_parent.transform;
    }

    public virtual void OnUpdate() { }

    public virtual void OnEnd()
    {
        // 親子関係を解除する
        m_owner.transform.parent = null;

        // 物理的な挙動を再開
        m_rigidbody.isKinematic = false;

        m_rigidbody = null;
        m_owner = null;
        m_parent = null;
    }
}

// Rigidbodyのみのオブジェクトが担がれているときの処理
public class CarryRigid : CarryAction
{
    const float ThrownPower = 7.0f;   // 投げ飛ばされる強さ

    BoxCollider m_parentCollider;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();

        // 座標と回転を設定する
        Vector3 pos = Vector3.zero;
        pos.y = m_parentCollider.center.y + (m_parentCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        // プレイヤーの頭上に移動
        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;
    }

    public override void OnEnd()
    {
        // 予め物理的な挙動を再開しておく
        m_rigidbody.isKinematic = false;

        // 投げ飛ばす
        Vector3 vec = m_parent.transform.forward * ThrownPower / m_rigidbody.mass +
            m_parent.transform.up * ThrownPower / m_rigidbody.mass;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        base.OnEnd();
    }
}

// プレイヤー
public class CarryPlayer : CarryAction
{
    const float OffsetY = 0.1f; // 担いだ時のY座標オフセット

    Player m_player;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_player = m_owner.GetComponent<Player>();
        BoxCollider m_ownerCollider = m_owner.GetComponent<BoxCollider>();
        Vector3 pos = Vector3.zero;

        // Boxならプレイヤー
        // BoxColliderがついていなければコメントに担がれている
        if(m_parent.TryGetComponent<BoxCollider>(out BoxCollider boxCollider) == true)
        {
            // 担いでいるプレイヤーの頭上に設定
            pos.y = (m_ownerCollider.size.y / 2) + (boxCollider.size.y / 2) + OffsetY;
        }
        else
        {
            // コライダーの中心に設定
            pos = new Vector3(0, -m_ownerCollider.size.y, 0);
        }

        // 座標と回転を適用する
        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;

        // 移動の機能を止める
        m_player.enabled = false;
    }

    public override void OnEnd()
    {
        // 物理的な挙動を再開
        m_rigidbody.isKinematic = false;

        // 移動の機能を再開して投げ飛ばす
        m_player.enabled = true;
        m_player.OnThrown();

        base.OnEnd();
    }
}

// プロペラ
public class CarryPropeller : CarryAction
{
    BoxCollider m_parentCollider;
    PropellerRotate m_propellerRotate;
    Player m_player;
    float m_startPos;

    const float MaxHeight = 5.0f;       // 最高高度
    const float MaxRisingTime = 2.0f;   // 最高高度に到達するまでの時間
    float m_elapsedTime;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();
        m_propellerRotate = m_owner.GetComponent<PropellerRotate>();
        m_player = m_parent.GetComponent<Player>();
        m_elapsedTime = 0;
        m_startPos = m_parent.transform.position.y;

        // 座標と回転を設定する
        Vector3 pos = new Vector3(0, m_parentCollider.center.y + m_parentCollider.size.y / 2 + m_owner.transform.localScale.y / 2, 0);
        m_owner.transform.localPosition = pos;
        m_owner.transform.rotation = Quaternion.identity;

        // プレイヤーの重力を無効化する
        m_player.IsGravity = false;

        // プロペラの回転を開始する
        m_propellerRotate.OnStart();

        // プロペラの使用をカウンターに報告
        StageCounter.OnUseItem((int)StageCounter.Item.Propeller);
    }

    public override void OnUpdate()
    {
        m_elapsedTime = m_elapsedTime < MaxRisingTime ? m_elapsedTime + Time.deltaTime : MaxRisingTime;

        // 一定の高度まで上昇する
        Vector3 parentPos = m_parent.transform.position;
        float t = Mathf.Sqrt(1 - Mathf.Pow(m_elapsedTime / MaxRisingTime - 1, 2));
        parentPos.y = Mathf.Lerp(m_startPos, MaxHeight, t);
        m_parent.transform.position = parentPos;
    }

    public override void OnEnd()
    {
        // プレイヤーの重力を有効化する
        m_player.IsGravity = true;

        // プロペラの回転を停止する
        m_propellerRotate.OnStop();

        base.OnEnd();
    }
}

// ボム
public class CarryBomb : CarryAction
{
    const float ThrownPower = 5.0f; // 投げ飛ばされる強さ

    BoxCollider m_boxCollider;

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_boxCollider = m_parent.GetComponent<BoxCollider>();

        // 座標と回転を設定する
        Vector3 pos = Vector3.zero;
        pos.y = m_boxCollider.center.y + (m_boxCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;
    }

    public override void OnEnd()
    {
        m_rigidbody.isKinematic = false;

        // 着火
        m_owner.GetComponent<Bomb>().OnIgnition();

        // 投げ飛ばす
        Vector3 vec = m_parent.transform.forward * ThrownPower + m_parent.transform.up * ThrownPower;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        // このオブジェクトを持てないようにする
        m_owner.GetComponent<CarryObject>().enabled = false;

        base.OnEnd();
    }
}

// 壁
public class CarryWall : CarryAction
{
    BoxCollider m_parentCollider;
    List<Transform> m_deleteTransformList = new List<Transform>();

    public override void OnStart(GameObject owner, GameObject parent)
    {
        base.OnStart(owner, parent);

        m_parentCollider = m_parent.GetComponent<BoxCollider>();

        // 座標と回転を設定する
        Vector3 pos = Vector3.zero;
        pos.y = m_parentCollider.center.y + (m_parentCollider.size.y / 2) + (m_owner.transform.localScale.y / 2);

        m_owner.transform.localPosition = pos;
        m_owner.transform.localRotation = Quaternion.identity;

        // 報告
        StageCounter.OnUseItem((int)StageCounter.Item.Wall);
        
        // 壁オブジェクトのうちRigidbodyのついているものは切り離す
        GetChildren(owner);
        if(m_deleteTransformList != null)
        {
            foreach(Transform transform in m_deleteTransformList)
            {
                transform.parent = null;

                // 切り離したオブジェクトを集める
                StageSceneManager.instance.AddStage(transform.gameObject);
            }
            m_deleteTransformList = null;
        }
    }

    void GetChildren(GameObject gameObject)
    {
        // すべての子オブジェクトのRigidbodyを起動する
        Transform children = gameObject.transform.GetComponentInChildren<Transform>();
        if (children.childCount == 0) return;

        foreach (Transform transform in children)
        {
            if(transform.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
                m_deleteTransformList.Add(transform);
            }

            // すべての子オブジェクトを再帰関数で探す
            GetChildren(transform.gameObject);
        }
    }

    public override void OnEnd()
    {
        m_rigidbody.isKinematic = false;

        // 投げ飛ばす
        float power = 7;
        Vector3 vec = m_parent.transform.forward * power + m_parent.transform.up * power;
        m_rigidbody.AddForce(vec, ForceMode.Impulse);

        base.OnEnd();
    }
}