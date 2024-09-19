using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] bool m_mainCharactor;   // 操作方法の判定用
    [SerializeField] bool m_canMove;
    [SerializeField] bool m_canJump;

    public bool MainCharactor { get { return m_mainCharactor; } }

    [Header("Control")]
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_thrownPower;   // 投げられる速度
    [SerializeField] float m_gravity;
    [SerializeField] Vector3 m_rayOffset;
    [SerializeField] float m_rayLength;

    [Header("UI")]
    [SerializeField] Image m_image;

    List<GameObject> m_carryList = new List<GameObject>();  // 担げるオブジェクトのリスト
    GameObject m_carryingObject;
    Rigidbody m_rigidbody;
    Animator m_animator;

    public GameObject CarryingObject { get { return m_carryingObject; } }

    Vector2 m_inputMove;
    bool m_isLanding;
    bool m_isThrowing;     // 投げられている状態かどうか
    bool m_isGravity;      // 重力の影響を受けるか

    public bool IsGravity { set { m_isGravity = value; } }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();

        m_carryList = new List<GameObject>();
        m_isLanding = false;
        m_isGravity = true;
    }

    private void OnEnable()
    {
        m_carryList.Clear();
        transform.parent = null;

        // 担いでいるものがあれば離す
        if(m_carryingObject)
        {
            m_carryingObject.GetComponent<CarryObject>().OnDrop();
            m_carryingObject = null;
        }

        // UIを非表示
        m_image.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        m_inputMove = Vector2.zero;
        m_animator.SetBool("Move", false);
    }

    public void OnMove(Vector2 inputMove)
    {
        m_inputMove = inputMove;
    }

    public void OnJump()
    {
        if (m_canJump && m_isLanding)
        {
            m_rigidbody.AddForce(new Vector3(0, m_jumpPower, 0), ForceMode.Impulse);
            m_isLanding = false;
        }
    }

    public void OnCarryAndDrop()
    {
        if(!m_carryingObject && m_carryList.Count > 0)
        {
            // 担げるオブジェクトの中で最も距離が近いものを指定する
            foreach(GameObject gameObject in m_carryList)
            {
                if(m_carryingObject == null)
                {
                    m_carryingObject = gameObject;
                    continue;
                }

                float length1 = (m_carryingObject.transform.position - transform.position).sqrMagnitude;
                float length2 = (gameObject.transform.position - transform.position).sqrMagnitude;

                if(length1 > length2)
                {
                    m_carryingObject = gameObject;

                    // UIを非表示
                    m_image.gameObject.SetActive(false);
                }
            }

            // 担ぐ処理
            m_carryingObject.GetComponent<CarryObject>().OnCarry(gameObject);

            // アニメーション設定
            m_animator.SetBool("Carry", true);

            return;
        }

        if(m_carryingObject)
        {
            // 降ろす処理
            m_carryingObject.GetComponent<CarryObject>().OnDrop();
            m_carryingObject = null;

            // アニメーション設定
            m_animator.SetBool("Carry", false);

            return;
        }
    }

    // 外から呼び出す関数
    public void OnThrown(float thrownPower = -1)
    {
        // 移動量をリセット
        m_rigidbody.velocity = Vector3.zero;

        // 投げられる処理
        m_isLanding = false;
        m_isThrowing = true;

        // 引数で投げる強さが渡されたらそれを使う
        // 縦にやや強くとばす
        float power = thrownPower == -1 ? m_thrownPower : thrownPower;
        m_rigidbody.AddForce(transform.forward * power + transform.up * power * 1.25f, ForceMode.Impulse);
    }

    bool IsGround()
    {
        // MEMO BoxRayを飛ばしたほうがよくないか
        // ５本のレイを飛ばして接地判定
        for(int x = -1; x <= 1; x += 2)
        {
            for(int z = -1; z <= 1; z += 2)
            {
                Vector3 pos = transform.rotation * new Vector3(m_rayOffset.x * x, m_rayOffset.y, m_rayOffset.z * z) + transform.position;
                Ray ray = new Ray(pos, Vector3.down);

                Debug.DrawRay(pos, Vector3.down * m_rayLength);

                foreach (RaycastHit hit in Physics.RaycastAll(ray, m_rayLength))
                {
                    // 自分のコライダーならスキップ
                    if (hit.collider.gameObject == gameObject) continue;

                    // Trigger用のコライダーならスキップ
                    if (hit.collider.isTrigger) continue;

                    // アニメーション設定
                    m_animator.SetBool("Landing", true);

                    return true;
                }
            }
        }

        // アニメーション設定
        m_animator.SetBool("Landing", false);

        return false;
    }

    void SetLocalGravity()
    {
        // 着地中かつ入力がない場合は、弱めの重力を入れる
        m_rigidbody.AddForce(new Vector3(0, -m_gravity, 0), ForceMode.Force);
    }

    private void FixedUpdate()
    {
        // 接地判定
        m_isLanding = IsGround();

        // 投げられたら着地するまで移動出来なくなる
        if(m_isThrowing && m_isLanding)
        {
            m_isThrowing = false;
        }

        // 移動
        // 投げられている間は動けない
        if(m_canMove && !m_isThrowing)
        {
            // カメラの角度に合わせて移動
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 velocity = cameraForward * m_inputMove.y * m_moveSpeed + Camera.main.transform.right * m_inputMove.x * m_moveSpeed;

            // ゆっくりと進行方向を向く
            if(m_inputMove != Vector2.zero)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, Quaternion.LookRotation(velocity), 0.3f);
            }

            // 重力を反映
            float gravity = m_rigidbody.velocity.y;

            // 移動量を反映
            m_rigidbody.velocity = velocity + new Vector3(0, gravity, 0);

            // アニメーション設定
            bool moveFlag = velocity != Vector3.zero ? true : false;
            m_animator.SetBool("Move", moveFlag);
        }

        // 重力を適用
        if(m_isGravity) SetLocalGravity();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 自分ならスルー
        // 既に入っているならスルー
        // Triggerならスキップ
        // 担げるオブジェクトでなければスキップ
        // 自分が担がれていたらスキップ
        if (
            other.gameObject == gameObject ||
            m_carryList.Contains(other.gameObject) ||
            other.isTrigger ||
            other.TryGetComponent<CarryObject>(out CarryObject carryObject) == false ||
            carryObject.enabled == false ||
            GetComponent<CarryObject>().IsCarrying == true
            )
        {
            return;
        }

        // リストが空で、何も担いでいなければUIを表示
        if(m_carryList.Count == 0 && m_carryingObject == null)
        {
            m_image.gameObject.SetActive(true);
        }

        // 近くにある担げるオブジェクトをリストに追加する
        m_carryList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // 入っていないならスキップ
        if (m_carryList.Contains(other.gameObject) == false) return;

        // 距離が離れたオブジェクトをリストから除外する
        m_carryList.Remove(other.gameObject);

        // リストが空になったらUIを非表示にする
        if (m_carryList.Count == 0)
        {
            m_image.gameObject.SetActive(false);
        }
    }
}
