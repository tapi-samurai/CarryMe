using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

// �R�����g�̐������x�̊Ǘ��Ɩ���

public class CommentControl : SingletonMonoBehaviour<CommentControl>
{
    [Header("Text")]
    [SerializeField] CommentData m_commentData;

    [Header("Value")]
    [SerializeField] float m_minInterval;               // �R�����g�̍ŒZ�����Ԋu
    [SerializeField] float m_maxInterval;               // �R�����g�̍Œ������Ԋu
    [SerializeField] float m_minCarryCommentInterval;   // �v���C���[��S���R�����g�̍ŒZ�����Ԋu
    [SerializeField] float m_maxCarryCommentInterval;   // �v���C���[��S���R�����g�̍Œ������Ԋu
    [SerializeField] int m_carryCommentHeight;          // �v���C���[��S���R�����g��}������i��
    [SerializeField] int m_maxComplimentComment;        // �v���C���[���ق߂�R�����g�̍ő�������
    [SerializeField] int m_minComplimentComment;        // �v���C���[���ق߂�R�����g�̍ŏ�������

    CommentManager m_commentManager;
    List<CommentDataEntity> m_nowCommentList = new List<CommentDataEntity>();
    List<string> m_nextCommentList = new List<string>();

    float m_elapsedTime;
    float m_nextCommentInterval;
    float m_carryCommentElapsedTime;
    float m_nextCarryCommentInterval;

    private void Awake()
    {
        // �����R�����g�̃��X�g��������
        m_nowCommentList = m_commentData.normal;

        m_commentManager = GetComponent<CommentManager>();

        m_elapsedTime = 0;
        m_carryCommentElapsedTime = 0;
        m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);
        m_nextCarryCommentInterval = m_maxCarryCommentInterval;

        // �d���Ȃ邩��ŏ��Ɉ�x�R�����g�𐶐�
        m_commentManager.AddNextCommentList(m_commentManager.InstantiateComment("�����ł�", false));
    }

    private void FixedUpdate()
    {
        m_elapsedTime += Time.deltaTime;
        if (StageCounter.CheckClearBelow((int)StageCounter.Item.Wall + 1))
        {
            m_carryCommentElapsedTime += Time.deltaTime;
        }

        if (m_elapsedTime >= m_nextCommentInterval)
        {
            // �v���C���[��S���R�����g�̐����͈ȉ��̏����𖞂������ꍇ�̂�
            // �E�X�e�[�W��̃I�u�W�F�N�g�����ׂĂȂ��Ȃ��Ă���
            // �E�v���C���[��S���R�����g�p�̕K�v���Ԃ��o�߂��Ă���
            // �E�R�����g�̐ݒ�̒i���ɑ}���ł���
            if (
                StageCounter.CheckClear((int)StageCounter.Item.Wall) == true &&
                m_carryCommentElapsedTime >= m_nextCarryCommentInterval
                )
            {
                // �S���R�����g�𐶐�
                string carryCommentText = m_commentData.carry[Random.Range(0, m_commentData.carry.Count)].comment;
                GameObject carryComment = m_commentManager.InstantiateComment(carryCommentText, true);
                TextMeshProUGUI textMeshProUGUI = carryComment.GetComponent<TextMeshProUGUI>();

                // �R�����g��}���ł���i�����ݒ�ʂ�ł����
                if(m_commentManager.CheckCommentArray(textMeshProUGUI).x == m_carryCommentHeight - 1)
                {
                    // �R�����g��}��
                    m_commentManager.AddNextCommentList(carryComment);

                    // �o�ߎ��Ԃ����Z�b�g
                    m_elapsedTime = 0;
                    m_carryCommentElapsedTime = 0;

                    // �C���^�[�o�����Đݒ�
                    m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);
                    m_nextCarryCommentInterval = Random.Range(m_minCarryCommentInterval, m_maxCarryCommentInterval);

                    return;
                }

                // �S���R�����g���}���ł��Ȃ���΍폜
                Destroy(carryComment);
            }

            // �ʏ�R�����g�𐶐����đ}��
            // �R�����g���X�g�ɗ\�񂪂���΂���𗬂�
            // �Ȃ���Ί��S�����_��
            GameObject comment;
            if(m_nextCommentList.Count == 0)
            {
                comment = m_commentManager.InstantiateComment(
                    m_nowCommentList[Random.Range(0, m_nowCommentList.Count)].comment, false);
            }
            else
            {
                comment = m_commentManager.InstantiateComment(
                    m_nextCommentList[0], false);

                // ���X�g����폜
                m_nextCommentList.Remove(m_nextCommentList[0]);
            }

            m_commentManager.AddNextCommentList(comment);

            // ���̃R�����g�𐶐�����܂ł̎��Ԃ��Đݒ�
            m_nextCommentInterval = Random.Range(m_minInterval, m_maxInterval);

            // �o�ߎ��Ԃ����Z�b�g
            m_elapsedTime = 0;
        }
    }

    void AddNextCommentList(string nextComment)
    {
        m_nextCommentList.Add(nextComment);
    }

    public async void ComplimentComment()
    {
        // �v���C���[���ق߂�R�����g���ʂɗ���
        int i = Random.Range(m_minComplimentComment, m_maxComplimentComment);
        while(i > 0)
        {
            // �R�����g�𐶐�
            GameObject comment = CommentManager.instance.InstantiateComment(
                m_commentData.homeru[Random.Range(0, m_commentData.homeru.Count)].comment, false);

            // �}��
            CommentManager.instance.AddNextCommentList(comment);

            // �J�E���g�����炷
            i--;

            // �������f�B���C��������
            await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0.0f, 0.1f)));
        }
    }

    public void GoalHintComment()
    {
        AddNextCommentList(m_commentData.hint[Random.Range(0, m_commentData.hint.Count)].comment);
    }

    public void End()
    {
        // �G���f�B���O���̃R�����g�ɍ����ւ���
        m_nowCommentList = m_commentData.end;

        // �R�����g�̕p�x��������
        m_minInterval = 0;
        m_maxInterval = 0.2f;
    }
}
