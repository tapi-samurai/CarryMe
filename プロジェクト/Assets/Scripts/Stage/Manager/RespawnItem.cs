using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���W�����ȉ��ɂȂ����烊�X�|�[������

public class RespawnItem : MonoBehaviour
{
    static readonly float RespawnPosY = 12.0f;
    static readonly float TriggerPosY = -30.0f;

    Vector3 m_spawnPos;

    private void OnEnable()
    {
        // �I�u�W�F�N�g���L�����������̍��W�𐶐�
        m_spawnPos = transform.position;
        m_spawnPos.y = RespawnPosY;
    }

    public void Respawn()
    {
        transform.position = m_spawnPos;
        transform.rotation = Quaternion.identity;

        // �����I�ȋ��������Z�b�g
        if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���W�����ȉ��ł���΃��X�|�[���ʒu�ɐݒ�
        if (transform.position.y > TriggerPosY) return;

        Respawn();
    }
}
