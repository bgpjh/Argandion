using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmChange : MonoBehaviour
{
    private SystemManager _systemManager;
    [SerializeField] private GameObject _terrain1;
    [SerializeField] private GameObject _terrain2;
    [SerializeField] private GameObject _terrain3;

    void Start()
    {
        _systemManager = GameObject.Find("SystemManager").GetComponent<SystemManager>();
    }

    // 발전도 변하면 call하기
    public void ChangeFarm()
    {
        int devLevel = _systemManager.getDevelopLevel();

        if(devLevel==2){
            _terrain1.transform.GetChild(1).gameObject.SetActive(false);
            _terrain1.transform.GetChild(2).gameObject.SetActive(true);
            _terrain2.transform.GetChild(0).gameObject.SetActive(false);
            _terrain2.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).gameObject.SetActive(true);
        }
        else{
            _terrain2.transform.GetChild(1).gameObject.SetActive(false);
            _terrain2.transform.GetChild(2).gameObject.SetActive(true);
            _terrain3.transform.GetChild(0).gameObject.SetActive(false);
            _terrain3.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
        }
    }
}
