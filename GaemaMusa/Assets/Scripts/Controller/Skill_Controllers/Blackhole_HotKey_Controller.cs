using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;                    // 스프라이트 렌더러
    private KeyCode myHotKey;                     // 핫키 코드
    private TextMeshProUGUI myText;               // 텍스트 컴포넌트

    private Transform myEnemy;                    // 적 참조
    private Blackhole_Skill_Controller blackHole; // 블랙홀 컨트롤러 참조

    // 핫키 설정 함수
    public void SetupHotKey(KeyCode _myHotKey,Transform _myEnemy, Blackhole_Skill_Controller _myBlackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackHole = _myBlackHole;

       
        myHotKey = _myHotKey;
        myText.text = _myHotKey.ToString();
    }

    private void Update()
    {
        // 설정된 핫키가 눌리면 블랙홀에 적 추가 및 핫키 숨기기
        if(Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }


}
