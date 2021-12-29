using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed;   // ��ü �ӵ�
    private Rigidbody2D rigid2D;
    private Vector2 firstPos;
    private bool isSideCollision = false;
    private bool isStraight = false;
    private bool isTwoStage = false;
    private Collision2D collisionObject; // �浹 ��ü
    public float jumpHeight = 5.0f;   // ��ü ���� ���̰�
    private int maxJumpCount = 0;
    float sb_moveX = 0; // straight�� �浹 �� ���� x�� ������

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

        if (other.gameObject.tag == "RStraight" || other.gameObject.tag == "LStraight")    // �� �±� ����
        {
            isStraight = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Goal");
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); // �浹 ��� �����ϰ�
            // ���� Ŭ���� ������ ��ȯ
        }
    }

    void OnBecameInvisible()
    {
        Die();
    }



    // RLStraight�� �浹 �Լ�
    private void Straight(Collision2D other)
    {
        rigid2D.gravityScale = 0;

        if (other.gameObject.tag == "RStraight")
            rigid2D.MovePosition(new Vector2(other.transform.position.x + other.transform.localScale.x + sb_moveX, other.transform.position.y));
        else if (other.gameObject.tag == "LStraight")
            rigid2D.MovePosition(new Vector2(other.transform.position.x - other.transform.localScale.x - sb_moveX, other.transform.position.y));

        // ȭ��ǥ Ű �Է� or ���� �浹 �� �߷� �ۿ�
        // �� �κ� Ű �Է� ������ �� �ȵ�!
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || isSideCollision)
        {
            isStraight = false;
            isSideCollision = false;
            rigid2D.gravityScale = 1;
            sb_moveX = 0;
        }
        sb_moveX += (float)0.1;
    }

    // JumpTwoStage�� �浹 �Լ�
    private void JumpTwoStage()
    {
        maxJumpCount = 1;
        // �� �κе� Ű �Է� ������ �� �ȵ�!
        if (maxJumpCount > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rigid2D.velocity = Vector2.up * jumpHeight;
            maxJumpCount--;
            isTwoStage = false;
        }
    }

    // �浹 ���� Ȯ�� �� �����ϴ� �Լ�
    private void JumpSide(Collision2D other)
    {
        Vector3 hit = other.contacts[0].normal;         //ù��° �浹������ ��������
        float angle = Vector3.Angle(hit, Vector3.up);   //hit���� (0,1,0)������ ���� ��ȯ, Vector3.up=(0, 1, 0)

        for (int i = 0; i <= 45; i++)                   // �𼭸� �ε����� �� �̲����� �ּ�ȭ
        {
            if (Mathf.Approximately((int)angle, i))     //Mathf.Approximately: ���� �ΰ� ��(������ true, �ٸ��� false)
            {
                isSideCollision = false;
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpHeight);
                Debug.Log("Down");
            }
        }
        if (Mathf.Approximately(angle, 90))
        {
            Vector3 cross = Vector3.Cross(Vector3.forward, hit);    // Vector3.Cross:�� ������ ����, Vector3.forward=(0,0,1)
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
    
    // �� �̵� �Լ�
    public void Move(float h)
    {
        rigid2D.velocity = new Vector2(h * moveSpeed, rigid2D.velocity.y);
    }

    // �� �ʱ�ȭ
    void Die()
    {
        rigid2D.Sleep();
        rigid2D.position = firstPos;
        rigid2D.WakeUp();
    }
}