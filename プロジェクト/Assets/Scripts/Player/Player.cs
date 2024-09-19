using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] bool m_mainCharactor;   // ������@�̔���p
    [SerializeField] bool m_canMove;
    [SerializeField] bool m_canJump;

    public bool MainCharactor { get { return m_mainCharactor; } }

    [Header("Control")]
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_jumpPower;
    [SerializeField] float m_thrownPower;   // �������鑬�x
    [SerializeField] float m_gravity;
    [SerializeField] Vector3 m_rayOffset;
    [SerializeField] float m_rayLength;

    [Header("UI")]
    [SerializeField] Image m_image;

    List<GameObject> m_carryList = new List<GameObject>();  // �S����I�u�W�F�N�g�̃��X�g
    GameObject m_carryingObject;
    Rigidbody m_rigidbody;
    Animator m_animator;

    public GameObject CarryingObject { get { return m_carryingObject; } }

    Vector2 m_inputMove;
    bool m_isLanding;
    bool m_isThrowing;     // �������Ă����Ԃ��ǂ���
    bool m_isGravity;      // �d�͂̉e�����󂯂邩

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

        // �S���ł�����̂�����Η���
        if(m_carryingObject)
        {
            m_carryingObject.GetComponent<CarryObject>().OnDrop();
            m_carryingObject = null;
        }

        // UI���\��
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
            // �S����I�u�W�F�N�g�̒��ōł��������߂����̂��w�肷��
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

                    // UI���\��
                    m_image.gameObject.SetActive(false);
                }
            }

            // �S������
            m_carryingObject.GetComponent<CarryObject>().OnCarry(gameObject);

            // �A�j���[�V�����ݒ�
            m_animator.SetBool("Carry", true);

            return;
        }

        if(m_carryingObject)
        {
            // �~�낷����
            m_carryingObject.GetComponent<CarryObject>().OnDrop();
            m_carryingObject = null;

            // �A�j���[�V�����ݒ�
            m_animator.SetBool("Carry", false);

            return;
        }
    }

    // �O����Ăяo���֐�
    public void OnThrown(float thrownPower = -1)
    {
        // �ړ��ʂ����Z�b�g
        m_rigidbody.velocity = Vector3.zero;

        // �������鏈��
        m_isLanding = false;
        m_isThrowing = true;

        // �����œ����鋭�����n���ꂽ�炻����g��
        // �c�ɂ�⋭���Ƃ΂�
        float power = thrownPower == -1 ? m_thrownPower : thrownPower;
        m_rigidbody.AddForce(transform.forward * power + transform.up * power * 1.25f, ForceMode.Impulse);
    }

    bool IsGround()
    {
        // MEMO BoxRay���΂����ق����悭�Ȃ���
        // �T�{�̃��C���΂��Đڒn����
        for(int x = -1; x <= 1; x += 2)
        {
            for(int z = -1; z <= 1; z += 2)
            {
                Vector3 pos = transform.rotation * new Vector3(m_rayOffset.x * x, m_rayOffset.y, m_rayOffset.z * z) + transform.position;
                Ray ray = new Ray(pos, Vector3.down);

                Debug.DrawRay(pos, Vector3.down * m_rayLength);

                foreach (RaycastHit hit in Physics.RaycastAll(ray, m_rayLength))
                {
                    // �����̃R���C�_�[�Ȃ�X�L�b�v
                    if (hit.collider.gameObject == gameObject) continue;

                    // Trigger�p�̃R���C�_�[�Ȃ�X�L�b�v
                    if (hit.collider.isTrigger) continue;

                    // �A�j���[�V�����ݒ�
                    m_animator.SetBool("Landing", true);

                    return true;
                }
            }
        }

        // �A�j���[�V�����ݒ�
        m_animator.SetBool("Landing", false);

        return false;
    }

    void SetLocalGravity()
    {
        // ���n�������͂��Ȃ��ꍇ�́A��߂̏d�͂�����
        m_rigidbody.AddForce(new Vector3(0, -m_gravity, 0), ForceMode.Force);
    }

    private void FixedUpdate()
    {
        // �ڒn����
        m_isLanding = IsGround();

        // ������ꂽ�璅�n����܂ňړ��o���Ȃ��Ȃ�
        if(m_isThrowing && m_isLanding)
        {
            m_isThrowing = false;
        }

        // �ړ�
        // �������Ă���Ԃ͓����Ȃ�
        if(m_canMove && !m_isThrowing)
        {
            // �J�����̊p�x�ɍ��킹�Ĉړ�
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 velocity = cameraForward * m_inputMove.y * m_moveSpeed + Camera.main.transform.right * m_inputMove.x * m_moveSpeed;

            // �������Ɛi�s����������
            if(m_inputMove != Vector2.zero)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation, Quaternion.LookRotation(velocity), 0.3f);
            }

            // �d�͂𔽉f
            float gravity = m_rigidbody.velocity.y;

            // �ړ��ʂ𔽉f
            m_rigidbody.velocity = velocity + new Vector3(0, gravity, 0);

            // �A�j���[�V�����ݒ�
            bool moveFlag = velocity != Vector3.zero ? true : false;
            m_animator.SetBool("Move", moveFlag);
        }

        // �d�͂�K�p
        if(m_isGravity) SetLocalGravity();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����Ȃ�X���[
        // ���ɓ����Ă���Ȃ�X���[
        // Trigger�Ȃ�X�L�b�v
        // �S����I�u�W�F�N�g�łȂ���΃X�L�b�v
        // �������S����Ă�����X�L�b�v
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

        // ���X�g����ŁA�����S���ł��Ȃ����UI��\��
        if(m_carryList.Count == 0 && m_carryingObject == null)
        {
            m_image.gameObject.SetActive(true);
        }

        // �߂��ɂ���S����I�u�W�F�N�g�����X�g�ɒǉ�����
        m_carryList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // �����Ă��Ȃ��Ȃ�X�L�b�v
        if (m_carryList.Contains(other.gameObject) == false) return;

        // ���������ꂽ�I�u�W�F�N�g�����X�g���珜�O����
        m_carryList.Remove(other.gameObject);

        // ���X�g����ɂȂ�����UI���\���ɂ���
        if (m_carryList.Count == 0)
        {
            m_image.gameObject.SetActive(false);
        }
    }
}
