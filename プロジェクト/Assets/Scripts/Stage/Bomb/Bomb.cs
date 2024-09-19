using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    const float GraceTime = 3.0f;
    const float MinColorBlinkInterval = 1.0f;
    const float MaxColorBlinkInterval = 20.0f;

    [SerializeField] GameObject m_explosionEffect;
    [SerializeField] SphereCollider m_triggerCollider;
    [SerializeField] float m_power;
    [SerializeField] Material m_material;
    [SerializeField] AudioClip m_se;
    [SerializeField] bool m_enabled;

    List<GameObject> m_explosionObjectList = new List<GameObject>();
    List<GameObject> m_deleteList = new List<GameObject>();
    float m_timeLimit;

    private void Awake()
    {
        Initialize();
    }

    // ������
    void Initialize()
    {
        m_timeLimit = GraceTime;
        m_enabled = false;

        m_material.SetColor("_BaseColor", Color.black);
        GetComponent<CarryObject>().enabled = true;
    }

    public void OnIgnition()
    {
        m_enabled = true;
    }

    private void FixedUpdate()
    {
        if (m_enabled == false) return;

        // �P�\���Ԃ����炷
        m_timeLimit -= Time.deltaTime;

        // �}�e���A���̓_�ŏ���
        float interval = Mathf.Lerp(MinColorBlinkInterval, MaxColorBlinkInterval, 1 - m_timeLimit / GraceTime);
        float x = Mathf.PI * (1 - (m_timeLimit / GraceTime)) * interval;
        while (x > Mathf.PI)
        {
            x -= Mathf.PI;
        }
        float red = Mathf.Sin(x);
        Color color = new Color(red, 0, 0, 1);
        m_material.SetColor("_BaseColor", color);

        // �P�\���Ԃ������Ȃ�����
        if (m_timeLimit > 0) return;

        // ����
        // ������ԑ��x�͑���Ƃ̋����Ō��܂�
        foreach (GameObject gameObject in m_explosionObjectList)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if(gameObject.TryGetComponent<CarryObject>(out CarryObject carryObject)) { carryObject.enabled = true; }

            // ������΂�
            if (rigidbody.isKinematic)
            {
                // �����悻�����̃R���C�_�[���~����
                if(gameObject.TryGetComponent<BoxCollider>(out BoxCollider boxCollider))
                {
                    int value = Random.Range(0, 2);
                    boxCollider.enabled = value == 0 ? true : false;
                }

                rigidbody.isKinematic = false;
                rigidbody.AddExplosionForce(m_power, transform.position, m_triggerCollider.radius, 0, ForceMode.Impulse);

                // ������΂����I�u�W�F�N�g�͐ӔC�������ĉ��
                StageSceneManager.instance.AddStage(gameObject);
            }

            // ���e���g���ăX�e�[�W��ω����������Ƃ��
            StageCounter.OnUseItem((int)StageCounter.Item.Bomb);
        }

        // �G�t�F�N�g�𐶐�
        Instantiate(m_explosionEffect, transform.position, Quaternion.Euler(-90, 0, 0));

        // SE
        SoundEffect.Play25D(m_se, null, 0.3f, 1, false);

        // ���������čĔz�u
        GetComponent<RespawnItem>().Respawn();
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����𖞂����Ȃ��I�u�W�F�N�g�͐�����΂����X�g�ɒǉ����Ȃ�
        // �ERigidbody�������Ă��Ȃ�
        // �E�S���Ȃ��I�u�W�F�N�g
        // �E�v���C���[
        if(
            other.TryGetComponent<CarryObject>(out CarryObject carryObject) == false ||
            other.TryGetComponent<Rigidbody>(out Rigidbody rigidbody) == false ||
            other.TryGetComponent<Player>(out Player player)
            )
        {
            return;
        }

        m_explosionObjectList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // ������΂����X�g�ɓ����Ă��Ȃ��Ȃ�X�L�b�v
        if (m_explosionObjectList.Contains(other.gameObject) == false) return;

        m_explosionObjectList.Remove(other.gameObject);
    }
}
