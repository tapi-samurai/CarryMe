using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

// model�̒l���Ď����āA�����̃v���C���[�ɓ`�B����

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] Player[] m_playerList; // view
    PlayerModel m_playerModel;              // model

    // ������
    public void Initialize()
    {
        // PlayerInput������͂��ꂽ�l�����N���X�𐶐�
        m_playerModel = new PlayerModel();
        m_playerModel.SetPlayerInput(GetComponent<PlayerInput>());

        // �v���C���[��Model��R�Â�
        Bind();

        // PlayerInput����̓��͂�Model���󂯎��悤�ɂ���
        m_playerModel.OnEntry();
    }

    private void OnDisable()
    {
        m_playerModel.OnDelete();
    }

    // model�̒l�̕ύX���Ď����āA�����̃v���C���[�ɒʒm
    void Bind()
    {
        foreach(Player player in m_playerList)
        {
            m_playerModel.InputMove
                .Subscribe(player.OnMove)
                .AddTo(player.gameObject);

            m_playerModel.InputJump
                .Subscribe(_ => player.OnJump())
                .AddTo(player.gameObject);

            if(player.MainCharactor)
            {
                m_playerModel.InputCarry1
                    .Subscribe(_ => player.OnCarryAndDrop())
                    .AddTo(player.gameObject);
            }
            else
            {
                m_playerModel.InputCarry2
                    .Subscribe(_ => player.OnCarryAndDrop())
                    .AddTo(player.gameObject);
            }
        }
    }
}
