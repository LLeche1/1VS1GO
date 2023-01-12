using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform lever;
    [Range(10, 150)]
    public float leverRange;
    public Vector2 inputDir;
    RectTransform rectTransform;
    LobbyManager lobbyManager;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }

    void Update()
    {
        if(lever.anchoredPosition.x > 0 && lever.anchoredPosition.x < 150 && lever.anchoredPosition.y < 150 && lever.anchoredPosition.y > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (lever.anchoredPosition.x > 0 && lever.anchoredPosition.x < 150 && lever.anchoredPosition.y < 0 && lever.anchoredPosition.y > -150)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (lever.anchoredPosition.x > -150 && lever.anchoredPosition.x < 0 && lever.anchoredPosition.y < 0 && lever.anchoredPosition.y > -150)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (lever.anchoredPosition.x > -150 && lever.anchoredPosition.x < 0 && lever.anchoredPosition.y < 150 && lever.anchoredPosition.y > 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        lever.anchoredPosition = Vector2.zero;
        inputDir = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Reset();
    }
}
