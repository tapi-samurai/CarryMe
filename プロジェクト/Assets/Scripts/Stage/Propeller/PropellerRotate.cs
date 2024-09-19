using UnityEngine;

public class PropellerRotate : MonoBehaviour
{
    [SerializeField] GameObject m_propeller;
    [SerializeField] float m_rotateSpeed = 57.6f;
    [SerializeField] bool m_canRotate;
    [SerializeField] AudioClip m_se;
    [SerializeField] GameObject m_seObject;

    public void OnStart()
    {
        m_canRotate = true;

        // SEÇê∂ê¨
        m_seObject = SoundEffect.Play25D(m_se, this.transform, 0.3f, 1.5f, true);
    }

    public void OnStop()
    {
        m_canRotate = false;

        // SEÇçÌèú
        Destroy(m_seObject);
    }

    private void FixedUpdate()
    {
        if (m_canRotate == false) return;

        m_propeller.transform.Rotate(0, m_rotateSpeed, 0);
    }
}
