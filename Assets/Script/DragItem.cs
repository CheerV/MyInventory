using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler {
    private Transform myTransform;
    private RectTransform myRectTransform;

    //用于event trigger对自身检测的开关
    private CanvasGroup canvasGroup;
    //拖拽前的有效位置，拖到有效位置更新
    public Vector3 originalPosition;
    //记录上一帧所在物品格子
    private GameObject lastEnter = null;
    //记录上一帧所在物品格子的正常颜色
    private Color lastEnterNormalColor;
    //拖拽至新的物品格子的高亮颜色
    private Color highLightColor = Color.cyan;

	// Use this for initialization
	void Start () {
        myTransform = this.transform;
        myRectTransform = this.transform as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = myTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBeginDrag(PointerEventData eventData) {
        //让EventTrigger忽略自身，这样才可以让EventTrigger检测到下一层的触摸事件
        canvasGroup.blocksRaycasts = false;
        lastEnter = eventData.pointerEnter;
        lastEnterNormalColor = lastEnter.GetComponent<Image>().color;

        originalPosition = myTransform.position;
        //层级置于最下方 保证可以优先渲染
        gameObject.transform.SetAsLastSibling();
        //拖拽物品时禁用悬停显示信息
        UIManager.instance.allowShowTips = false;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 globalMousePos;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(myRectTransform, eventData.position, 
            eventData.pressEventCamera, out globalMousePos)) {
                myRectTransform.position = globalMousePos;
        }

        GameObject curEnter = eventData.pointerEnter;

        bool inItemGrid = EnterItemGrid(curEnter);
        if(inItemGrid) {
            Image img = curEnter.GetComponent<Image>();

            if(lastEnter != curEnter) {
                lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
                //记录当前物品格子以供下一帧调用
                lastEnter = curEnter;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        GameObject curEnter = eventData.pointerEnter;

        //拖拽到无效位置，恢复原位
        if (curEnter == null)
        {
            myTransform.position = originalPosition;
        }
        else { 
            //移动至物品格子上
            if (curEnter.name == "UI_ItemGrid")
            {
                myTransform.position = curEnter.transform.position;
                originalPosition = myTransform.position;
                //当前格子恢复正常颜色
                curEnter.GetComponent<Image>().color = lastEnterNormalColor;
            }
            else
            {
                //移动到包裹上的其他物品上
                if (curEnter.name == eventData.pointerDrag.name && curEnter != eventData.pointerDrag)
                {
                    Vector3 targetPosition = curEnter.transform.position;
                    curEnter.transform.position = originalPosition;
                    myTransform.position = targetPosition;
                    originalPosition = myTransform.position;
                }
                else//拖拽到别的物体上 
                {
                    myTransform.position = originalPosition;
                }
            }
        }

        lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
        canvasGroup.blocksRaycasts = true;
        UIManager.instance.allowShowTips = true;
    }

    bool EnterItemGrid(GameObject go) { 
        if(go == null) {
            return false;
        }

        return go.name == "UI_ItemGrid";
    }

    public void OnPointerEnter(PointerEventData eventData) { 
        if(UIManager.instance.allowShowTips) {
            //鼠标悬在同一物体上，则已经显示的tips保持原样，避免闪烁
            if(eventData.lastPress == eventData.pointerEnter) {
                return;
            }

            //显示提示信息
            if(!UIManager.instance.tips.gameObject.activeSelf) {
                UIManager.instance.tips.transform.position = myTransform.position + new Vector3(myRectTransform.rect.width * 0.5f, 0, 0);
                UIManager.instance.tips.gameObject.SetActive(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        UIManager.instance.tips.gameObject.SetActive(false);
    }
}
