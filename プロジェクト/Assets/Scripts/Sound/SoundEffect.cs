using UnityEngine;

// サウンドの生成・設定・配置・再生

public class SoundEffect
{
    public static void Play2D(AudioClip audioClip, float volume, float pitch, bool loop)
    {
        CreateSound(audioClip, null, volume, 0, pitch, loop);
    }

    public static GameObject Play25D(AudioClip audioClip, Transform parent, float volume, float pitch, bool loop)
    {
        return CreateSound(audioClip, parent, volume, 0.8f, pitch, loop);
    }

    public static void Play3D(AudioClip audioClip, Transform parent, float volume, float pitch, bool loop)
    {
        CreateSound(audioClip, parent, volume, 1, pitch, loop);
    }

    static GameObject CreateSound(AudioClip audioClip, Transform parent = null, float volume = 1, float spatialBlend = 0, float pitch = 1, bool loop = false)
    {
        GameObject gameObject = new GameObject();
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        gameObject.transform.parent = parent;
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        audioSource.pitch = pitch;

        // ループしなければ再生後に削除
        audioSource.Play();
        if(loop == false) MonoBehaviour.Destroy(gameObject, audioSource.clip.length * (1.0f / pitch));

        return gameObject;
    }
}
