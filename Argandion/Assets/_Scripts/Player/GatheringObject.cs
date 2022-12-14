using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringObject : MonoBehaviour
{
    private bool _isFlower;
    public int _itemCode;
    public float _delayedTimer;
    public float _movedDelay;
    private PlayerSystem _ps;
    public Inventory _inventory;
    public Item _item;
    public bool _isremain;
    private int _sectorNumber;
    public bool _isHave;
    public GameObject _fruit;

    void Start()
    {
        _ps = GameObject.Find("PlayerObject").GetComponent<PlayerSystem>();
        _inventory = GameObject.Find("UIManager").transform.GetChild(8).transform.GetChild(1).GetComponent<Inventory>();
        _item = GameObject.Find("ItemManager").GetComponent<Item>();
    }

    public void Interaction(float time)
    {
        if(_inventory.CheckInven(_item.FindItem(_itemCode),1))
        {
            if (_ps._equipList[_ps._equipItem,0] == 300)
            {
                _ps.damageStamina(1.4f);
            }
            else if (_ps._equipList[_ps._equipItem,0] == 305)
            {
                _ps.damageStamina(1.2f);
            }
            else if (_ps._equipList[_ps._equipItem,0] == 310)
            {
                _ps.damageStamina(0.9f);
            }
            else if (_ps._equipList[_ps._equipItem,0] == 315)
            {
                _ps.damageStamina(0.6f);
            }
            Vector3 Direction = (gameObject.transform.position - _ps.gameObject.transform.position);
            Direction.y = 0;
            Direction = Direction.normalized;
            _ps._character.forward = Direction;
            _ps._playerAnimator.SetInteger("action", 10);
            StartCoroutine(StopPlayer(time));
        }
    }

    IEnumerator StopPlayer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        gatheringItem();
    }

    private void gatheringItem()
        {
        _ps._playerAnimator.SetInteger("action", 0);
        _inventory.AcquireItem(_item.FindItem(_itemCode),1);
        if (_isFlower)
        {
            gameObject.transform.parent.GetComponent<SectorObject>()._flower_remain -= 1;
            _ps.damageStamina(2.2f);
        }
        if (!_isremain)
        {
            Destroy(this.gameObject);
        }
        else if(_isHave)
        {
            _inventory.AcquireItem(_item.FindItem(_itemCode),Random.Range(0,3));
            _isHave = false;
            _fruit.SetActive(false);
        }
    }

    public void setFlower(bool value)
    {
        _isFlower = value;
    }

    public void setHave(bool value)
    {
        _isHave = value;
        _fruit.SetActive(value);
    }

    public void setSector(int value)
    {
        _sectorNumber = value;
    }
}
