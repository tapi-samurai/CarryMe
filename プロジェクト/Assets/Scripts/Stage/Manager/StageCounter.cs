using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使用したアイテムとステージの進行度を管理する

public static class StageCounter
{
    static int m_stage = new int();     // 生成するステージをビットで保持
    static int m_usedItemNum;  // 使用したアイテムの番号を保持

    public enum Item
    {
        Nothing = -1,   // 何も使用していない

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
        // 初期化
        m_stage = 0;
        m_usedItemNum = (int)Item.Nothing;
    }

    public static bool CheckClear(int stageNum)
    {
        // 番号を指定してステージをクリアしているか確認
        return (m_stage & (1 << stageNum)) != 0 ? true : false;
    }

    public static bool CheckClearBelow(int stageNum)
    {
        // 指定の番号以下のフラグが立っているか確認
        // 全部立っていればTrue
        for(int i = stageNum - 1; i > 0; i--)
        {
            if ((m_stage & (1 << i)) == 0) return false;
        }

        return true;
    }

    public static void OnClear()
    {
        // 一番最初に使用したアイテムをステージから削除する
        // 何も使用されていない場合は、Boxesを削除する
        int clearStage = m_usedItemNum == (int)Item.Nothing ? (int)Item.Cubes : m_usedItemNum;

        m_stage |= 1 << clearStage;

        // 使用したアイテムの記録をリセット
        m_usedItemNum = (int)Item.Nothing;
    }

    public static void OnUseItem(int useItem)
    {
        // 使用したアイテムを一時的に記録
        // 最初に使用したアイテムのみ記録
        m_usedItemNum = m_usedItemNum == (int)Item.Nothing ? useItem : m_usedItemNum;
    }
}
