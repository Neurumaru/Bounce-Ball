using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    private Rigidbody2D rigid2D;

    private bool doubleJump;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        doubleJump = false;
    }

    void Update()
    {
        if(doubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            Ball.Jump(rigid2D, Ball.floorJumpHeight);
            doubleJump = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "JumpTwoStage")
        {
            doubleJump = true;
        }
    }
}
