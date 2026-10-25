using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class BossWeakPoint : MonoBehaviour
{
    public SpriteRenderer weakPointMarker;
    public float switchInterval = 3f;
    public float markerGap = 0.2f;

    private Rigidbody2D rb;
    private BoxCollider2D bodyCollider;

    private float timer;
    private int facingDirection = 1;
    private bool weakPointInFront = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        timer = switchInterval;
    }

    private void LateUpdate()
    {
        // 마지막으로 이동한 방향을 앞쪽으로 기억한다.
        if (rb.linearVelocity.x > 0.05f)
        {
            facingDirection = 1;
        }
        else if (rb.linearVelocity.x < -0.05f)
        {
            facingDirection = -1;
        }

        // 일정 시간이 지나면 앞쪽과 뒤쪽을 바꾼다.
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            weakPointInFront = !weakPointInFront;
            timer = Mathf.Max(0.1f, switchInterval);
        }

        if (weakPointMarker == null)
        {
            return;
        }

        int side = weakPointInFront
            ? facingDirection
            : -facingDirection;

        // 기사 충돌 범위의 가장자리 바깥에 표식을 놓는다.
        Bounds bodyBounds = bodyCollider.bounds;

        weakPointMarker.transform.position = new Vector3(
            bodyBounds.center.x
                + side * (bodyBounds.extents.x + markerGap),
            bodyBounds.center.y,
            transform.position.z
        );

        weakPointMarker.color = weakPointInFront
            ? Color.yellow
            : Color.cyan;
    }
}