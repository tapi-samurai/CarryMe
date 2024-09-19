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
        // �^�C�g����\��
        m_titleCarry.gameObject.SetActive(true);
        m_titleMe.gameObject.SetActive(true);

        foreach(Image image in m_imageList)
        {
            image.gameObject.SetActive(true);
        }

        // �G���f�B���O�̃^�C�g����\��
        m_endCarry.gameObject.SetActive(false);
        m_endMe.gameObject.SetActive(false);

        // Canvas�̈ʒu�����ɐݒ�
        GetComponent<Canvas>().sortingOrder = -1;
    }

    // �^�C�g���̃A�j���[�V����
    public async UniTask HiddenTitle()
    {
        // �A�C�R���͑����ɍ폜
        foreach(Image image in m_imageList)
        {
            image.gameObject.SetActive(false);
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        // �V�[�N�G���X���쐬�E�ҏW
        var sequence = DOTween.Sequence();
        _ = sequence.Append(m_titleCarry.DOFade(0f, 2f).SetEase(Ease.InQuart));
        _ = sequence.Join(m_titleMe.DOFade(0f, 2f).SetEase(Ease.InQuart));

        // �V�[�N�G���X�̊�����҂�
        await sequence;

        // �^�C�g�����\���ɂ���
        m_titleCarry.gameObject.SetActive(false);
        m_titleMe.gameObject.SetActive(false);
    }

    // �^�C�g����\��
    public void DispTitle(bool main)
    {
        // Canvas�̈ʒu����O�ɐݒ�
        GetComponent<Canvas>().sortingOrder = 1;

        // ME�̕������c�����L�����N�^�[�̐F�ɂ���
        m_endMe.color = main ? m_meSubColor : m_meMainColor;

        m_endCarry.gameObject.SetActive(true);
        m_endMe.gameObject.SetActive(true);
    }
}
