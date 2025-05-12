using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using LeiaUnity.Examples;

public class ZoomClick : MonoBehaviour
{
    public GameObject[] SpaceObjects;
    public GameObject infoPannel;
    public TMP_Text Text_Info_Title;
    public TMP_Text Text_Info_Content;
    public Button closeButton;
    public Button detailButton;
    public Button detailCloseButton;
    public GameObject SkinBody;
    public Transform detailPos;

    public GameObject GlowObject; // (선택) 수동 할당용

    float moveSpeed = 0.5f;

    Vector3 originalPos;
    Vector3 originalScale;
    Vector3 originalRotation;

    bool isZoomNow = false;
    GameObject currentGlow;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            closeButton.transform.parent.GetComponent<DoTEffect>().Close();
            ZoomOut();
        });

        detailButton.onClick.AddListener(() =>
        {
            detailButton.transform.parent.GetComponent<DoTEffect>().Close();
            zoomDetail();
        });

        detailCloseButton.onClick.AddListener(() =>
        {
            ZoomOut();
            detailCloseButton.gameObject.SetActive(false);
        });

        detailCloseButton.gameObject.SetActive(false);
    }

    [System.Serializable]
    public struct SpaceObjectInfo
    {
        public string objectName;
        public string title;
        [TextArea(2, 5)]
        public string content;
        public AudioClip audioClip;
        public Transform centerPos;
    }

    public SpaceObjectInfo[] spaceObjectInfos;

    public void Zoom()
    {
        if (isZoomNow) return;
        isZoomNow = true;

        GameObject zoomObj = TouchObjectDetector.selectedObject;
        if (zoomObj == null)
        {
            isZoomNow = false;
            return;
        }

        originalPos = zoomObj.transform.position;
        originalScale = zoomObj.transform.localScale;
        originalRotation = zoomObj.transform.rotation.eulerAngles;

        Transform targetCenter = GetCenterPosByName(zoomObj.name);
        SetInfoByName(zoomObj.name);
        infoPannel.SetActive(true);

        zoomObj.GetComponent<Collider>().enabled = false;
        zoomObj.transform.DOLocalMove(targetCenter.position, moveSpeed);

        foreach (GameObject obj in SpaceObjects)
        {
            obj.GetComponent<Collider>().enabled = false;
            if (obj != zoomObj) obj.SetActive(false);
        }

        // Glow 켜기
        currentGlow = GetGlowObject(zoomObj);
        if (currentGlow != null)
            currentGlow.SetActive(true);

        StartCoroutine(ZoomCoroutine(zoomObj));
    }

    public void zoomDetail()
    {
        SkinBody.SetActive(false);
        detailCloseButton.gameObject.SetActive(true);

        GameObject zoomObj = TouchObjectDetector.selectedObject;
        if (zoomObj == null) return;

        // Glow 끄기
        if (currentGlow != null)
            currentGlow.SetActive(false);

        zoomObj.transform.DOMove(detailPos.position, moveSpeed);
        zoomObj.transform.DOScale(new Vector3(6f, 6f, 6f), moveSpeed);

        var viewer = zoomObj.GetComponent<ModelViewerControls>();
        if (viewer != null)
            viewer.enabled = true;
    }

    public void ZoomOut()
    {
        if (!isZoomNow) return;

        GameObject zoomObj = TouchObjectDetector.selectedObject;
        if (zoomObj == null)
        {
            isZoomNow = false;
            return;
        }

        if (!SkinBody.activeSelf)
            SkinBody.SetActive(true);

        zoomObj.transform.DOMove(originalPos, moveSpeed);
        zoomObj.transform.DORotate(originalRotation, moveSpeed);
        zoomObj.transform.DOScale(originalScale, moveSpeed);

        foreach (GameObject obj in SpaceObjects)
            obj.SetActive(true);

        // Glow 끄기
        if (currentGlow != null)
            currentGlow.SetActive(false);

        var viewer = zoomObj.GetComponent<ModelViewerControls>();
        if (viewer != null)
            viewer.enabled = false;

        StartCoroutine(BackCoroutine(zoomObj));
    }

    GameObject GetGlowObject(GameObject target)
    {
        // 직접 할당된 GlowObject가 있으면 그것 사용
        if (GlowObject != null)
            return GlowObject;

        // 없으면 자식 1번 오브젝트 사용
        if (target.transform.childCount > 1)
            return target.transform.GetChild(1).gameObject;

        return null;
    }

    void SetInfoByName(string objName)
    {
        foreach (var info in spaceObjectInfos)
        {
            if (info.objectName == objName)
            {
                Text_Info_Title.text = info.title;
                Text_Info_Content.text = info.content;
                if (info.audioClip != null)
                    Audio_Manager.Instance.PlayMent(info.audioClip);
                return;
            }
        }
    }

    Transform GetCenterPosByName(string objName)
    {
        foreach (var info in spaceObjectInfos)
        {
            if (info.objectName == objName)
                return info.centerPos;
        }

        Debug.LogWarning($"[ZoomClick] Center position not found for: {objName}");
        return this.transform;
    }

    IEnumerator ZoomCoroutine(GameObject zoomObj)
    {
        yield return new WaitForSeconds(moveSpeed);
    }

    IEnumerator BackCoroutine(GameObject zoomObj)
    {
        yield return new WaitForSeconds(moveSpeed);

        foreach (GameObject obj in SpaceObjects)
            obj.GetComponent<Collider>().enabled = true;

        zoomObj.GetComponent<Collider>().enabled = true;
        isZoomNow = false;
    }
}
