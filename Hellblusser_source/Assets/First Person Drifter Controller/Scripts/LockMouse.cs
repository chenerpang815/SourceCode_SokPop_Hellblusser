using UnityEngine;

public class LockMouse : MonoBehaviour
{
    void Awake ()
    {
        LockCursor();
    }

    void Update ()
    {
        LockCursor();
    }

    public void LockCursor ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}