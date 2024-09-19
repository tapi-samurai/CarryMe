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
                //  �A�N�Z�X���ꂽ��܂��́A�C���X�^���X�����邩���ׂ�
                m_instance = (T)FindObjectOfType(typeof(T));

                if (m_instance == null)
                {
                    // �Ȃ���΃C���X�^���X�𐶐�
                    m_instance = new GameObject().AddComponent<T>();
                }
            }

            return m_instance;
        }
    }
}