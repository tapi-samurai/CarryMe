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
        // コメントの位置をスクリーン上の座標に変換
        Vector2 screenPos = RectTranformPosToScreenPos(m_testComment.localPosition);

        // レイを生成
        Ray ray = m_canvas.worldCamera.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction.normalized * 100);

        // 交点座標を生成
        Vector3 intersectPos = CreateIntersectPos(ray);

        m_carryPlayerPrefab.transform.position = intersectPos;
    }

    // ステージ上の疑似的な平面とコメントに伸びるベクトルの交点座標を生成
    Vector3 CreateIntersectPos(Ray ray)
    {
        Vector3 n = -ray.direction.normalized;  // 平面の法線
        Vector3 x = Vector3.zero;               // 平面の座標
        Vector3 x0 = ray.origin;                // ベクトルの始点
        Vector3 m = ray.direction.normalized;   // ベクトルの方向
        float h = Vector3.Dot(n, x);            // 平面の法線と座標の内積

        // 交点座標を生成
        Vector3 intersectPos = x0 + ((h - Vector3.Dot(n, x0)) / (Vector3.Dot(n, m))) * m;

        return intersectPos;
    }

    // RectTransformのLocal座標をスクリーン座標に変換
    Vector2 RectTranformPosToScreenPos(Vector3 rectTransformPos)
    {
        Vector2 screenPos = new Vector2(
            rectTransformPos.x + Screen.width / 2,
            rectTransformPos.y + Screen.height / 2
            );

        return screenPos;
    }
}
