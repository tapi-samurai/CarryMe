using UnityEngine;

public class CarryPlayerObject : MonoBehaviour
{
    static readonly Vector3 PlayerPos = new Vector3(100,100,100);

    bool m_isCarry;

    private void OnDisable()
    {
        // コメント削除時にプレイヤーを子に持っていたら遠くにワープして切り離す
        if (transform.childCount == 0) return;
        transform.GetChild(0).position = PlayerPos;
        transform.GetChild(0).parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ・コライダーがIsTrigger
        // ・プレイヤー以外
        // ならスルー
        if (
            other.isTrigger == true ||
            other.TryGetComponent<Player>(out Player player) == false
            )
        {
            return;
        }

        // 範囲内にプレイヤーが入ってきたら担ぐ
        other.GetComponent<CarryObject>().OnCarry(gameObject);

        // エンディング演出
        bool main = other.GetComponent<Player>().MainCharactor;
        StageSceneManager.instance.End(main);
    }
}
