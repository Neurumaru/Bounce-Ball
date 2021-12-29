using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static int floorJumpHeight = 5;
    public static int highjumpJumpHeight = 7;
    public static int defaultMoveSpeed = 5;

    public static void Jump(Rigidbody2D rigid2D, Collision2D other, int jumpHeight)
    {
        foreach (ContactPoint2D contact in other.contacts)
        {
            Vector3 hit = contact.normal;         //ù��° �浹������ ��������
            float angle = Vector3.Angle(hit, Vector3.up);   //hit���� (0,1,0)������ ���� ��ȯ, Vector3.up=(0, 1, 0)

            if (angle > 270 || angle < 90)                  //Mathf.Approximately: ���� �ΰ� ��(������ true, �ٸ��� false)
            {
                Jump(rigid2D, jumpHeight);
            }
        }
    }

    public static void Jump(Rigidbody2D rigid2D, int jumpHeight)
    {
        rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpHeight);
    }

    public static void Move(Rigidbody2D rigid2D, float speed)
    {
        rigid2D.velocity = new Vector2(speed, rigid2D.velocity.y);
    }

    public static void Stop(Rigidbody2D rigid2D)
    {
        rigid2D.velocity = new Vector2(0, 0);
    }

    public static void Die(Rigidbody2D rigid2D, Vector2 firstPos)
    {
        rigid2D.gravityScale = 1;
        rigid2D.position = firstPos;
        Ball.Stop(rigid2D);
    }

    public static void ZeroGravity(Rigidbody2D rigid2D, Collision2D other)
    {
        rigid2D.gravityScale = 0;
        Ball.Stop(rigid2D);
    }
}
