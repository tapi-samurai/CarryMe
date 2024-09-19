using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

// ステージシーンの進行を管理する

public class StageSceneManager : SingletonMonoBehaviour<StageSceneManager>
{
    [Header("[Lists]")]
    [SerializeField] List<StageData> m_stageDataList;
    [SerializeField] List<Player> m_playerList;
    [SerializeField] List<GameObject> m_spawnPoint;

    [Header("[Player]")]
    [SerializeField] PlayerPresenter m_playerPresenter;
    [SerializeField] PlayerInput m_playerInput;

    [Header("[BGM]")]
    [SerializeField] AudioClip m_bgm;

    List<GameObject> m_currentStageList = new List<GameObject>();   // 生成されたステージのリスト

    bool m_isClick;

    [System.Serializable]
    class StageData
    {
        public GameObject prefab;
        public StageCounter.Item itemType;
    }

    private async void Awake()
    {
        // PlayerInputからの入力を受け取るようにする
        m_isClick = false;
        m_playerInput.actions["Carry1"].started += OnClick;

        // ステージ管理の初期化
        StageCounter.Initialize();

        // プレイヤーの操作関連の初期化処理
        m_playerPresenter.Initialize();
        SetPlayerAndWarp(false);

        // マップの生成
        InstantiateStage();

        // クリックされたらクリックを受け付けないようにする
        await UniTask.WaitUntil(() => m_isClick);
        m_playerInput.actions["Carry1"].started -= OnClick;
        
        // タイトルを非表示にする
        await TitleViewer.instance.HiddenTitle();

        // カメラをフェードイン
        await CameraTransition.instance.CameraTransitionFadeIn();

        // プレイヤーを表示
        SetPlayerAndWarp(true);

        // ディレイ
        await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));

        // BGMを再生
        SoundEffect.Play2D(m_bgm, 0.05f, 1, true);
    }

    public void OnClick(InputAction.CallbackContext callback)
    {
        m_isClick = true;
    }

    public async void SetNextStage()
    {
        // プレイヤーを非表示にする
        SetPlayerAndWarp(false);

        // 使用したアイテムを記録する
        StageCounter.OnClear();

        // カメラをフェードアウト
        await CameraTransition.instance.CameraTransitionFadeOut();

        // 現在のステージをすべて削除する
        while (m_currentStageList.Count != 0)
        {
            GameObject currentStage = m_currentStageList[0];
            m_currentStageList.Remove(currentStage);
            Destroy(currentStage);
        }

        // ステージを生成
        InstantiateStage();

        // カメラをフェードイン
        await CameraTransition.instance.CameraTransitionFadeIn();

        // プレイヤーを表示して移動させる
        SetPlayerAndWarp(true);
    }

    void SetPlayerAndWarp(bool active)
    {
        // プレイヤーの表示と位置を設定
        for (int i = 0; i < m_playerList.Count; i++)
        {
            m_playerList[i].gameObject.SetActive(active);

            if(active)
            {
                m_playerList[i].gameObject.transform.position = m_spawnPoint[i].transform.position;

                // プレイヤーを着地するまで行動不能にする
                m_playerList[i].OnThrown(0);
            }
        }
    }

    void InstantiateStage()
    {
        // ステージを生成する
        foreach(StageData stageData in m_stageDataList)
        {
            // まだ使用していないアイテムのみ生成する
            if(StageCounter.CheckClear((int)stageData.itemType) == false)
            {
                GameObject stage = Instantiate(stageData.prefab, transform);
                m_currentStageList.Add(stage);

                // 壁の生成時の処理
                if(stageData.itemType == StageCounter.Item.Wall)
                {
                    // 壁以外のアイテムが生成されるなら、持ち上げられないようにする
                    if(
                        StageCounter.CheckClearBelow((int)StageCounter.Item.Wall) == false
                        )
                    {
                        Destroy(stage.GetComponent<CarryObject>());
                    }
                }
            }
            else
            {
                // 使用済みアイテムの例外処理
                switch((int)stageData.itemType)
                {
                    case (int)StageCounter.Item.Cubes:
                        // 壁のみのステージが未クリアの場合は、持ち上げられなくして配置
                        // クリアしていれば生成しない
                        if(StageCounter.CheckClear((int)StageCounter.Item.Wall) == false)
                        {
                            GameObject stage = Instantiate(stageData.prefab, transform);
                            stage.GetComponent<Cubes>().OnInvalidCarry();
                            m_currentStageList.Add(stage);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void AddStage(GameObject stage)
    {
        m_currentStageList.Add(stage);
    }

    public async void End(bool main)
    {
        // ゲーム終了時の処理

        // コメントをエンディング用に変化させる
        CommentControl.instance.End();

        // ディレイ
        await UniTask.Delay(System.TimeSpan.FromSeconds(3.0f));

        // プレイヤーの操作を停止
        foreach (Player player in m_playerList)
        {
            player.enabled = false;
            player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        // カメラを動かないようにする
        CameraTransition.instance.End();

        // ディレイ
        await UniTask.Delay(System.TimeSpan.FromSeconds(3.0f));

        // タイトルを表示
        TitleViewer.instance.DispTitle(main);
    }
}
