using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;   // 객체 속도
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;
    private bool isSideCollision = false;
    private bool isStraight = false;
    private bool isTwoStage = false;
    private Collision2D collisionObject; // 충돌 물체
    public float jumpHeight = 5.0f;   // 객체 점프 높이값
    private int maxJumpCount = 0;
    float sb_moveX = 0; // straight블럭 충돌 시 공의 x축 증가값

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        firstPos = rigid2D.position;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(h);
        if (isStraight)
        {
            Straight(collisionObject);
        }
        if (isTwoStage)
        {
            JumpTwoStage();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        collisionObject = other;
        if (other.gameObject.tag == "HighJump")
        {
            jumpHeight = 7;
            JumpSide(other);
        }
        else
        {
            jumpHeight = 5;
        }

        if (other.gameObject.tag == "Floor")
        {
            JumpSide(other);
        }

        if (other.gameObject.tag == "JumpTwoStage")
        {
            isTwoStage = true;
            JumpSide(other);
        }

        if (other.gameObject.tag == "RStraight" || other.gameObject.tag == "LStraight")    // 블럭 태그 구분
        {
            isStraight = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Goal");
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); // 충돌 대상 투명하게
            // 게임 클리어 씬으로 전환
        }
    }

    void OnBecameInvisible()
    {
        Die();
    }



    // RLStraight블럭 충돌 함수
    private void Straight(Collision2D other)
    {
        rigid2D.gravityScale = 0;

        if (other.gameObject.tag == "RStraight")
            rigid2D.MovePosition(new Vector2(other.transform.position.x + other.transform.localScale.x + sb_moveX, other.transform.position.y));
        else if (other.gameObject.tag == "LStraight")
            rigid2D.MovePosition(new Vector2(other.transform.position.x - other.transform.localScale.x - sb_moveX, other.transform.position.y));

        // 화살표 키 입력 or 벽과 충돌 시 중력 작용
        // 이 부분 키 입력 감지가 잘 안됨!
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || isSideCollision)
        {
            isStraight = false;
            isSideCollision = false;
            rigid2D.gravityScale = 1;
            sb_moveX = 0;
        }
        sb_moveX += (float)0.1;
    }

    // JumpTwoStage블럭 충돌 함수
    private void JumpTwoStage()
    {
        maxJumpCount = 1;
        // 이 부분도 키 입력 감지가 잘 안됨!
        if (maxJumpCount > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rigid2D.velocity = Vector2.up * jumpHeight;
            maxJumpCount--;
            isTwoStage = false;
        }
    }

    // 충돌 방향 확인 후 점프하는 함수
    private void JumpSide(Collision2D other)
    {
        Vector3 hit = other.contacts[0].normal;         //첫번째 충돌지점의 법선벡터
        float angle = Vector3.Angle(hit, Vector3.up);   //hit에서 (0,1,0)까지의 각도 반환, Vector3.up=(0, 1, 0)

        for (int i = 0; i <= 45; i++)                   // 모서리 부딪혔을 때 미끄러짐 최소화
        {
            if (Mathf.Approximately((int)angle, i))     //Mathf.Approximately: 각도 두개 비교(같으면 true, 다르면 false)
            {
                isSideCollision = false;
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpHeight);
                Debug.Log("Down");
            }
        }
        if (Mathf.Approximately(angle, 90))
        {
            Vector3 cross = Vector3.Cross(Vector3.forward, hit);    // Vector3.Cross:두 벡터의 외적, Vector3.forward=(0,0,1)
            isSideCollision = true;
            if (cross.y > 0)
            {
                Debug.Log("Left");
            }
            else
            {
                Debug.Log("Right");
            }
        }
        if (Mathf.Approximately(angle, 180))
        {
            isSideCollision = false;
            Debug.Log("Up");
        }
    }
    
    // 공 이동 함수
    public void Move(float h)
    {
        rigid2D.velocity = new Vector2(h * moveSpeed, rigid2D.velocity.y);
    }

    // 공 초기화
    void Die()
    {
        rigid2D.Sleep();
        rigid2D.position = firstPos;
        rigid2D.WakeUp();
    }
}