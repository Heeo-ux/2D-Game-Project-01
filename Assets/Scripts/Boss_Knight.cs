using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossKnight : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    public float actionInterval = 1.5f;
    public float chaseDistance = 5f;

    private Rigidbody2D rb;
    private float actionTimer;
    private bool isChasing;
    private int wanderDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float speedX = 0f;

        if (target != null)
        {
            float distanceX = target.position.x - rb.position.x;

            actionTimer -= Time.fixedDeltaTime;

            // 시간이 지나면 다음 행동을 선택한다.
            if (actionTimer <= 0f)
            {
                ChooseAction();
            }

            // 너무 멀어지면 추적으로 전환한다.
            if (Mathf.Abs(distanceX) > chaseDistance && !isChasing)
            {
                isChasing = true;
                actionTimer = actionInterval;
            }

            if (isChasing)
            {
                // 플레이어를 향해 이동하다가 가까워지면 멈춘다.
                if (Mathf.Abs(distanceX) > stopDistance)
                {
                    speedX = Mathf.Sign(distanceX) * moveSpeed;
                }
            }
            else
            {
                // 배회할 때는 추적 속도의 절반으로 움직인다.
                speedX = wanderDirection * moveSpeed * 0.5f;
            }
        }

        // 세로 속도를 유지해서 중력이 계속 작동하게 한다.
        rb.linearVelocity = new Vector2(speedX, rb.linearVelocity.y);
    }

    private void ChooseAction()
    {
        actionTimer = actionInterval;

        // 0~9 중에서 0~5가 나오면 추적한다.
        isChasing = Random.Range(0, 10) < 6;

        // -1: 왼쪽, 0: 정지, 1: 오른쪽
        wanderDirection = Random.Range(-1, 2);
    }
}