using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���X�g�̉摜�����Ԋu�œ���ւ���

public class ImageBlinking : MonoBehaviour
{
    [SerializeField] List<Sprite> m_spriteList; // �摜�̃��X�g
    [SerializeField] float m_blinkingInterval;  // �_�ł̑��x

    Image m_image;
    int m_currentSpriteNum;
    float m_elapsedTime;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        // ��ԍŏ��̉摜��\��
        int currentSpriteNum = 0;
        m_image.sprite = m_spriteList[currentSpriteNum];
        m_currentSpriteNum = currentSpriteNum;
        m_elapsedTime = 0;
    }

    private void FixedUpdate()
    {
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime < m_blinkingInterval) return;

        // �摜���X�g�̔ԍ���i�߂�
        m_currentSpriteNum = m_currentSpriteNum >= m_spriteList.Count - 1 ? 0 : m_currentSpriteNum + 1;

        // �摜�̐؂�ւ�
        m_image.sprite = m_spriteList[m_currentSpriteNum];

        // �o�ߎ��Ԃ����Z�b�g
        m_elapsedTime = 0;
    }
}
