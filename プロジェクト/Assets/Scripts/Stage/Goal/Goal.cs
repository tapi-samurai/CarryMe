using UnityEngine;

public class Goal : MonoBehaviour
{
    const float StandardPosY = -2.1f;
    const float FullSinkPosY = -2.45f;
    const float FullBounceUpPosY = 2.4f;
    const float BounceUpTime = 0.125f;
    const float TransitionPlayerPosY = 21.0f;

    bool m_isClear;
    bool m_hintFlag;

    [SerializeField] Player m_playerParent;

    [SerializeField] float m_sinkTime;     // ���ރX�s�[�h
    [SerializeField] bool m_isCollision;
    [SerializeField] bool m_isTrigger;
    [SerializeField] AudioClip m_se;

    private void Awake()
    {
        m_isClear = false;
        m_hintFlag = true;
    }

    private void FixedUpdate()
    {
        // �R���W�������Ă���
        // �g���K�[�ɂ��������Ă���
        // ���A�{�^�������ݐ؂�
        // ���A�v���C���[��S���ł���
        // ���N���A

        if (m_isClear == false)
        {
            if (m_isCollision == true && m_isTrigger == true)
            {
                // �v���C���[������Ă���ꍇ
                // �������Ă���
                if (transform.position.y > FullSinkPosY)
                {
                    Vector3 pos = transform.position;
                    pos.y += (FullSinkPosY - StandardPosY) * (Time.deltaTime / m_sinkTime);
                    pos.y = FullSinkPosY > pos.y ? FullSinkPosY : pos.y;
                    transform.position = pos;
                }
            }
            else
            {
                // ����Ă��Ȃ��ꍇ
                // �オ���Ă���
                if (transform.position.y < StandardPosY)
                {
                    Vector3 pos = transform.position;
                    pos.y += (StandardPosY - FullSinkPosY) * (Time.deltaTime / m_sinkTime);
                    pos.y = StandardPosY < pos.y ? StandardPosY : pos.y;
                    transform.position = pos;

                    m_hintFlag = true;
                }
            }

            // �v���C���[������Ă��Ȃ��Ȃ�X�L�b�v
            // ���ݐ؂��Ă��Ȃ���΃X�L�b�v
            if (m_isCollision == false || m_isTrigger == false ||
                transform.position.y != FullSinkPosY)
            {
                return;
            }

            // �v���C���[��S���ł��邩�ǂ���
            if (
                m_playerParent.CarryingObject != null &&
                m_playerParent.TryGetComponent<Player>(out Player child) == true
                )
            {
                m_isClear = true;

                // �v���C���[�𓮂��Ȃ�����
                m_playerParent.enabled = false;
            }
            else if(m_hintFlag)
            {
                // �v���C���[��S���ł��Ȃ���΃q���g�̃R�����g�𗬂�
                CommentControl.instance.GoalHintComment();
                m_hintFlag = false;
            }
        }
        else
        {
            // �N���A��̉��o
            // �v���C���[���͂ˏグ�ăX�e�[�W��J��

            // ����ƃv���C���[���͂ˏグ��
            float bounceUpMoveY = (FullBounceUpPosY - FullSinkPosY) * (Time.deltaTime / BounceUpTime);
            if (transform.position.y < FullBounceUpPosY)
            {
                Vector3 pos = transform.position;
                pos.y += bounceUpMoveY;
                if (FullBounceUpPosY < pos.y)
                {
                    pos.y = FullBounceUpPosY;

                    // SE
                    SoundEffect.Play25D(m_se, null, 0.5f, 1, false);    
                }
                transform.position = pos;
            }

            m_playerParent.gameObject.transform.position += new Vector3(0, bounceUpMoveY, 0);

            // �v���C���[���オ��؂�����X�e�[�W�J��
            if(m_playerParent.gameObject.transform.position.y > TransitionPlayerPosY)
            {
                m_playerParent.enabled = true;
                StageSceneManager.instance.SetNextStage();

                this.enabled = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (m_isCollision) return;

        // �v���C���[���������Ă��邩����
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            m_isCollision = true;
            m_playerParent = m_playerParent == player ? m_playerParent : player;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // �v���C���[���������Ă��邩����
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            m_isCollision = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_isTrigger || other.isTrigger) return;

        // �v���C���[���������Ă��邩����
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            m_isTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        // �v���C���[���������Ă��邩����
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            m_isTrigger = false;
        }
    }
}
