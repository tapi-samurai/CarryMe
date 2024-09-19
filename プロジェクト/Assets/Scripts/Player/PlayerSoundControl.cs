using System.Collections.Generic;
using UnityEngine;

// �v���C���[��SE���Ǘ��E�Đ�

public class PlayerSoundControl : MonoBehaviour
{
    [Header("[SE]")]
    [SerializeField] List<SoundData> m_seList;

    [System.Serializable]
    class SoundData
    {
        public string name;
        public AudioClip clip;
        public float volume;
        public float pitch;
        public bool loop;
    }

    public void PlaySE(string name)
    {
        // ���X�g����Y������f�[�^�𒊏o
        SoundData data = null;
        foreach(SoundData soundData in m_seList)
        {
            if (soundData.name != name) continue;

            data = soundData;

            // �����ł���΃s�b�`�������_���ŕω�������
            data.pitch = Random.Range(0.5f, 1.0f);
        }

        // �f�[�^������΍Đ�
        if(data != null)
        {
            SoundEffect.Play25D(data.clip, this.transform, data.volume, data.pitch, data.loop);
        }
    }
}
