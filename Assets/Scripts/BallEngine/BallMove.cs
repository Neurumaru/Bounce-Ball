using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        firstPos = rigid2D.position;
        moveSpeed = Ball.defaultMoveSpeed;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (rigid2D.gravityScale != 0)
            Ball.Move(rigid2D, h * moveSpeed);
    }

    void OnBecameInvisible()
    {
        Ball.Die(rigid2D, firstPos);
    }
}
