using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �g�p�����A�C�e���ƃX�e�[�W�̐i�s�x���Ǘ�����

public static class StageCounter
{
    static int m_stage = new int();     // ��������X�e�[�W���r�b�g�ŕێ�
    static int m_usedItemNum;  // �g�p�����A�C�e���̔ԍ���ێ�

    public enum Item
    {
        Nothing = -1,   // �����g�p���Ă��Ȃ�

        Cubes,
        Propeller,
        Bomb,
        NearSwitch,
        FarSwitch,
        Wall,

        Length,
    }

    public static void Initialize()
    {
        // ������
        m_stage = 0;
        m_usedItemNum = (int)Item.Nothing;
    }

    public static bool CheckClear(int stageNum)
    {
        // �ԍ����w�肵�ăX�e�[�W���N���A���Ă��邩�m�F
        return (m_stage & (1 << stageNum)) != 0 ? true : false;
    }

    public static bool CheckClearBelow(int stageNum)
    {
        // �w��̔ԍ��ȉ��̃t���O�������Ă��邩�m�F
        // �S�������Ă����True
        for(int i = stageNum - 1; i > 0; i--)
        {
            if ((m_stage & (1 << i)) == 0) return false;
        }

        return true;
    }

    public static void OnClear()
    {
        // ��ԍŏ��Ɏg�p�����A�C�e�����X�e�[�W����폜����
        // �����g�p����Ă��Ȃ��ꍇ�́ABoxes���폜����
        int clearStage = m_usedItemNum == (int)Item.Nothing ? (int)Item.Cubes : m_usedItemNum;

        m_stage |= 1 << clearStage;

        // �g�p�����A�C�e���̋L�^�����Z�b�g
        m_usedItemNum = (int)Item.Nothing;
    }

    public static void OnUseItem(int useItem)
    {
        // �g�p�����A�C�e�����ꎞ�I�ɋL�^
        // �ŏ��Ɏg�p�����A�C�e���̂݋L�^
        m_usedItemNum = m_usedItemNum == (int)Item.Nothing ? useItem : m_usedItemNum;
    }
}
