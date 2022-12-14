using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int idx;
    public ItemObject item;
    public int itemCount;
    public Image itemImage;
    public FoodManager foodManager;

    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    void Awake()
    {
        itemImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        text_Count = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public ItemObject getSlotItemData()
    {
        return item;
    }

    public int getSlotItemCount()
    {
        return itemCount;
    }

    public void setIdx(int slotIdx)
    {
        idx = slotIdx;
    }

    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    public void AddItem(ItemObject _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        LoadImage(item.ItemCode);

        if (item.Category != "장비" && _item.Category != "옷")
        {
            text_Count.text = itemCount.ToString();
            text_Count.gameObject.SetActive(true);
        }
        else
        {
            text_Count.text = "0";
            text_Count.gameObject.SetActive(false);
        }

        SetColor(1);
    }

    private void LoadImage(int idx)
    {
        Sprite iconImg = UIManager._uimanagerInstance.getItemIcon(idx);
        itemImage.sprite = iconImg;
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            if(!(gameObject.transform.parent.parent.parent.gameObject.name == "StorageScroll")) {
                ClearSlot();
            }
            itemCount = 0;
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        text_Count.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭 시, 상점이 열려있는 경우
        if (eventData.button == PointerEventData.InputButton.Left && UIManager._uimanagerInstance.getIsOpenTransaction() && itemCount != 0)
        {
            if (!(item.ItemCode >= 50 && item.ItemCode <= 56) && !(item.ItemCode >= 300 && item.ItemCode <= 304) && item.ItemCode != 320 && !(item.ItemCode >= 212 && item.ItemCode <= 219))
            {
                if (gameObject.transform.parent.parent.parent.name == "Bag")
                {
                    UIManager._uimanagerInstance.OnTradeModal(item.Name, item.ItemCode.ToString(), itemCount, item.SellCost, 0, 1, idx);
                }
                else if (gameObject.transform.parent.parent.parent.name == "QuickSlot")
                {
                    UIManager._uimanagerInstance.OnTradeModal(item.Name, item.ItemCode.ToString(), itemCount, item.SellCost, 0, 2, idx);
                }
            }
        }

        // 좌클릭 시, 창고가 열려있는 경우
        if (eventData.button == PointerEventData.InputButton.Left && UIManager._uimanagerInstance.getIsOpenStorage() && itemCount != 0)
        {
            if (gameObject.transform.parent.parent.parent.gameObject.name == "Bag")
            {
                UIManager._uimanagerInstance.OnTradeModal(item.Name, item.ItemCode.ToString(), itemCount, item.SellCost, 3, -1, idx);
            }
            else if (gameObject.transform.parent.parent.parent.gameObject.name == "StorageScroll")
            {
                UIManager._uimanagerInstance.OnTradeModal(item.Name, item.ItemCode.ToString(), itemCount, item.SellCost, 4, -1, idx);
            }
        }

        // 좌클릭 시, 인벤토리가 열려있는 경우
        if(eventData.button == PointerEventData.InputButton.Left && UIManager._uimanagerInstance.getIsOpenInventory()) {
            if(itemCount != 0 && item.Category == "옷") {
                if(gameObject.transform.parent.parent.parent.gameObject.name == "Bag") {
                    UIManager._uimanagerInstance.rightEquip(item, itemCount, idx);
                } else if(gameObject.transform.parent.name == "Equipment") {
                    UIManager._uimanagerInstance.rightDismiss(item, itemCount, idx);
                }
                return;
            }

            if(UIManager._uimanagerInstance.getIsLeftClickInInven()) {
                // Debug.Log("두 번째 클릭");
                if(gameObject.transform.parent.parent.parent.gameObject.name == "QuickSlot") {
                    if(itemCount == 0) {
                        UIManager._uimanagerInstance.setSecondClick(2, idx, 0, 0);
                        return;
                    }
                    UIManager._uimanagerInstance.setSecondClick(2, idx, item.ItemCode, itemCount);
                } else if(gameObject.transform.parent.parent.parent.gameObject.name == "Bag") {
                    if(itemCount == 0) {
                        UIManager._uimanagerInstance.setSecondClick(1, idx, 0, 0);
                        return;
                    }
                    UIManager._uimanagerInstance.setSecondClick(1, idx, item.ItemCode, itemCount);
                }
            } else {
                // Debug.Log("첫 번째 클릭");
                if(gameObject.transform.parent.parent.parent.gameObject.name == "QuickSlot") {
                    if(itemCount == 0) {
                        UIManager._uimanagerInstance.setFirstClick(2, idx, 0, 0);
                        return;
                    }
                    UIManager._uimanagerInstance.setFirstClick(2, idx, item.ItemCode, itemCount);
                } else if(gameObject.transform.parent.parent.parent.gameObject.name == "Bag") {
                    if(itemCount == 0) {
                        UIManager._uimanagerInstance.setFirstClick(1, idx, 0, 0);
                        return;
                    }
                    UIManager._uimanagerInstance.setFirstClick(1, idx, item.ItemCode, itemCount);
                }
            }
        }

        // 우클릭 시, 인벤토리가 열려있는 경우
        if (eventData.button == PointerEventData.InputButton.Right && UIManager._uimanagerInstance.getIsOpenInventory() && itemCount != 0)
        {
            // Debug.LogWarning("인벤토리 우클릭");
            if (gameObject.transform.parent.parent.parent.gameObject.name == "QuickSlot" || gameObject.transform.parent.name == "Equipment")
            {
                // Debug.Log("여긴 퀵 슬롯 또는 장비 슬롯");
                UIManager._uimanagerInstance.clickRightSlotModal(4, Input.mousePosition - new Vector3(2, -2, 0), item, itemCount, idx);
            }
            else if (gameObject.transform.parent.parent.parent.gameObject.name == "Bag")
            {
                // Debug.Log("여긴 인벤토리");
                switch (item.Category)
                {
                    case "옷":
                    case "기타":
                        if (item.Name != "양털")
                        {
                            UIManager._uimanagerInstance.clickRightSlotModal(1, Input.mousePosition - new Vector3(2, -2, 0), item, itemCount, idx);
                        }
                        break;
                    case "장비":
                    case "꽃":
                    case "씨앗":
                        UIManager._uimanagerInstance.clickRightSlotModal(2, Input.mousePosition - new Vector3(2, -2, 0), item, itemCount, idx);
                        break;
                    case "식량":
                        if (item.ItemCode < 103 || (item.ItemCode > 106 && item.ItemCode < 111) || item.ItemCode > 113)
                        {
                            UIManager._uimanagerInstance.clickRightSlotModal(2, Input.mousePosition - new Vector3(2, -2, 0), item, itemCount, idx);
                            UIManager._uimanagerInstance.clickRightSlotModal(3, Input.mousePosition - new Vector3(2, -2, 0), item, itemCount, idx);
                        }
                        break;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemCount != 0)
        {
            if(UIManager._uimanagerInstance.getIsOpenTransaction()) {
                Vector3 vec3 = Input.mousePosition - new Vector3(2, -5, 0);
                string _text = item.Name;
                if (item.SellCost == -1)
                {
                    _text += "\n판매불가 아이템";
                }
                else
                {
                    _text += "\n가격 : " + item.SellCost;
                }
                UIManager._uimanagerInstance.onSlotOverModal(_text, vec3);
            } else {
                Vector3 vec3 = Input.mousePosition - new Vector3(2, -5, 0);
                string _text = item.Name + "\n" + item.Desc;
                if(item.Category == "옷") {
                    _text += "\n[우클릭 사용]";
                }
                UIManager._uimanagerInstance.onSlotOverModal(_text, vec3);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager._uimanagerInstance.offSlotOverModal();
    }
}
