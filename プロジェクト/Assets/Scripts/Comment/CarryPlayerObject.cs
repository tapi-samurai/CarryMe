using UnityEngine;

public class CarryPlayerObject : MonoBehaviour
{
    static readonly Vector3 PlayerPos = new Vector3(100,100,100);

    bool m_isCarry;

    private void OnDisable()
    {
        // �R�����g�폜���Ƀv���C���[���q�Ɏ����Ă����牓���Ƀ��[�v���Đ؂藣��
        if (transform.childCount == 0) return;
        transform.GetChild(0).position = PlayerPos;
        transform.GetChild(0).parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �E�R���C�_�[��IsTrigger
        // �E�v���C���[�ȊO
        // �Ȃ�X���[
        if (
            other.isTrigger == true ||
            other.TryGetComponent<Player>(out Player player) == false
            )
        {
            return;
        }

        // �͈͓��Ƀv���C���[�������Ă�����S��
        other.GetComponent<CarryObject>().OnCarry(gameObject);

        // �G���f�B���O���o
        bool main = other.GetComponent<Player>().MainCharactor;
        StageSceneManager.instance.End(main);
    }
}
