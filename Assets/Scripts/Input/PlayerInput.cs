using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    public KeyboardMouseConfig keyboardMouseConfig;
    public WindowsGamepadConfig windowsGamepadConfig;
    public float horizontalDirection;
    public float verticalDirection;
    public bool grabbed;
    public bool holdingJump;
    public bool jumped;
    public bool startedShooting;
    public bool shooting;
    public bool teleported;
    public bool threw;
    public bool crushed;
    public bool lockAim;

    void Update()
    {
        this.SetDirection();
        this.SetJump();
        this.SetGrab();
        this.SetShoot();
        this.SetThrow();
        this.SetCrush();
        this.SetLockAim();
    }

    void SetDirection()
    {
        this.horizontalDirection = 0;
        if (Input.GetKey(this.keyboardMouseConfig.left) || Input.GetAxisRaw("Horizontal") < 0)
        {
            this.horizontalDirection = -1;
        }
        else if (Input.GetKey(this.keyboardMouseConfig.right) || Input.GetAxisRaw("Horizontal") > 0)
        {
            this.horizontalDirection = 1;
        }
        this.verticalDirection = 0;
        if (Input.GetKey(this.keyboardMouseConfig.down) || Input.GetAxisRaw("Vertical") < 0)
        {
            this.verticalDirection = -1;
        }
        else if (Input.GetKey(this.keyboardMouseConfig.up) || Input.GetAxisRaw("Vertical") > 0)
        {
            this.verticalDirection = 1;
        }
    }

    void SetJump()
    {
        jumped = Input.GetKeyDown(this.keyboardMouseConfig.jump) || Input.GetKeyDown(this.windowsGamepadConfig.jump);
        teleported = Input.GetKeyDown(this.keyboardMouseConfig.teleport) || Input.GetKeyDown(this.windowsGamepadConfig.teleport);
        holdingJump = Input.GetKey(this.keyboardMouseConfig.jump) || Input.GetKey(this.windowsGamepadConfig.jump);
    }

    void SetGrab()
    {
        grabbed = Input.GetKeyDown(this.keyboardMouseConfig.grab) || Input.GetKeyDown(this.windowsGamepadConfig.grab);
    }

    void SetThrow()
    {
        threw = GetKeyDown(this.keyboardMouseConfig.throwEnemy) || GetKeyDown(this.windowsGamepadConfig.throwEnemy);
    }

    void SetShoot()
    {
        startedShooting = Input.GetKeyDown(this.keyboardMouseConfig.shot) || Input.GetKeyDown(this.windowsGamepadConfig.shot);
        shooting = Input.GetKey(this.keyboardMouseConfig.shot) || Input.GetKey(this.windowsGamepadConfig.shot);
    }

    void SetCrush()
    {
        crushed = Input.GetKey(this.keyboardMouseConfig.crush) || Input.GetKey(this.windowsGamepadConfig.crush);
    }

    void SetLockAim()
    {
        lockAim = Input.GetKey(this.keyboardMouseConfig.lockAim) || Input.GetKey(this.windowsGamepadConfig.lockAim);
    }

    bool GetKeyDown(KeyCode[] keys)
    {
        for (int i = 0; i<keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                return true;
            }
        }
        return false;
    }
}