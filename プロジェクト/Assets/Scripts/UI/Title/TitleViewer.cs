using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class TitleViewer : SingletonMonoBehaviour<TitleViewer>
{
    [SerializeField] TextMeshProUGUI m_titleCarry;
    [SerializeField] TextMeshProUGUI m_titleMe;
    [SerializeField] TextMeshProUGUI m_endCarry;
    [SerializeField] TextMeshProUGUI m_endMe;
    [SerializeField] List<Image> m_imageList;
    [SerializeField] Color m_meMainColor;
    [SerializeField] Color m_meSubColor;

    private void Awake()
    {
        // タイトルを表示
        m_titleCarry.gameObject.SetActive(true);
        m_titleMe.gameObject.SetActive(true);

        foreach(Image image in m_imageList)
        {
            image.gameObject.SetActive(true);
        }

        // エンディングのタイトルを表示
        m_endCarry.gameObject.SetActive(false);
        m_endMe.gameObject.SetActive(false);

        // Canvasの位置を奥に設定
        GetComponent<Canvas>().sortingOrder = -1;
    }

    // タイトルのアニメーション
    public async UniTask HiddenTitle()
    {
        // アイコンは即座に削除
        foreach(Image image in m_imageList)
        {
            image.gameObject.SetActive(false);
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        // シークエンスを作成・編集
        var sequence = DOTween.Sequence();
        _ = sequence.Append(m_titleCarry.DOFade(0f, 2f).SetEase(Ease.InQuart));
        _ = sequence.Join(m_titleMe.DOFade(0f, 2f).SetEase(Ease.InQuart));

        // シークエンスの完了を待つ
        await sequence;

        // タイトルを非表示にする
        m_titleCarry.gameObject.SetActive(false);
        m_titleMe.gameObject.SetActive(false);
    }

    // タイトルを表示
    public void DispTitle(bool main)
    {
        // Canvasの位置を手前に設定
        GetComponent<Canvas>().sortingOrder = 1;

        // MEの文字を残ったキャラクターの色にする
        m_endMe.color = main ? m_meSubColor : m_meMainColor;

        m_endCarry.gameObject.SetActive(true);
        m_endMe.gameObject.SetActive(true);
    }
}
