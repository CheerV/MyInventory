using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance = null;

    public Transform inventory;
    public Transform tips;
    public bool allowShowTips = true;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        tips.gameObject.SetActive(false);
        
	}

    void OnEnable()
    {
        sortItems();
    }
    /// <summary>
    /// 打开或关闭包裹界面
    /// </summary>
    public void SetInventoryActive()
    {
        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
    //sort items in grid
    public void sortItems() {
        GameObject[] items = GameObject.FindGameObjectsWithTag("UI_Item");
        GameObject[] grids = GameObject.FindGameObjectsWithTag("UI_ItemGrid");

        #region 
        //简单排序，顺序为从左至右从上到下 类似魔兽世界
        List<GameObject> gridList = new List<GameObject>();
        gridList.AddRange(grids);

        gridList.Sort(delegate(GameObject x, GameObject y)
        {
            Vector3 xp = x.transform.position;
            Vector3 yp = y.transform.position;
            if (xp.y == yp.y && xp.x == yp.x)
            {
                return 0;
            }
            if (xp.y == yp.y)
            {
                if (xp.x < yp.x)
                {
                    return -1;
                }
                return 1;
            }
            if (xp.x == yp.x)
            {
                if (xp.y > yp.y)
                {
                    return -1;
                }
                return 1;
            }
            if (xp.y > yp.y)
            {
                return -1;
            }
            return 1;
        });
        #endregion
        //物品与排列的格子呼应
        for (int i = 0; i < items.Length;i++ )
        {
            items[i].transform.position = gridList[i].transform.position;
            items[i].GetComponent<DragItem>().originalPosition = items[i].transform.position;
        }
    }
}
