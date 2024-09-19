using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

// �X�e�[�W�V�[���̐i�s���Ǘ�����

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

    List<GameObject> m_currentStageList = new List<GameObject>();   // �������ꂽ�X�e�[�W�̃��X�g

    bool m_isClick;

    [System.Serializable]
    class StageData
    {
        public GameObject prefab;
        public StageCounter.Item itemType;
    }

    private async void Awake()
    {
        // PlayerInput����̓��͂��󂯎��悤�ɂ���
        m_isClick = false;
        m_playerInput.actions["Carry1"].started += OnClick;

        // �X�e�[�W�Ǘ��̏�����
        StageCounter.Initialize();

        // �v���C���[�̑���֘A�̏���������
        m_playerPresenter.Initialize();
        SetPlayerAndWarp(false);

        // �}�b�v�̐���
        InstantiateStage();

        // �N���b�N���ꂽ��N���b�N���󂯕t���Ȃ��悤�ɂ���
        await UniTask.WaitUntil(() => m_isClick);
        m_playerInput.actions["Carry1"].started -= OnClick;
        
        // �^�C�g�����\���ɂ���
        await TitleViewer.instance.HiddenTitle();

        // �J�������t�F�[�h�C��
        await CameraTransition.instance.CameraTransitionFadeIn();

        // �v���C���[��\��
        SetPlayerAndWarp(true);

        // �f�B���C
        await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));

        // BGM���Đ�
        SoundEffect.Play2D(m_bgm, 0.05f, 1, true);
    }

    public void OnClick(InputAction.CallbackContext callback)
    {
        m_isClick = true;
    }

    public async void SetNextStage()
    {
        // �v���C���[���\���ɂ���
        SetPlayerAndWarp(false);

        // �g�p�����A�C�e�����L�^����
        StageCounter.OnClear();

        // �J�������t�F�[�h�A�E�g
        await CameraTransition.instance.CameraTransitionFadeOut();

        // ���݂̃X�e�[�W�����ׂč폜����
        while (m_currentStageList.Count != 0)
        {
            GameObject currentStage = m_currentStageList[0];
            m_currentStageList.Remove(currentStage);
            Destroy(currentStage);
        }

        // �X�e�[�W�𐶐�
        InstantiateStage();

        // �J�������t�F�[�h�C��
        await CameraTransition.instance.CameraTransitionFadeIn();

        // �v���C���[��\�����Ĉړ�������
        SetPlayerAndWarp(true);
    }

    void SetPlayerAndWarp(bool active)
    {
        // �v���C���[�̕\���ƈʒu��ݒ�
        for (int i = 0; i < m_playerList.Count; i++)
        {
            m_playerList[i].gameObject.SetActive(active);

            if(active)
            {
                m_playerList[i].gameObject.transform.position = m_spawnPoint[i].transform.position;

                // �v���C���[�𒅒n����܂ōs���s�\�ɂ���
                m_playerList[i].OnThrown(0);
            }
        }
    }

    void InstantiateStage()
    {
        // �X�e�[�W�𐶐�����
        foreach(StageData stageData in m_stageDataList)
        {
            // �܂��g�p���Ă��Ȃ��A�C�e���̂ݐ�������
            if(StageCounter.CheckClear((int)stageData.itemType) == false)
            {
                GameObject stage = Instantiate(stageData.prefab, transform);
                m_currentStageList.Add(stage);

                // �ǂ̐������̏���
                if(stageData.itemType == StageCounter.Item.Wall)
                {
                    // �ǈȊO�̃A�C�e�������������Ȃ�A�����グ���Ȃ��悤�ɂ���
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
                // �g�p�ς݃A�C�e���̗�O����
                switch((int)stageData.itemType)
                {
                    case (int)StageCounter.Item.Cubes:
                        // �ǂ݂̂̃X�e�[�W�����N���A�̏ꍇ�́A�����グ���Ȃ����Ĕz�u
                        // �N���A���Ă���ΐ������Ȃ�
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
        // �Q�[���I�����̏���

        // �R�����g���G���f�B���O�p�ɕω�������
        CommentControl.instance.End();

        // �f�B���C
        await UniTask.Delay(System.TimeSpan.FromSeconds(3.0f));

        // �v���C���[�̑�����~
        foreach (Player player in m_playerList)
        {
            player.enabled = false;
            player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        // �J�����𓮂��Ȃ��悤�ɂ���
        CameraTransition.instance.End();

        // �f�B���C
        await UniTask.Delay(System.TimeSpan.FromSeconds(3.0f));

        // �^�C�g����\��
        TitleViewer.instance.DispTitle(main);
    }
}
