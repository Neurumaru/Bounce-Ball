using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        firstPos = rigid2D.position;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            Ball.Die(rigid2D, firstPos);
        }
    }
}
