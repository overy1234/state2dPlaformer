using UnityEngine;

public class SinWaveScaling : MonoBehaviour
{
    public float speed = 1.0f;  // 속도 조절
    public float scaleFactor = 0.02f;  // 크기 변화량

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float time = Time.time * speed; // 시간에 속도를 곱함
        float sinValue = Mathf.Sin(time); // 사인 함수 적용
        float scaleChange = sinValue * scaleFactor; // 스케일 변화량 계산

        transform.localScale = initialScale + new Vector3(scaleChange, scaleChange, scaleChange); // 모든 축에 변화 적용
    }
}
