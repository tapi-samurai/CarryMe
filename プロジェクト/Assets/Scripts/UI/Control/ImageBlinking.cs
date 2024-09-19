using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// リストの画像を一定間隔で入れ替える

public class ImageBlinking : MonoBehaviour
{
    [SerializeField] List<Sprite> m_spriteList; // 画像のリスト
    [SerializeField] float m_blinkingInterval;  // 点滅の速度

    Image m_image;
    int m_currentSpriteNum;
    float m_elapsedTime;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        // 一番最初の画像を表示
        int currentSpriteNum = 0;
        m_image.sprite = m_spriteList[currentSpriteNum];
        m_currentSpriteNum = currentSpriteNum;
        m_elapsedTime = 0;
    }

    private void FixedUpdate()
    {
        m_elapsedTime += Time.deltaTime;

        if (m_elapsedTime < m_blinkingInterval) return;

        // 画像リストの番号を進める
        m_currentSpriteNum = m_currentSpriteNum >= m_spriteList.Count - 1 ? 0 : m_currentSpriteNum + 1;

        // 画像の切り替え
        m_image.sprite = m_spriteList[m_currentSpriteNum];

        // 経過時間をリセット
        m_elapsedTime = 0;
    }
}
