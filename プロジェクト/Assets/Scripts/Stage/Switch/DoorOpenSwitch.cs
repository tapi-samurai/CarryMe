using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class DoorOpenSwitch : MonoBehaviour
{
    readonly Color BaseColor = new Color(0.9647059f, 0.772549f, 0.317647f, 1);
    readonly Color BaseEmission = new Color(1, 0.572549f, 0);
    const float MaxIntensity = 5.0f;    // �{�^���̍ō����x
    const float MinIntensity = 0.001f;  // �{�^���̍Œ���x
    const float PressTime = 1.0f;       // ��������

    [SerializeField] Material m_material;
    [SerializeField] SwitchType m_switchType;

    float m_elapsedTime;
    bool m_enabled;

    enum SwitchType
    {
        Near,
        Far,
    }

    private void Awake()
    {
        m_enabled = false;

        // �{�^���̐F��������
        m_material.SetColor("_BaseColor", BaseColor);
        m_material.SetColor("_EmissionColor", BaseEmission * 0.001f);
    }

    private void FixedUpdate()
    {
        if (m_enabled == false) return;

        m_elapsedTime += Time.deltaTime;

        // �{�^�������点�鏈��
        float intensity = Mathf.Lerp(MinIntensity, MaxIntensity, Mathf.Sin(Mathf.PI * m_elapsedTime / PressTime));
        m_material.SetColor("_EmissionColor", BaseEmission * intensity);

        // MEMO �ł����
        // ���܂�Ă���Ԃ̓{�^�����ւ���


        // ����Ă��Ȃ���΂��Ƃɖ߂��Ă���


        // ��莞�Ԃ��o�߂�����h�A���J��
        if (m_elapsedTime > PressTime)
        {
            SemiAutomaticDoor.OnOpen();
            this.enabled = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_enabled == false)
        {
            m_enabled = true;

            // �X�C�b�`�����������Ƃ��
            StageCounter.Item useItem = m_switchType == SwitchType.Near
                ? StageCounter.Item.NearSwitch : StageCounter.Item.FarSwitch;
            StageCounter.OnUseItem((int)useItem);
        }
    }
}