using UnityEngine;

public class WeaponShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float baseShakeAmount = 0.05f;   // shake at slow speed
    public float maxShakeAmount = 0.2f;     // max shake at sprint
    public float shakeSpeed = 5f;           // how fast weapon jitters
    public float returnSpeed = 10f;         // how fast it returns to rest
    public float maxSpeed = 10f;            // player max speed (for scaling)

    private Vector3 startLocalPos;
    private Vector3 lastPlayerPos;
    private Transform player;

    void Start()
    {
        startLocalPos = transform.localPosition;
        player = transform.root; // assumes weapon is child of player
        lastPlayerPos = player.position;
    }

    void Update()
    {
        Vector3 playerDelta = player.position - lastPlayerPos;
        float speed = playerDelta.magnitude / Time.deltaTime; // units per sec

        float currentShake = Mathf.Lerp(baseShakeAmount, maxShakeAmount, speed / maxSpeed);

        if (speed > 0.01f)
        {
            
            Vector3 offset = new Vector3(
                (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f),
                (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f),
                0f
            ) * currentShake;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startLocalPos + offset,
                Time.deltaTime * shakeSpeed
            );
        }
        else 
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startLocalPos,
                Time.deltaTime * returnSpeed
            );
        }

        lastPlayerPos = player.position;
    }
}
