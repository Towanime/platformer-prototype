using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    public KeyboardMouseConfig keyboardMouseConfig;
    public WindowsGamepadConfig windowsGamepadConfig;
    public float direction;
    public bool grabbing;

    void Update()
    {
        this.SetDirection();
    }

    void SetDirection()
    {
        this.direction = 0;
        if (Input.GetKey(this.keyboardMouseConfig.left) || Input.GetKey(this.windowsGamepadConfig.left))
        {
            this.direction = -1;
        }
        else if (Input.GetKey(this.keyboardMouseConfig.right) || Input.GetKey(this.windowsGamepadConfig.right))
        {
            this.direction = 1;
        }
        grabbing = Input.GetKey(this.keyboardMouseConfig.grab) || Input.GetKey(this.windowsGamepadConfig.grab);
    }
}