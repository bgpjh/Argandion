using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public bool _hold_cam;
    private PlayerSystem _ps;
    private SystemManager _sm;
    private GameObject _cam;
    private GameObject _po;
    // Start is called before the first frame update
    void Start()
    {
        _po = GameObject.Find("PlayerObject");
        _ps = _po.GetComponent<PlayerSystem>();
        _sm = GameObject.Find("SystemManager").GetComponent<SystemManager>();
        _cam = GameObject.Find("Main Camera");
    }

    public void CookingCamera()
    {
        Vector3 pos = new Vector3(_po.transform.position.x+0.15f,_po.transform.position.y+2.51f,_po.transform.position.z+2.75f);
        Quaternion rot = Quaternion.Euler(30f,180f,0.4f);
        _cam.transform.SetPositionAndRotation(pos,rot);
    }

    public void ResetCamera()
    {
        Vector3 pos = new Vector3(_po.transform.position.x+0f,_po.transform.position.y+5.5f,_po.transform.position.z+-8f);
        Quaternion rot = Quaternion.Euler(30f,0f,0f);
        _cam.transform.SetPositionAndRotation(pos,rot);
    }

    // test code
    void Update()
    {
        if (_hold_cam)
        {
            CookingCamera();
            _hold_cam = false;
        }
    }
}