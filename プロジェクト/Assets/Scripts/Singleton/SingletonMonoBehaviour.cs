using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    private static T m_instance;

    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                //  アクセスされたらまずは、インスタンスがあるか調べる
                m_instance = (T)FindObjectOfType(typeof(T));

                if (m_instance == null)
                {
                    // なければインスタンスを生成
                    m_instance = new GameObject().AddComponent<T>();
                }
            }

            return m_instance;
        }
    }
}