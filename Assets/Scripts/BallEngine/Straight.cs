using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;

    private bool straight;


    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        firstPos = rigid2D.position;
        straight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (straight && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space)))
        {
            rigid2D.gravityScale = 1;
            straight = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (straight)
        {
            rigid2D.gravityScale = 1;
            straight = false;
            Ball.Stop(rigid2D);
        }
        if (other.gameObject.tag == "LStraight")
        {
            straight = true;
            rigid2D.transform.position=new Vector2(other.transform.position.x - other.transform.localScale.x, other.transform.position.y);
            Ball.ZeroGravity(rigid2D, other);
            Ball.Move(rigid2D, -Ball.defaultMoveSpeed);
        }
        else if (other.gameObject.tag == "RStraight")
        {
            straight = true;
            rigid2D.transform.position = new Vector2(other.transform.position.x + other.transform.localScale.x, other.transform.position.y);
            Ball.ZeroGravity(rigid2D, other);
            Ball.Move(rigid2D, Ball.defaultMoveSpeed);
        }
    }
}
