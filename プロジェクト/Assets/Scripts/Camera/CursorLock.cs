using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()
    {
        // �����ݒ�
        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
