using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fun : MonoBehaviour
{
    [Header("3D 모델 관련")]
    public GameObject Skin;
    public RotateModel RotateSkin;

    [Header("돋보기 관련")]
    public GameObject magnifier;
    public Button magCloseButton;
    private Vector3 magPosition;

    [Header("화살표 관련")]
    public GameObject[] ArrowSign;
    public Material[] ArrowMaterial;    //0:red, 1:green

    [Header("UI 버튼 관련")]
    public Button skinButton;
    public Sprite skinOn;
    private Sprite skinOff;
    public Button magButton;
    public Sprite magOn;
    private Sprite magOff;

    private void Start()
    {
        // 초기 버튼 스프라이트 저장
        skinOff = skinButton.GetComponent<Image>().sprite;
        magOff = magButton.GetComponent<Image>().sprite;

        // 버튼 이벤트 리스너 등록
        skinButton.onClick.AddListener(OnSkinButtonClick);
        magButton.onClick.AddListener(OnMagButtonClick);
        magCloseButton.onClick.AddListener(OnMagCloseButtonClick);

        // 돋보기 초기 위치 저장 및 비활성화
        magPosition = magnifier.transform.position;
        magnifier.SetActive(false);
    }

    private void OnSkinButtonClick()
    {
        // 돋보기가 활성화되어 있으면 비활성화
        if (magnifier.activeSelf)
        {
            magButton.GetComponent<Image>().sprite = magOff;
            magnifier.transform.position = magPosition;
            magnifier.SetActive(false);
        }

        // 스킨 활성화 상태 토글
        Skin.SetActive(!Skin.activeSelf);
        RotateSkin.enabled = true;

        // 돋보기 닫기 버튼이 활성화되어 있으면 비활성화
        if (magCloseButton.gameObject.activeSelf)
        {
            magCloseButton.gameObject.SetActive(false);
        }

        // 화살표 색상 변경 (녹색)
        UpdateArrowMaterials(1);

        // 스킨 버튼 스프라이트 업데이트
        UpdateSkinButtonSprite();
    }

    private void OnMagButtonClick()
    {
        // 돋보기 활성화
        magnifier.SetActive(true);

        // 화살표 색상 변경 (빨간색)
        UpdateArrowMaterials(0);

        // 스킨 활성화 및 관련 UI 설정
        Skin.SetActive(true);
        magCloseButton.gameObject.SetActive(true);
        skinButton.GetComponent<Image>().sprite = skinOff;
        RotateSkin.enabled = false;

        // 돋보기 버튼 스프라이트 변경 및 위치 설정
        magButton.GetComponent<Image>().sprite = magOn;
        magnifier.transform.position = magPosition;
    }

    public void OnMagCloseButtonClick()
    {
        // 돋보기 비활성화
        magnifier.SetActive(false);

        // 버튼 스프라이트 업데이트
        magButton.GetComponent<Image>().sprite = magOff;

        // 회전 활성화
        RotateSkin.enabled = true;

        // 화살표 색상 변경 (녹색)
        UpdateArrowMaterials(1);

        // 닫기 버튼 비활성화
        magCloseButton.gameObject.SetActive(false);
    }

    // 화살표 재질 업데이트 함수
    private void UpdateArrowMaterials(int materialIndex)
    {
        if (ArrowSign == null || ArrowMaterial == null || materialIndex >= ArrowMaterial.Length)
            return;

        for (int i = 0; i < ArrowSign.Length; i++)
        {
            if (ArrowSign[i] != null)
            {
                Renderer renderer = ArrowSign[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = ArrowMaterial[materialIndex];
                }
            }
        }
    }

    // 스킨 버튼 스프라이트 업데이트 함수
    private void UpdateSkinButtonSprite()
    {
        if (skinButton == null)
            return;

        Image buttonImage = skinButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = !Skin.activeSelf ? skinOn : skinOff;
        }
    }
}