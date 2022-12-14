using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateCharacter : MonoBehaviour
{
    [SerializeField] private Image manImg;
    [SerializeField] private Image womanImg;
    public Sprite unselectedMan;
    public Sprite selectedMan;
    public Sprite unselectedWoman;
    public Sprite selectedWoman;

    private string _name;


    public void selectMan()
    {
        manImg.sprite = selectedMan;
        womanImg.sprite = unselectedWoman;
        gameObject.GetComponentInParent<UIManager>().setCharacterValue(1);
    }

    public void selectWoman()
    {
        manImg.sprite = unselectedMan;
        womanImg.sprite = selectedWoman;
        gameObject.GetComponentInParent<UIManager>().setCharacterValue(0);
    }

    public void backToMain()
    {
        gameObject.SetActive(false);
        gameObject.GetComponentInParent<UIManager>().OnMainPagePanel();
        gameObject.transform.GetChild(2).GetComponent<TMP_InputField>().text = "";
    }

    public void gameStart()
    {
        getName();

        if (_name == "")
        {
            UIManager._uimanagerInstance.OnResultNotificationPanel("이름은 공백일 수 없습니다-");
        }else if (UIManager._uimanagerInstance.getCharacterValue() == -1)
        {
            UIManager._uimanagerInstance.OnResultNotificationPanel("캐릭터를 선택해 주십시오-");
        }
        else
        {
            UIManager._uimanagerInstance.startTime();
            UIManager._uimanagerInstance.OnBaseUIPanel();
            UIManager._uimanagerInstance.selectPlayer();
            UIManager._uimanagerInstance.setPlayerName(_name);
            UIManager._uimanagerInstance.setGameState(true);

            gameObject.SetActive(false);

            UIManager._uimanagerInstance.delayRunControllKeys();
            UIManager._uimanagerInstance.startInvenSet();
        }
    }

    void Start()
    {
        manImg = transform.GetChild(5).GetChild(0).GetComponent<Image>();
        womanImg = transform.GetChild(6).GetChild(0).GetComponent<Image>();
    }

    public void getName()
    {
        _name = gameObject.transform.GetChild(2).GetComponent<TMP_InputField>().text;
    }

    public void clickInputField()
    {
        gameObject.transform.GetChild(2).GetComponent<TMP_InputField>().text = "";
    }
}
