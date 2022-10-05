using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    [SerializeField]
    private RectTransform rectTr;
    [SerializeField, Range(10, 150)]
    float leverRange;

    public GameObject player;

    private Vector2 inputDir;
    private bool isInput;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBegginDrag");
        var inputPos = eventData.position - rectTr.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        var inputPos = eventData.position - rectTr.anchoredPosition;
        var inputvetor = inputPos.magnitude < leverRange ? inputPos : leverRange * inputPos.normalized;
        lever.anchoredPosition = inputvetor;
        inputDir = inputvetor / leverRange;
        isInput = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        isInput = false;
        lever.anchoredPosition = Vector2.zero;
        inputDir = Vector2.zero;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInput)
        {
            //Debug.Log(inputDir.x + " / " + inputDir.y);
            player.transform.Translate(new Vector3(inputDir.x, 0, inputDir.y) * 10 * Time.deltaTime, Space.World);
            player.transform.LookAt(player.transform.position + new Vector3(inputDir.x, 0, inputDir.y));
        }
    }
}
