using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fun : MonoBehaviour
{
    public GameObject Skin;
    public RotateModel RotateSkin;
    public GameObject magnifier;

    public Button skinButton;
    public Sprite skinOn;
    Sprite skinOff;
    public Button magButton;
    public Sprite magOn;
    Sprite magOff;
    public Button magResetButton;

    Vector3 magPosition;

    private void Start()
    {
        skinOff=skinButton.GetComponent<Image>().sprite;
        magOff = magButton.GetComponent<Image>().sprite;
        skinButton.onClick.AddListener(OnSkinButtonClick);
        magButton.onClick.AddListener(OnMagButtonClick);
        magResetButton.onClick.AddListener(OnMagResetButtonClick);
        magPosition = magnifier.transform.position;
        magnifier.SetActive(false);
        magResetButton.gameObject.SetActive(false);
    }
    private void OnSkinButtonClick()
    {
        Skin.SetActive(!Skin.activeSelf);
        RotateSkin.enabled = true;
        if (magnifier.activeSelf)
        {
            magButton.GetComponent<Image>().sprite = magOff;
            magnifier.transform.position = magPosition;
            magnifier.SetActive(false);
        }
        if (!Skin.activeSelf)
        {
            skinButton.GetComponent<Image>().sprite = skinOn;
        }
        else
        {
            skinButton.GetComponent<Image>().sprite = skinOff;
        }
    }
    private void OnMagButtonClick()
    {
        magnifier.SetActive(true);
        Skin.SetActive(true);
        skinButton.GetComponent<Image>().sprite = skinOff;
        RotateSkin.enabled = false;
        if (magnifier.activeSelf)
        {
            magButton.GetComponent<Image>().sprite = magOn;
            OnMagResetButtonClick();
        }
    }
    private void OnMagResetButtonClick()
    {
        magnifier.transform.position = magPosition;
    }
}
