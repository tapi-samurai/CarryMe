using System.Collections.Generic;
using UnityEngine;

// プレイヤーのSEを管理・再生

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
        // リストから該当するデータを抽出
        SoundData data = null;
        foreach(SoundData soundData in m_seList)
        {
            if (soundData.name != name) continue;

            data = soundData;

            // 足音であればピッチをランダムで変化させる
            data.pitch = Random.Range(0.5f, 1.0f);
        }

        // データがあれば再生
        if(data != null)
        {
            SoundEffect.Play25D(data.clip, this.transform, data.volume, data.pitch, data.loop);
        }
    }
}
