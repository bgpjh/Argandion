using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [SerializeField] private bool _time_stop;
    public int _month;
    public int _day;
    public int _season;
    public int _hour_display;
    public int _minute_display;
    public int _hour;
    public int _weather;
    private float _minute;
    private bool _game_state = false;
    private bool idx4 = false;
    public float _hour_time_changemeter = 1000;
    private int _player_gold;
    public GameObject _light;
    public GameObject _MapObject;
    public PlayerSystem _player;
    public HouseChange _houseChange;
    public HouseChange _interiorChange;
    public FarmChange _farmChange;
    public WeatherManager _weatherManager;
    public BuffManager _buffManager;
    public NPCManager _NPCManager;
    public UIManager _UIManager;
    public SoundManager _soundManager;
    public EventPanel _EventPanel;
    public PrayBuff _PrayBuff;
    public SpiritBuff _SpiritBuff;
    public int _development_level;  // 1부터
    public int _purification_sector = 0;
    static int _sector_size = 8;
    public int _purification_size;
    public bool[] _purification = new bool[_sector_size];
    public WorldTree _worldTree;
    public Altar _altar;
    [SerializeField] private Dirt[] _dirts;

    public SectorObject _sectorTest;
    [SerializeField] private SectorObject[] _sectors;
    [SerializeField] private BuildingChange[] _buildings;
    [SerializeField] private BuildingChange _buiding1;
    public GameObject[] _randomNPC = new GameObject[2];

    [SerializeField] private Material[] _SkyBoxMat;
    [SerializeField] private bool _isnight;
    [SerializeField] private GameObject loadingStartPage;
    [SerializeField] private GameObject loadingEndPage;
    [SerializeField] private RectTransform loadingBar;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private TeleportationHome tphome;
    [SerializeField] private SaveSystem _save;
    private bool _inloading;

    public int[,] _timezone = new int[,] { { 6, 7, 18, 19 }, { 6, 6, 19, 20 }, { 6, 7, 18, 19 }, { 7, 8, 18, 19 } };

    void Start()
    {
        _month = 1;
        _day = 1;
        _season = 0;
        _hour = 6;
        _hour_display = 6;
        _minute = 0;
        _minute_display = 0;
        _player = GameObject.Find("PlayerObject").GetComponent<PlayerSystem>();
        _MapObject = GameObject.Find("Map");
        _sectors = _MapObject.GetComponentsInChildren<SectorObject>();
        _weatherManager = GameObject.Find("WeatherManager").GetComponent<WeatherManager>();
        _buffManager = GameObject.Find("BuffManager").GetComponent<BuffManager>();
        _NPCManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();
        _UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _PrayBuff = GameObject.Find("BuffManager").GetComponent<PrayBuff>();
        _SpiritBuff = GameObject.Find("BuffManager").GetComponent<SpiritBuff>();
        _worldTree = GameObject.Find("WorldTree").GetComponent<WorldTree>();
        _buildings = GameObject.Find("Buildings").GetComponentsInChildren<BuildingChange>();
        _houseChange = GameObject.Find("Player House").GetComponent<HouseChange>();
        _interiorChange = GameObject.Find("Interior").GetComponent<HouseChange>();
        _farmChange = GameObject.Find("Map").transform.GetChild(5).transform.GetChild(0).transform.GetChild(2).GetComponent<FarmChange>();
        _altar = GameObject.Find("Altar").GetComponent<Altar>();
        _MapObject.GetComponent<MapObject>().UpdateFieldManager(_season);
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _save = gameObject.GetComponent<SaveSystem>();

        Invoke("LoadGame", 2.0f);
    }

    void Update()
    {
        TimeSystem();
        if (_inloading)
        {
            LoadingBar();
        }
    }

    public void UpdateSeason(int index)
    {
        _season = index;
        _MapObject.GetComponent<MapObject>().UpdateFieldManager(index);
        _worldTree.ChangeSeason();
    }

    //건물이 모두 지어지면 호출
    public void UpdatePurification(int index)  //1번부터 
    {
        switch (index)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                _purification[index - 1] = true;
                _MapObject.GetComponent<MapObject>().ChangePurifier(index - 1);
                _NPCManager.NPCActive(index - 1);
                break;
            case 6:
            case 7:
                _purification[index - 1] = true;
                _MapObject.GetComponent<MapObject>().ChangePurifier(index - 1);
                _MapObject.GetComponent<MapObject>().ChangePurifier(7);
                _NPCManager.NPCActive(index - 2);
                break;

        }

        if (index == 4 && !idx4)
        {
            idx4 = true;
            _purification_sector -= 1;
        }

        _purification[index] = true;
        _purification_sector += 1;

        if (_purification_sector > 2 && _development_level < 2)
        {
            _development_level = 2;
            DevelopLevelUp();
        }
        if (_purification_sector > 4 && _development_level < 3)
        {
            _development_level = 3;
            DevelopLevelUp();
        }
    }

    public void setTimeSystem(bool value)
    {
        _time_stop = value;
    }

    public bool getTimeSystem()
    {
        return _time_stop;
    }

    private void TimeSystem()
    {
        if (!_time_stop)
        {
            _minute += Time.deltaTime * (_hour_time_changemeter / 1000f);
            if (_minute >= 60)
            {
                _minute -= 60;
                _hour += 1;

                if (_hour == 21)
                {
                    animalDestroy();
                }

                if (_hour >= ((_buffManager.whitePray || _buffManager.whiteSpirit) ? 25 : 23))
                {
                    _hour = ((_buffManager.whitePray || _buffManager.whiteSpirit) ? 5 : 6);
                    _day += 1;
                    InLoading();

                    if (_day >= 29)
                    {
                        _day -= 28;
                        _month += 1;

                        if (_month >= 9)
                        {
                            _month -= 8;
                        }
                        if (_month % 2 == 1)
                        {
                            UpdateSeason(_month / 2);
                        }
                    }
                    Invoke("TimeCall",1.5f);
                }
            }

            _minute_display = ((int)_minute);
            _hour_display = _hour;

            if (_hour < _timezone[_season, 0])
            {
                _light.transform.rotation = Quaternion.Euler(-10, -30, _light.transform.rotation.z);
            }
            else if (_timezone[_season, 0] <= _hour && _hour < _timezone[_season, 1])
            {
                _light.transform.rotation = Quaternion.Euler(-10 + _minute, -30, _light.transform.rotation.z);
            }
            else if (_timezone[_season, 1] <= _hour && _hour < _timezone[_season, 2])
            {
                _light.transform.rotation = Quaternion.Euler(50, -30, _light.transform.rotation.z);
            }
            else if (_timezone[_season, 2] <= _hour && _hour < _timezone[_season, 3])
            {
                _light.transform.rotation = Quaternion.Euler(50 + _minute * 2.5f, -30, _light.transform.rotation.z);
            }
            else if (_timezone[_season, 3] <= _hour)
            {
                _light.transform.rotation = Quaternion.Euler(200, -30, _light.transform.rotation.z);
            }

            if (_timezone[_season, 0] <= _hour && _isnight)
            {
                RenderSettings.skybox = _SkyBoxMat[0];
                RenderSettings.skybox.SetFloat("_Rotation", 215f);
                _isnight = false;
            }
            if (_timezone[_season, 2] <= _hour && !_isnight)
            {
                RenderSettings.skybox = _SkyBoxMat[1];
                _isnight = true;
            }
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.5f);
        }
    }

    private void TimeCall()
    {
        DayEnd();
        _save.Save(_save._savedata);
        DayStart();
        Invoke("OutLoading",5f);
    }

    public void DayEnd()
    {
        if (1 <= _weather && _weather <= 9)
        {
            _EventPanel.inactiveIcon(_weather + 49);
        }
        _weatherManager.SetWeather(_season);
        if (1 <= _weather && _weather <= 9)
        {
            _EventPanel.activeIcon(_weather + 49);
        }
        // 모든 SectorObject의 DayEnd 동작
        foreach (var sector in _sectors)
        {
            sector.DayEnd();
        }
        _altar.DayEnd();

        //순례자, 음악가 랜덤위치 생성
        int npc1_position = RandomPurification();
        int npc2_position = RandomPurification();

        // 필드의 동물, 떨어진 아이템, 매일 새로 생성되는 자원 제거
        GameObject[] rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
        GameObject[] deers = GameObject.FindGameObjectsWithTag("Deer");
        GameObject[] wolfs = GameObject.FindGameObjectsWithTag("Wolf");
        GameObject[] bears = GameObject.FindGameObjectsWithTag("Bear");
        GameObject[] items = GameObject.FindGameObjectsWithTag("droppedItem");
        GameObject[] props = GameObject.FindGameObjectsWithTag("resource");
        GameObject[] ores = GameObject.FindGameObjectsWithTag("Ore");
        GameObject[] berrys = GameObject.FindGameObjectsWithTag("berry");

        foreach (var r in rabbits)
        {
            Destroy(r);
        }
        foreach (var d in deers)
        {
            Destroy(d);
        }
        foreach (var w in wolfs)
        {
            Destroy(w);
        }
        foreach (var b in bears)
        {
            Destroy(b);
        }
        foreach (var i in items)
        {
            Destroy(i);
        }
        foreach (var p in props)
        {
            Destroy(p);
        }
        foreach (var o in ores)
        {
            Destroy(o);
        }
        foreach (var b in berrys)
        {
            b.GetComponent<GatheringObject>().setHave(true);
        }
        _buffManager.DayEnd();
    }

    private void animalDestroy()
    {
        Transform animals = GameObject.Find("Animals").transform;

        foreach (Transform animal in animals)
        {
            //플레이어랑 거리 계산해서 멀리있는 동물들만 삭제
            float distance = Vector3.Distance(animal.position, _player.transform.position);
            if (distance >= 50f)
            {
                Destroy(animal.gameObject);
            }
        }
    }

    public void DayStart()
    {
        // 모든 SectorObject의 DayEnd 동작
        foreach (var sector in _sectors)
        {
            sector.DayStart();
        }
        _UIManager.DayStart();
        _PrayBuff.DayStart();
        foreach (var dirt in _dirts)
        {
            if (dirt.isActiveAndEnabled)
            {
                dirt.DayStart();
            }
        }
        foreach (var building in _buildings)
        {
            if(building)
            {
            building.DayStart();
            }
        }
    }

    public void LoadWeather()
    {
        if (1 <= _weather && _weather <= 9)
        {
            _EventPanel.activeIcon(_weather + 49);
        }
        _weatherManager.PlayFXWeather(_weather);
        _EventPanel._foodPanel.SetActive(true);
        _EventPanel._otherPanel.SetActive(true);
    }

    //정화된 구역 중에서 랜덤 한 구역 정하기
    private int RandomPurification()
    {
        int[] region = new int[8];  //정화된 지역
        _purification_size = 0;

        for (int i = 0; i < 8; i++)
        {
            if (_purification[i])
            {
                region[_purification_size] = i;
                _purification_size++;
            }
        }

        int rand = Random.Range(0, _purification_size);

        return region[rand];
    }

    //선택된 지역중 랜덤 좌표
    private Vector3 NPCRandomPosition(int region)
    {
        Vector3 position = new Vector3(0, 0, 0);

        switch (region)
        {
            case 0:
                position = new Vector3(Random.Range(0.0f, 60.0f), 1.9f, Random.Range(225.0f, 275.0f));
                break;
            case 1:
                position = new Vector3(Random.Range(87.0f, 202.0f), 1.9f, Random.Range(193.0f, 264.0f));
                break;
            case 2:
                position = new Vector3(Random.Range(193.0f, 233.0f), 1.9f, Random.Range(200.0f, 256.0f));
                break;
            case 3:
                position = new Vector3(Random.Range(33.0f, 79.0f), 1.9f, Random.Range(110.0f, 162.0f));
                break;
            case 4:
                position = new Vector3(Random.Range(80.0f, 192.0f), 1.9f, Random.Range(110.0f, 200.0f));
                break;
            case 5:
                position = new Vector3(Random.Range(193.0f, 251.0f), 1.9f, Random.Range(120.0f, 200.0f));
                break;
            case 6:
                position = new Vector3(Random.Range(0.0f, 166.0f), 1.9f, Random.Range(38.0f, 110.0f));
                break;
            case 7:
                position = new Vector3(Random.Range(180.0f, 194.0f), 1.9f, Random.Range(81.0f, 110.0f));
                break;
        }
        return position;
    }

    public void setGameState(bool value)
    {
        _game_state = value;
    }

    public bool getGameState()
    {
        return _game_state;
    }

    public void setPlayerGold(int value)
    {
        _player_gold = value;
    }

    public int getPlayerGold()
    {
        return _player_gold;
    }

    public void addPlayerGold(int value)
    {
        _player_gold += value;
    }

    public void callActiveIcon(int value)
    {
        _EventPanel.activeIcon(value);
    }

    public void callInactiveIcon(int value)
    {
        _EventPanel.inactiveIcon(value);
    }

    public void CompleteConstruction(int index)
    {
        // 이자리에 특정 건물들 call 함수 입력
    }

    public void DevelopLevelUp()
    {
        if (_development_level == 2)
        {
            //실행문
            // npc 부르기
            _NPCManager.NPCActive(6);
            // 밭 활성화
            _farmChange.ChangeFarm();
            // 집 자라기
            _houseChange.ChangeHouse();
            _interiorChange.ChangeHouse();
            _worldTree.ChangeTreeLevel();
            // 제단 텔레포트 active
            GameObject.Find("Teleport").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Teleport").transform.GetChild(1).gameObject.SetActive(true);
        }
        if (_development_level == 3)
        {
            //실행문
            // npc 부르기
            _NPCManager.NPCActive(7);
            // 밭 활성화
            _farmChange.ChangeFarm();
            // 집 자라기
            _houseChange.ChangeHouse();
            _interiorChange.ChangeHouse();
            _worldTree.ChangeTreeLevel();
        }
    }
    
    public void LoadDevelopLevel(int level){
        if(level == 1){
            return;
        }else if(level >= 2){
            //실행문
            // npc 부르기
            _NPCManager.NPCActive(6);
            // 밭 활성화
            _farmChange.ChangeFarm();
            // 집 자라기
            _houseChange.ChangeHouse();
            _interiorChange.ChangeHouse();
            _worldTree.ChangeTreeLevel();
            // 제단 텔레포트 active
            GameObject.Find("Teleport").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Teleport").transform.GetChild(1).gameObject.SetActive(true);
        }else if(level == 3){
            //실행문
            // npc 부르기
            _NPCManager.NPCActive(7);
            // 밭 활성화
            _farmChange.ChangeFarm();
            // 집 자라기
            _houseChange.ChangeHouse();
            _interiorChange.ChangeHouse();
            _worldTree.ChangeTreeLevel();
        }
    }

    // 발전도 확인
    public int getDevelopLevel()
    {
        return _development_level;
    }

    public int getSeason()
    {
        return _season;
    }

    // 정화 여부 확인 (섹터 1번부터 8번까지)
    public bool isPurifiered(int index)
    {
        return _purification[index - 1];
    }

    public int getPuriCount()
    {
        return _purification_sector;
    }

    public void LoadingBar()
    {
        loadingBar.Rotate(loadingBar.forward * Time.deltaTime * 160);
    }

    public void InLoading()
    {
        loadingPanel.SetActive(true);
        loadingStartPage.SetActive(true);
        loadingEndPage.SetActive(false);
        loadingBar.rotation = Quaternion.Euler(0,0,0);
        _player.changeHealth(-1500);
        _player.changeEnergy(-1500);
        _inloading = true;
        _player.setCanAction(false);
        _player.setCanInteract(false);
        _player._canMove = false;
        GameObject.Find("PlayerObject").transform.position = teleportPosition.position;
        tphome.DayEndTeleport();
        setTimeSystem(true);
    }

    public void OutLoading()
    {
        _inloading = false;
        loadingStartPage.SetActive(false);
        loadingEndPage.SetActive(true);
        Invoke("OutLoadingPage",1.5f);
    }

    public void OutLoadingPage()
    {
        loadingPanel.SetActive(false);
        setTimeSystem(false);
        _player.setCanAction(true);
        _player.setCanInteract(true);
        _player._canMove = true;
    }

    public void setBackground(int num){
        _soundManager.FunctionCall(num);
    }

    public void playerDeath(){
        animalDestroy();
        _day += 1;
        _hour = ((_buffManager.whitePray || _buffManager.whiteSpirit) ? 5 : 6);
        _minute = 0;
        InLoading();
        if (_day >= 29)
        {
            _day -= 28;
            _month += 1;
            if (_month >= 9)
            {
                _month -= 8;
            }
            if (_month % 2 == 1)
            {
                UpdateSeason(_month / 2);
            }
        }
        UIManager._uimanagerInstance.closeAllPanel();
        Invoke("TimeCall",1.5f);
    }

    public void LoadGame()
    {
        _save.Load();
    }
}
