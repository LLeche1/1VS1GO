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

    public void Reset()
    {
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
