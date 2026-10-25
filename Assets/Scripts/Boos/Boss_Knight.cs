using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Boss_Knight : MonoBehaviour
{
    public Transform target;
    public Collider2D ground;

    public float moveSpeed = 2f;
    public float wanderSpeed = 1f;
    public float wanderDistance = 3f;

    public float actionInterval = 2f;
    public float waitTime = 0.7f;

    [Range(0f, 1f)]
    public float chaseChance = 0.6f;

    private enum MoveState
    {
        Wander,
        Chase,
        Wait
    }

    [SerializeField]
    private MoveState state;

    private Rigidbody2D rb;
    private BoxCollider2D bodyCollider;

    private float timer;
    private float wanderTargetX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (ground == null)
        {
            Debug.LogError(
                "BossKnight의 Ground에 바닥을 연결해주세요.",
                this
            );

            enabled = false;
            return;
        }

        ChooseAction();
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        float speedX = 0f;

        if (state == MoveState.Wait)
        {
            if (timer <= 0f)
            {
                ChooseAction();
            }
        }
        else if (state == MoveState.Chase && target == null)
        {
            BeginWait();
        }
        else
        {
            // 추적 중이면 플레이어, 배회 중이면 선택한 지점이 목표다.
            float destinationX = state == MoveState.Chase
                ? target.position.x
                : wanderTargetX;

            destinationX = ClampToGround(destinationX);

            float distanceX = destinationX - rb.position.x;

            // 도착하거나 행동 시간이 끝나면 잠깐 쉰다.
            if (Mathf.Abs(distanceX) < 0.01f || timer <= 0f)
            {
                BeginWait();
            }
            else
            {
                float speed = state == MoveState.Chase
                    ? moveSpeed
                    : wanderSpeed;

                speed = Mathf.Max(0f, speed);

                // 목표 지점을 지나치지 않도록 속도를 제한한다.
                speedX = Mathf.Clamp(
                    distanceX / Time.fixedDeltaTime,
                    -speed,
                    speed
                );
            }
        }

        rb.linearVelocity = new Vector2(
            speedX,
            rb.linearVelocity.y
        );
    }

    private void ChooseAction()
    {
        bool chase = target != null
            && Random.Range(0f, 1f) < chaseChance;

        if (chase)
        {
            state = MoveState.Chase;
            timer = Mathf.Max(0.1f, actionInterval);
        }
        else
        {
            state = MoveState.Wander;

            // 현재 위치 주변에서 배회할 목표 지점을 고른다.
            float candidateX = rb.position.x + Random.Range(-wanderDistance, wanderDistance);

            wanderTargetX = ClampToGround(candidateX);

            Debug.Log(
                $"[배회] 기사X={rb.position.x:F2}, " +
                $"선택한목표X={candidateX:F2}, " +
                $"범위제한후X={wanderTargetX:F2}, " +
                $"바닥={ground.name}, " +
                $"바닥범위={ground.bounds.min.x:F2}~{ground.bounds.max.x:F2}",
                this
                );

        }
    }

    private void BeginWait()
    {
        state = MoveState.Wait;
        timer = Mathf.Max(0.1f, waitTime);
    }

    private float ClampToGround(float positionX)
    {
        // 기사 몸 전체가 바닥 범위 안에 있도록 계산한다.
        float halfWidth = bodyCollider.bounds.extents.x;

        float leftEdge = ground.bounds.min.x + halfWidth + 0.1f;
        float rightEdge = ground.bounds.max.x - halfWidth - 0.1f;

        if (leftEdge > rightEdge)
        {
            return ground.bounds.center.x;
        }

        return Mathf.Clamp(positionX, leftEdge, rightEdge);
    }
}