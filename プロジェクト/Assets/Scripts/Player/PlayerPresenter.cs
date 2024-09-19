using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

// modelの値を監視して、複数のプレイヤーに伝達する

public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] Player[] m_playerList; // view
    PlayerModel m_playerModel;              // model

    // 初期化
    public void Initialize()
    {
        // PlayerInputから入力された値を持つクラスを生成
        m_playerModel = new PlayerModel();
        m_playerModel.SetPlayerInput(GetComponent<PlayerInput>());

        // プレイヤーとModelを紐づけ
        Bind();

        // PlayerInputからの入力をModelが受け取るようにする
        m_playerModel.OnEntry();
    }

    private void OnDisable()
    {
        m_playerModel.OnDelete();
    }

    // modelの値の変更を監視して、複数のプレイヤーに通知
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
