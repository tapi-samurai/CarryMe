using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// このスクリプトがついていれば、プレイヤーが担ぐことが出来る

public class CarryObject : MonoBehaviour
{
    [SerializeField] CarryActionType m_carryActionType; // 担がれたときの処理を指定
    CarryAction m_carryAction;                          // 担がれたときのアクションの基底クラス

    bool m_isCarrying;

    public bool IsCarrying { get { return m_isCarrying; } }

    enum CarryActionType
    {
        RigidbodyOnly,
        Player,
        Propeller,
        Bomb,
        Wall,
    }

    CarryAction[] m_actions =
    {
        new CarryRigid(),
        new CarryPlayer(),
        new CarryPropeller(),
        new CarryBomb(),
        new CarryWall(),
    };

    private void Awake()
    {
        m_carryAction = m_actions[(int)(m_carryActionType)];
        m_actions = null;
    }

    public void OnCarry(GameObject parent)
    {
        m_carryAction.OnStart(gameObject, parent);
        m_isCarrying = true;
    }

    private void Update()
    {
        if(m_isCarrying)
        {
            m_carryAction.OnUpdate();
        }
    }

    public void OnDrop()
    {
        m_carryAction.OnEnd();
        m_isCarrying = false;
    }
}
