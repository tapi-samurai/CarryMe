using UnityEngine;

public class LookAtCameraUI : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
