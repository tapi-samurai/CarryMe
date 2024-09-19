using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

// �R�����g�̐����E�ړ��E�폜

public class CommentManager : SingletonMonoBehaviour<CommentManager>
{
    const float CommentHeight = 90.0f;      // �R�����g�̍���   
    const int CommentLineHeight = 12;       // �R�����g�̏c�̍s��
    const int MaxCommentDraw = 100;           // ��s�ɕ\���ł���R�����g�̐�

    [SerializeField] GameObject m_commentPrefab;
    [SerializeField] GameObject m_carryCommentPrefab;
    [SerializeField] float m_commentDrawTime = 4.0f;     // �R�����g�̕\������

    GameObject[][] m_commentArray = new GameObject[CommentLineHeight][];    // �\�����Ă���R�����g�̔z��
    List<GameObject> m_nextCommentList = new List<GameObject>();    // ���̃t���[���ŕ\������R�����g�̃��X�g
    List<GameObject> m_deleteCommentList = new List<GameObject>();  // ���̃t���[���ō폜����R�����g�̃��X�g
    RectTransform m_canvasRectTransform;

    private void Awake()
    {
        // �R�����g�����z��̏�����
        for(int i = 0; i < CommentLineHeight; i++)
        {
            m_commentArray[i] = Enumerable.Repeat<GameObject>(null, MaxCommentDraw).ToArray();
        }

        m_canvasRectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // �z��ɓ����Ă���R�����g���E���獶�ֈړ�������
        for(int i = 0; i < CommentLineHeight; i++)
        {
            for(int j = 0; j < MaxCommentDraw; j++)
            {
                // ��Ȃ�X�L�b�v
                if (m_commentArray[i][j] == null)
                {
                    continue;
                }

                TextMeshProUGUI textMeshProUGUI = m_commentArray[i][j].GetComponent<TextMeshProUGUI>();

                // ���ړ��������擾
                float length;
                length = (m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2) * 2;

                // �R�����g���\���͈͊O�ɂ�����폜���X�g�ɓo�^
                if (-(length / 2) > textMeshProUGUI.rectTransform.localPosition.x)
                {
                    RemoveComment(i, j);
                }

                // ���ړ������ƑO�t���[������̌o�ߎ��Ԃňړ��ʂ��o��
                float move = length * (Time.deltaTime / m_commentDrawTime);
                Vector3 pos = textMeshProUGUI.rectTransform.localPosition - new Vector3(move, 0, 0);
                textMeshProUGUI.rectTransform.localPosition = pos;
            }
        }

        // �폜����R�����g������΍폜����
        while(m_deleteCommentList.Count != 0)
        {
            // �R�����g���폜����
            GameObject deleteComment = m_deleteCommentList[0];
            m_deleteCommentList.Remove(deleteComment);
            Destroy(deleteComment);
        }

        // �z��𐮗�
        for (int i = 0; i < CommentLineHeight; i++)
        {
            for (int j = 0; j < MaxCommentDraw; j++)
            {
                if (m_commentArray[i][j] == null) continue;
                if (j - 1 < 0) continue;
                if (m_commentArray[i][j - 1] != null) continue;

                m_commentArray[i][j - 1] = m_commentArray[i][j];
                m_commentArray[i][j] = null;
            }
        }

        // �\������R�����g������ΐ������Ĕz��ɓo�^
        while(m_nextCommentList.Count != 0)
        {
            // �z��̋󂫂��`�F�b�N
            Vector2Int num = CheckCommentArray(m_nextCommentList[0].GetComponent<TextMeshProUGUI>());

            // �󂫂��Ȃ���΃X�L�b�v
            if (num.x == -1 || num.y == -1) break;

            // �R�����g��}��
            AddCommentArray(m_nextCommentList[0]);
            m_nextCommentList.Remove(m_nextCommentList[0]);
        }
    }

    // �R�����g�̐����Ə����ݒ�
    public GameObject InstantiateComment(string text, bool canCarry)
    {
        // �R�����g�̐���
        GameObject comment;
        if (canCarry == false)
        {
            comment = Instantiate(m_commentPrefab, transform);
        }
        else
        {
            comment = Instantiate(m_carryCommentPrefab, transform);
            comment.GetComponent<CarryPlayerComment>().m_canvas = GetComponent<Canvas>();
        }

        TextMeshProUGUI textMeshProUGUI = comment.GetComponent<TextMeshProUGUI>();

        // �e�L�X�g��}��
        textMeshProUGUI.text = text;

        // �����e�L�X�g�ɍ��킹��
        textMeshProUGUI.rectTransform.sizeDelta = new Vector2(textMeshProUGUI.preferredWidth, CommentHeight);

        // ��ʂ̉E�[�Ɉړ�
        Vector3 pos = Vector3.zero;
        pos.x = m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2;
        textMeshProUGUI.rectTransform.localPosition = pos;

        return comment;
    }

    // �z��ɃR�����g��}��
    void AddCommentArray(GameObject comment)
    {
        TextMeshProUGUI textMeshProUGUI = comment.GetComponent<TextMeshProUGUI>();

        // �R�����g�̔z��ԍ����擾
        Vector2Int arrayNum = CheckCommentArray(textMeshProUGUI);

        // �c�̍��W��ݒ�
        Vector3 commentPos = textMeshProUGUI.rectTransform.localPosition;
        commentPos.y = (m_canvasRectTransform.rect.height / 2 - CommentHeight / 2) - CommentHeight * arrayNum.x;
        textMeshProUGUI.rectTransform.localPosition = commentPos;

        // �z��ɑ}��
        m_commentArray[arrayNum.x][arrayNum.y] = comment;
    }

    public Vector2Int CheckCommentArray(TextMeshProUGUI textMeshProUGUI)
    {
        // �Ȃ�ׂ���̒i�ɃR�����g��}������
        // ���Ԃ炸�\���ł���i���Ȃ����-1,-1��Ԃ�
        for (int i = 0; i < CommentLineHeight; i++)
        {
            for (int j = 0; j < MaxCommentDraw; j++)
            {
                if (m_commentArray[i][j] != null) continue;

                // �O�ɃR�����g������ꍇ
                if (j > 0)
                {
                    TextMeshProUGUI frontComment = m_commentArray[i][j - 1].GetComponent<TextMeshProUGUI>();

                    float frontCommentRightPosX =
                        frontComment.rectTransform.localPosition.x + frontComment.rectTransform.rect.width / 2;

                    // �܂��R�����g�S�̂���ʂɓ����Ă��Ȃ���Ή��̒i�Ɉړ�
                    if (frontCommentRightPosX > m_canvasRectTransform.rect.width / 2)
                    {
                        j = MaxCommentDraw;
                        continue;
                    }

                    float commentLeftPosX =
                        textMeshProUGUI.rectTransform.localPosition.x - (textMeshProUGUI.rectTransform.rect.width / 2);
                    float frontCommentLength = (m_canvasRectTransform.rect.width / 2 + frontComment.rectTransform.rect.width / 2) * 2;
                    float commentLength = (m_canvasRectTransform.rect.width / 2 + textMeshProUGUI.rectTransform.rect.width / 2) * 2;
                    float frontCommentSpeed = frontCommentLength / m_commentDrawTime;
                    float commentSpeed = commentLength / m_commentDrawTime;

                    // �O�̃R�����g�̂ق����x���ꍇ
                    if (frontCommentSpeed < commentSpeed)
                    {
                        float frontCommentRestDistance = m_canvasRectTransform.rect.width / 2 + frontCommentRightPosX;
                        float restTime = frontCommentRestDistance / frontCommentSpeed;

                        // �\�����ɂ��Ԃ��Ă��܂��ꍇ�͉��̒i�Ɉړ�
                        if (frontCommentRightPosX - frontCommentSpeed * restTime > commentLeftPosX - commentSpeed * restTime)
                        {
                            j = MaxCommentDraw;
                            continue;
                        }
                    }
                }

                return new Vector2Int(i, j);
            }
        }

        // ������Ȃ���΋󂢂Ă���ꏊ���ォ��T��
        for (int i = 0; i < MaxCommentDraw; i++)
        {
            for (int j = 0; j < CommentLineHeight; j++)
            {
                // ��Ȃ�X�L�b�v
                if (m_commentArray[j][i] != null) continue;

                return new Vector2Int(j, i);
            }
        }

        // �܂������󂫂��Ȃ��ꍇ��-1,-1��Ԃ�
        return new Vector2Int(-1,-1);
    }

    // �\��R�����g�̃��X�g�ɓo�^
    public void AddNextCommentList(GameObject comment)
    {
        m_nextCommentList.Add(comment);
    }

    // �폜���X�g�ɓo�^
    void RemoveComment(int height, int j)
    {
        m_deleteCommentList.Add(m_commentArray[height][j]);
    }
}
