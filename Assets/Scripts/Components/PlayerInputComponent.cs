using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputComponent : MonoBehaviour
{
    [SerializeField]
    private string horizontalInputAxisName;
    [SerializeField]
    private KeyCode jumpKey;

    public float GetHorizontalInputValue ()
    {
        return Input.GetAxis(horizontalInputAxisName);
    }

    public bool GetJumpState ()
    {
        return Input.GetKey(jumpKey);
    }
}
