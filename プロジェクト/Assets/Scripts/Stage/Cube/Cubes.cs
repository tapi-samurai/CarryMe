using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
    List<Transform> m_cubeList = new List<Transform>();

    private void OnEnable()
    {
        // �q�v�f��Cube�����ׂĕۑ�
        foreach(Transform child in transform)
        {
            m_cubeList.Add(child);
        }
    }

    private void OnDisable()
    {
        // ���g���폜���ꂽ�Ƃ���Cube���ׂĂ��폜����
        while (m_cubeList.Count != 0)
        {
            Transform cube = m_cubeList[0];
            m_cubeList.Remove(cube);
            Destroy(cube.gameObject);
        }
    }

    public void OnInvalidCarry()
    {
        // ���ׂĂ�Cube��S���Ȃ�����
        foreach (Transform transform in m_cubeList)
        {
            // �����Ȃ��ǂ𔲂���Cube�͕ǂ݂̂̃X�e�[�W�܂Ō����Ă��Ȃ��Ȃ�S����悤�ɂ���
            if (
                transform.gameObject.layer == 6 &&
                StageCounter.CheckClearBelow((int)StageCounter.Item.Wall) == false
                ) continue;
            transform.gameObject.GetComponent<CarryObject>().enabled = false;
            transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
