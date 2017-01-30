using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    public KeyboardMouseConfig keyboardMouseConfig;
    public WindowsGamepadConfig windowsGamepadConfig;
    public float direction;
    public bool grabbing;
    public bool shooting;

    void Update()
    {
        this.SetDirection();
    }

    void SetDirection()
    {
        this.direction = 0;
        if (Input.GetKey(this.keyboardMouseConfig.left) || Input.GetAxisRaw("Horizontal") < 0)
        {
            this.direction = -1;
        }
        else if (Input.GetKey(this.keyboardMouseConfig.right) || Input.GetAxisRaw("Horizontal") > 0)
        {
            this.direction = 1;
        }
        grabbing = Input.GetKeyDown(this.keyboardMouseConfig.grab) || Input.GetKeyDown(this.windowsGamepadConfig.grab);
        shooting = Input.GetKey(this.keyboardMouseConfig.shot) || Input.GetKey(this.windowsGamepadConfig.shot);
    }
}