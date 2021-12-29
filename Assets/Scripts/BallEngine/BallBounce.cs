using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;

    // Start is called before the first frame update
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        firstPos = rigid2D.position;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "JumpTwoStage")
        {
            Ball.Jump(rigid2D, other, Ball.floorJumpHeight);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "JumpTwoStage")
        {
            Ball.Jump(rigid2D, other, Ball.floorJumpHeight);
        }
    }
}
