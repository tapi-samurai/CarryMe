using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// コライダーの範囲に見えない壁を生成

public class SetMoveRestrickCollider : MonoBehaviour
{
    private void Awake()
    {
        // コライダーと表示を削除
        Collider collider = GetComponent<Collider>();
        DestroyImmediate(collider);
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        // メッシュを反転
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();

        gameObject.AddComponent<MeshCollider>();
    }
}
