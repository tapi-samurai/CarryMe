using UnityEngine;

public class SemiAutomaticDoor : MonoBehaviour
{
    const float OpenTime = 2.0f;
    const float Length = 2.0f;      // どれだけ動くか

    [SerializeField] GameObject m_door1;
    [SerializeField] GameObject m_door2;

    static bool m_isOpen;  // 監視する変数

    float m_elapsedTime;

    private void OnEnable()
    {
        m_elapsedTime = 0;
        m_isOpen = false;
    }

    public static void OnOpen()
    {
        m_isOpen = true;
    }

    private void FixedUpdate()
    {
        if (m_elapsedTime > OpenTime) return;
        if (m_isOpen == false) return;

        m_elapsedTime += Time.deltaTime;

        // ドアが開くアニメーション
        float z = Mathf.Lerp(0, Length, m_elapsedTime / OpenTime);
        m_door1.transform.localPosition = new Vector3(0,0,z);
        m_door2.transform.localPosition = new Vector3(0,0,-z);
        m_door1.transform.localScale = new Vector3(1, 1, 1 - z / Length);
        m_door2.transform.localScale = new Vector3(1, 1, 1 - z / Length);

        if (m_elapsedTime > OpenTime)
        {
            Destroy(m_door1);
            Destroy(m_door2);
            this.enabled = false;
        }
    }
}
