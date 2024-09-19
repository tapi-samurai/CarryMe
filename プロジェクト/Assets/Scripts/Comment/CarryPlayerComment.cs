using UnityEngine;

public class CarryPlayerComment : MonoBehaviour
{
    [SerializeField] GameObject m_carryPlayerPrefab;

    [SerializeField] RectTransform m_testComment;
    public Canvas m_canvas;

    private void OnEnable()
    {
        GameObject gameObject = Instantiate(m_carryPlayerPrefab);
        m_carryPlayerPrefab = gameObject;
    }

    private void OnDisable()
    {
        Destroy(m_carryPlayerPrefab);
    }

    private void FixedUpdate()
    {
        // �R�����g�̈ʒu���X�N���[����̍��W�ɕϊ�
        Vector2 screenPos = RectTranformPosToScreenPos(m_testComment.localPosition);

        // ���C�𐶐�
        Ray ray = m_canvas.worldCamera.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction.normalized * 100);

        // ��_���W�𐶐�
        Vector3 intersectPos = CreateIntersectPos(ray);

        m_carryPlayerPrefab.transform.position = intersectPos;
    }

    // �X�e�[�W��̋^���I�ȕ��ʂƃR�����g�ɐL�т�x�N�g���̌�_���W�𐶐�
    Vector3 CreateIntersectPos(Ray ray)
    {
        Vector3 n = -ray.direction.normalized;  // ���ʂ̖@��
        Vector3 x = Vector3.zero;               // ���ʂ̍��W
        Vector3 x0 = ray.origin;                // �x�N�g���̎n�_
        Vector3 m = ray.direction.normalized;   // �x�N�g���̕���
        float h = Vector3.Dot(n, x);            // ���ʂ̖@���ƍ��W�̓���

        // ��_���W�𐶐�
        Vector3 intersectPos = x0 + ((h - Vector3.Dot(n, x0)) / (Vector3.Dot(n, m))) * m;

        return intersectPos;
    }

    // RectTransform��Local���W���X�N���[�����W�ɕϊ�
    Vector2 RectTranformPosToScreenPos(Vector3 rectTransformPos)
    {
        Vector2 screenPos = new Vector2(
            rectTransformPos.x + Screen.width / 2,
            rectTransformPos.y + Screen.height / 2
            );

        return screenPos;
    }
}
