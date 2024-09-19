using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// �R���C�_�[�͈̔͂Ɍ����Ȃ��ǂ𐶐�

public class SetMoveRestrickCollider : MonoBehaviour
{
    private void Awake()
    {
        // �R���C�_�[�ƕ\�����폜
        Collider collider = GetComponent<Collider>();
        DestroyImmediate(collider);
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        // ���b�V���𔽓]
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();

        gameObject.AddComponent<MeshCollider>();
    }
}
