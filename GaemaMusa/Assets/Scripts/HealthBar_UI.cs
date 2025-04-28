using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity; // Entity 컴포넌트 참조
    private CharacterStats myStats; // CharacterStats 컴포넌트 참조
    private RectTransform myTransform; // RectTransform 컴포넌트 참조
    private Slider slider; // 슬라이더 컴포넌트 참조
    private void Start()
    {
        myTransform = GetComponent<RectTransform>(); // RectTransform 컴포넌트 가져오기
        
        entity = GetComponentInParent<Entity>(); // 부모 오브젝트에서 Entity 컴포넌트 가져오기
        myStats = GetComponentInParent<CharacterStats>(); // 부모 오브젝트에서 CharacterStats 컴포넌트 가져오기
        slider = GetComponentInChildren<Slider>(); // 슬라이더 컴포넌트 가져오기
        entity.onFlipped += FlipUI; // Entity의 onFlipped 델리게이트에 FlipUI 메서드 등록
        myStats.onHealthChanged += UpdateHealthUI; // 체력 변경 시 UI 업데이트

        UpdateHealthUI(); // 초기 체력 UI 업데이트

        

        Debug.Log("헬스바불르고있다.");
    }

   

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();  // 최대 체력 설정
        slider.value = myStats.currentHealth; // 현재 체력 설정

    }


 // UI를 180도 회전시켜서 반대 방향으로 표시
    public void FlipUI() => myTransform.Rotate(0,180,0);
    
   
 // Entity의 onFlipped 델리게이트에서 FlipUI 메서드 등록 해제
    private void OnDisable()
    {
        entity.onFlipped -= FlipUI; 
        myStats.onHealthChanged -= UpdateHealthUI; // 체력 변경 시 UI 업데이트 해제
    } 
   
    




}
