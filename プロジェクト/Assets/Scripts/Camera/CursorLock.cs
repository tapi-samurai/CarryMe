using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()
    {
        // ‰Šúİ’è
        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
