using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
    List<Transform> m_cubeList = new List<Transform>();

    private void OnEnable()
    {
        // 子要素のCubeをすべて保存
        foreach(Transform child in transform)
        {
            m_cubeList.Add(child);
        }
    }

    private void OnDisable()
    {
        // 自身が削除されたときにCubeすべても削除する
        while (m_cubeList.Count != 0)
        {
            Transform cube = m_cubeList[0];
            m_cubeList.Remove(cube);
            Destroy(cube.gameObject);
        }
    }

    public void OnInvalidCarry()
    {
        // すべてのCubeを担げなくする
        foreach (Transform transform in m_cubeList)
        {
            // 見えない壁を抜けるCubeは壁のみのステージまで言っていないなら担げるようにする
            if (
                transform.gameObject.layer == 6 &&
                StageCounter.CheckClearBelow((int)StageCounter.Item.Wall) == false
                ) continue;
            transform.gameObject.GetComponent<CarryObject>().enabled = false;
            transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
