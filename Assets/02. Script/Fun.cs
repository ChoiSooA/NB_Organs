using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fun : MonoBehaviour
{
    [Header("3D �� ����")]
    public GameObject Skin;
    public RotateModel RotateSkin;

    [Header("������ ����")]
    public GameObject magnifier;
    public Button magCloseButton;
    private Vector3 magPosition;

    [Header("ȭ��ǥ ����")]
    public GameObject[] ArrowSign;
    public Material[] ArrowMaterial;    //0:red, 1:green

    [Header("UI ��ư ����")]
    public Button skinButton;
    public Sprite skinOn;
    private Sprite skinOff;
    public Button magButton;
    public Sprite magOn;
    private Sprite magOff;

    private void Start()
    {
        // �ʱ� ��ư ��������Ʈ ����
        skinOff = skinButton.GetComponent<Image>().sprite;
        magOff = magButton.GetComponent<Image>().sprite;

        // ��ư �̺�Ʈ ������ ���
        skinButton.onClick.AddListener(OnSkinButtonClick);
        magButton.onClick.AddListener(OnMagButtonClick);
        magCloseButton.onClick.AddListener(OnMagCloseButtonClick);

        // ������ �ʱ� ��ġ ���� �� ��Ȱ��ȭ
        magPosition = magnifier.transform.position;
        magnifier.SetActive(false);
    }

    private void OnSkinButtonClick()
    {
        // �����Ⱑ Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ
        if (magnifier.activeSelf)
        {
            magButton.GetComponent<Image>().sprite = magOff;
            magnifier.transform.position = magPosition;
            magnifier.SetActive(false);
        }

        // ��Ų Ȱ��ȭ ���� ���
        Skin.SetActive(!Skin.activeSelf);
        RotateSkin.enabled = true;

        // ������ �ݱ� ��ư�� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ
        if (magCloseButton.gameObject.activeSelf)
        {
            magCloseButton.gameObject.SetActive(false);
        }

        // ȭ��ǥ ���� ���� (���)
        UpdateArrowMaterials(1);

        // ��Ų ��ư ��������Ʈ ������Ʈ
        UpdateSkinButtonSprite();
    }

    private void OnMagButtonClick()
    {
        // ������ Ȱ��ȭ
        magnifier.SetActive(true);

        // ȭ��ǥ ���� ���� (������)
        UpdateArrowMaterials(0);

        // ��Ų Ȱ��ȭ �� ���� UI ����
        Skin.SetActive(true);
        magCloseButton.gameObject.SetActive(true);
        skinButton.GetComponent<Image>().sprite = skinOff;
        RotateSkin.enabled = false;

        // ������ ��ư ��������Ʈ ���� �� ��ġ ����
        magButton.GetComponent<Image>().sprite = magOn;
        magnifier.transform.position = magPosition;
    }

    public void OnMagCloseButtonClick()
    {
        // ������ ��Ȱ��ȭ
        magnifier.SetActive(false);

        // ��ư ��������Ʈ ������Ʈ
        magButton.GetComponent<Image>().sprite = magOff;

        // ȸ�� Ȱ��ȭ
        RotateSkin.enabled = true;

        // ȭ��ǥ ���� ���� (���)
        UpdateArrowMaterials(1);

        // �ݱ� ��ư ��Ȱ��ȭ
        magCloseButton.gameObject.SetActive(false);
    }

    // ȭ��ǥ ���� ������Ʈ �Լ�
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

    // ��Ų ��ư ��������Ʈ ������Ʈ �Լ�
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