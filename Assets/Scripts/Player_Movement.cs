
using UnityEngine;

public class Player_Movemonet : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("바닥 확인")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    void Awake()
    {
        // 플레이어의 물리 컴포넌트를 가져온다.
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 좌우 입력을 읽는다. 왼쪽은 -1, 오른쪽은 1이다.
        moveInput = Input.GetAxisRaw("Horizontal");

        // 발밑에 바닥이 있는지 확인한다.
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // 바닥에 있을 때 Space를 누르면 점프한다.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x, 0f
            );

            rb.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse
            );
        }
    }

    void FixedUpdate()
    {
        // 물리 업데이트에서 좌우 속도를 적용한다.
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );
    }
}