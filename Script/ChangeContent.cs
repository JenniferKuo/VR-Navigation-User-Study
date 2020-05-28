using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using UnityEngine.SceneManagement;

public class ChangeContent : MonoBehaviour
{
    public GameObject arrowObject;
    public GameObject button;
    public GameObject MapView;
    public GameObject FixedCircleMapView;
    public GameObject CircleMask;
    public GameObject CircleMapView;
    public Camera MapCamera;
    public Camera FixedMapCamera;
    public Camera FPSCamera;
    public GameObject cellphone;
    Vector3 endPos, startPos;
    public GameObject mapObject;
    public Text describeText;
    public GameObject navMeshAgent;

    public SteamVR_Action_Boolean MapOnOff;
    public SteamVR_Action_Boolean PhoneOnOff;

    // a reference to the hand
    public SteamVR_Action_Single m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;
    public SteamVR_Input_Sources rightHand;
    public bool isPhoneLocked = false;

    public GameObject startCanvas;
    public GameObject endCanvas;
    public GameObject DirectionCanvas;
    public GameObject MapTaskCanvas;
    public GameObject cellPhone;
    public Text placeholder;
    public Text startInfo;
    public GameObject startPoints;
    public GameObject endPoints;
    public GameObject ARIcon;
    public GameObject SelfIcon;
    public GameObject directionBtn;
    public GameObject mapTaskBtn;
    public GameObject nextStageBtn;

    // For record
    bool lastPressState = false;
    Vector3 lastPosition;
    float distanceMoved;
    bool isArrived = false;
    bool isTesting = false;
    SaveData.Record r;
    List<SaveData.PressLog> pressLogs;
    float phoneActiveTime = 0;
    int pickUpNumber = 0;
    float taskTime = 0;
    List<Vector3> path;
    string condition = "";
    float angleDiff;
    float distanceDiff;

    private void Awake()
    {
        startPos = startPoints.transform.Find(VROption.startPoint.ToString()).position;
        endPos = endPoints.transform.Find(VROption.endPoint.ToString()).position;
        Debug.Log("Distance:");
        Debug.Log(Vector3.Distance(new Vector3(-36.1f, 0, 27.6f), endPos));
        VROption.endPos = endPos;

        Debug.Log(VROption.endPos);

        // 設定FPS起點位置
        transform.position = startPos;

        // 更改icon位置
        Transform icon = mapObject.transform.Find("DestinationIcon");
        icon.position = new Vector3(endPos.x, 30f, endPos.z);
        icon = mapObject.transform.Find("StartPointIcon");
        icon.position = new Vector3(startPos.x, 30f, startPos.z);
        ARIcon.transform.position = new Vector3(endPos.x, 3f, endPos.z);

        // 顯示目前設置
        Debug.Log(VROption.order + VROption.condition + VROption.ID + VROption.endPoint);
    }

    

    // Use this for initialization
    void Start()
    {
        // 初始化頭部朝向
        StartCoroutine(InitHeadRotation());

        // 禁止移動
        VRController.isWalkable = false;

        lastPosition = transform.position;
        // 判斷是哪一個scene
        Scene scene = SceneManager.GetActiveScene();

        // 初始化紀錄的物件
        r = new SaveData.Record();
        pressLogs = new List<SaveData.PressLog>();

        // 更改終點座標
        endPos = VROption.endPos;

        // 更改手機上顯示之目的地
        int endPoint = VROption.endPoint;

        // 更改起始點顯示資訊
        SetStartCanvas();

        if (VROption.isTesting)
        {
            isTesting = true;
            DirectionCanvas.SetActive(true);
            // 一開始就按下搜尋鍵
            ShowMapObject();
            // 鎖住手機，不能偷看
            if (VROption.endPoint != 0)
                isPhoneLocked = true;
        }
        else
        {
            cellphone.SetActive(true);
            startCanvas.SetActive(true);
        }

        // 如果是練習關 就不顯示task
        if (VROption.endPoint == 0)
        {
            DirectionCanvas.SetActive(false);
            startCanvas.SetActive(true);
        }

        switch (endPoint)
        {
            case 0:
                placeholder.text = "摩天輪";
                describeText.text = "目的地在摩天輪東北方約150公尺\n經過摩天輪後往右直行約110公尺\n往左方則會看到目的地";
                break;
            case 1:
                placeholder.text = "全家便利商店";
                describeText.text = "目的地在摩天輪東北方約213公尺\n經過摩天輪後往右直行約210公尺\n往右方則會看到目的地";
                break;
            case 2:
                placeholder.text = "全聯福利中心";
                describeText.text = "目的地在摩天輪西南方約209公尺\n經過摩天輪後往左直行約210公尺\n往右方則會看到目的地";
                break;
            case 3:
                placeholder.text = "UNIQLO";
                describeText.text = "目的地在摩天輪西北方約111公尺\n經過摩天輪以後的路口右轉\n持續直行約145公尺的左方會看到目的地";
                break;
            case 4:
                placeholder.text = "可不可紅茶專賣店";
                describeText.text = "目的地在摩天輪北方約196公尺\n在經過摩天輪前的路口右轉\n直行約165公尺到三角路口往右斜前方前進\n經過兩個路口的左方為目的地";
                break;

        }

        if(condition == "C" || condition == "D")
        {
            CircleMask.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveMapCamera();
       
        // 判斷是否抵達
        if (!isArrived) IsArrived();

        // 如果是在測試，就限制看手機
        if (isTesting && !isPhoneLocked) OpenCellphone();
        if (isPhoneLocked) cellphone.SetActive(false);
    }

    public SaveData.Record GetRecord()
    {
        return r;
    }
    public void OpenCellphone()
    {

        // 當按下右邊的trigger就開始記錄
        if (PhoneOnOff.GetState(rightHand))
        {
            cellphone.SetActive(true);
            phoneActiveTime += Time.deltaTime;

            // 如果上一個frame的狀態是放開trigger的
            if (lastPressState == false)
            {
                pickUpNumber += 1;
                // for log
                SaveData.PressLog log = new SaveData.PressLog();
                log.timeStamp = taskTime;
                log.isMap = MapView.activeSelf;
                log.describe = "Open Cellphone";
                log.pos = transform.position;
                pressLogs.Add(log);
            }
        }
        else
        {
            cellphone.SetActive(false);
            // 如果上個frame的狀態是按下trigger的
            if(lastPressState == true)
            {
                // for log
                SaveData.PressLog log = new SaveData.PressLog();
                log.timeStamp = taskTime;
                log.isMap = MapView.activeSelf;
                log.describe = "Close Cellphone";
                log.pos = transform.position;
                pressLogs.Add(log);
            }

        }
        lastPressState = PhoneOnOff.GetState(rightHand);
    }

    public void OpenMap()
    {
        SaveData.PressLog log = new SaveData.PressLog();
        log.timeStamp = taskTime;
        log.isMap = MapView.activeSelf;
        log.pos = transform.position;

        if (MapView.activeSelf)
        {
            log.describe = "Show AR";
        }
        else
        {
            log.describe = "Show Map";
        }
        pressLogs.Add(log);
    }

    public void SetStartCanvas()
    {
        condition = VROption.condition;
        Debug.Log(condition);

        if (condition == "D")
        {
            CircleMapView.SetActive(false);
            FixedCircleMapView.SetActive(true);
        }
        else if (condition == "C")
        {
            CircleMapView.SetActive(true);
            FixedCircleMapView.SetActive(false);
        }
        else if (condition == "B")
        {
            CircleMapView.SetActive(true);
            FixedCircleMapView.SetActive(false);
        }
        else
        {
            MapView.SetActive(true);
        }

        if (isTesting)
        {
            // startInfo.text += "按住右手板機鍵可讓手機出現，按住的秒數會列入查看手機的時間總長。\n";
            startInfo.text += "按下下方的\"開始導航\"按鈕，即會開始計時及記錄觀看手機的次數。";
        }
    }

    // 按下開始導航
    public void CloseStartCanvas()
    {
        startCanvas.SetActive(false);
        // 當關閉時，開始計時
        StartCoroutine(StartTimer());
        // 紀錄路線
        path = new List<Vector3>();
        InvokeRepeating("StartRecordPath", 0, 1);
        // 允許移動
        VRController.isWalkable = true;
    }

    public void ShowEndCanvas()
    {
        endCanvas.SetActive(true);
    }

    private void MoveMapCamera()
    {
        Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;
        float m_Speed = 0;

        Vector2 padPos = m_MoveValue.GetAxis(SteamVR_Input_Sources.LeftHand);
        float triggerValue = m_MovePress.GetAxis(SteamVR_Input_Sources.LeftHand);

        // If not pressing button
        if (triggerValue == 0.0f)
        {
            m_Speed = 0;
        }

        // If pressing touchpad
        if (triggerValue >= 0.0f)
        {
            m_Speed = 2.5f;
            MapCamera.transform.Translate(Vector3.up * m_Speed * padPos.y, Space.Self);
            MapCamera.transform.Translate(Vector3.right * m_Speed * padPos.x, Space.Self);
            FixedMapCamera.transform.position += m_Speed * new Vector3(padPos.x, 0, padPos.y);
        }

        // 調整mapCamera角度與頭朝向一致
        float fpsRot = FPSCamera.transform.eulerAngles.y;
        Vector3 mapRot = MapCamera.transform.eulerAngles;
        MapCamera.transform.rotation = Quaternion.Euler(mapRot.x, fpsRot, 0);
    }

    public void LocateCamera()
    {
        StartCoroutine(MoveCameraToOrigin());
    }

    private IEnumerator InitHeadRotation()
    {

        Debug.Log("init head rotation");
        Quaternion rot = FixedMapCamera.transform.rotation;
        int counter = 1;

        while (counter > 0)
        {
            yield return new WaitForSeconds(0.1f);
            counter--;
        }
        transform.rotation = Quaternion.Euler(0, -40, 0);
        // 旋轉相機朝向
        FixedMapCamera.transform.rotation = rot;
    }

    private IEnumerator InitCamera()
    {

        Debug.Log("init camera");
        float moveMax = 1;

        float targetRotation = 70f;

        while (MapCamera.transform.eulerAngles.x > targetRotation)
        {
            Vector3 eulerAngles = MapCamera.transform.eulerAngles;
            eulerAngles.x = eulerAngles.x - moveMax;
            MapCamera.transform.eulerAngles = eulerAngles;
            yield return null;
        }
    }

    private IEnumerator MoveCameraToOrigin()
    {
        StartCoroutine(InitCamera());
        float moveMax = 15;

        Vector3 target = new Vector3(0, 120, 0);
        Vector3 fixedTarget = new Vector3(0, 120, 0);

        while (Vector3.Distance(MapCamera.transform.localPosition, target) > 1 || Vector3.Distance(FixedMapCamera.transform.localPosition, fixedTarget) > 1)
        {
            MapCamera.transform.localPosition = Vector3.MoveTowards(MapCamera.transform.localPosition, target, moveMax);
            FixedMapCamera.transform.localPosition = Vector3.MoveTowards(FixedMapCamera.transform.localPosition, fixedTarget, moveMax);
            yield return null;
        }
        MapCamera.transform.Translate(Vector3.back * 30, Space.Self);
    }

    public void IsArrived()
    {
        if(isArrived!=true && endCanvas.activeSelf!= true && Vector3.Distance(transform.position, endPos) < 10)
        {
            isArrived = true;
            Arrived();
        }
    }

    public void Arrived()
    {
        // 停止記錄路線
        CancelInvoke();
        Vector3 pos = transform.localPosition;
        pos.y = 2;

        // 禁止移動
        VRController.isWalkable = false;

        if (isTesting && VROption.endPoint != 0)
        {
            // 隱藏目的地icon
            Transform icon = mapObject.transform.Find("DestinationIcon");
            icon.gameObject.SetActive(false);
            // 隱藏自己的icon
            SelfIcon.SetActive(false);
            // 隱藏路線
            icon = mapObject.transform.Find("LineRenderer");
            icon.gameObject.SetActive(false);
            // 顯示起點icon
            mapObject.SetActive(true);
            icon = mapObject.transform.Find("StartPointIcon");
            icon.gameObject.SetActive(true);
            FixedMapCamera.gameObject.SetActive(false);

            // 將地圖camera移回原點
            Vector3 newPos = new Vector3(startPos.x, FixedMapCamera.transform.position.y, startPos.z);
            FixedMapCamera.transform.position = newPos;

            // 放大相機尺寸
            FixedMapCamera.orthographicSize = 180f;

            FixedMapCamera.gameObject.SetActive(true);
            // 打開地圖標記任務
            MapTaskCanvas.SetActive(true);
            // 鎖住手機 不能偷看
            isPhoneLocked = true;
        }
        else
        {
            endCanvas.SetActive(true);
            // 如果是練習階段 就直接儲存資料
            SaveRecord();
        }

        /*
        endCanvas.transform.localRotation = transform.localRotation;
        endCanvas.transform.localPosition = pos;
        // 往後推5公尺
        endCanvas.transform.position += endCanvas.transform.forward * 5;
        */
    }

    // 按下搜尋鍵
    public void ShowMapObject()
    {
        // 讓地圖上的物件顯示
        mapObject.SetActive(true);

        // 隱藏搜尋欄
        GameObject.Find("SearchBox").SetActive(false);

        if (condition == "A")
        {
            InvokeRepeating("LocateCamera", 0f, 10f);
        }
        // 產生AR箭頭
        else if (condition == "B")
        {
            navMeshAgent.GetComponent<NavMeshTester>().CallGenerateArrow();
        }
        // 大方向箭頭
        else if(condition == "C")
        {
            arrowObject.SetActive(true);
        }
        else if (condition == "D")
        {
            arrowObject.SetActive(true);
            describeText.gameObject.SetActive(true);
        }

        StartCoroutine(ChangeCameraSize());
    }

    // 計算方位與實際方位的差距
    public void GetAngleDiffFromCanvas()
    {
        CalculteDirection cal = DirectionCanvas.GetComponent<CalculteDirection>();
        angleDiff = cal.GetAngleDiff();
        DirectionCanvas.SetActive(false);
        isPhoneLocked = false;
        startCanvas.SetActive(true);
    }

    // 計算地圖標記與實際距離的差距
    public void GetDistanceDiffFromCanvas()
    {
        MapPointTask task = MapTaskCanvas.GetComponent<MapPointTask>();
        distanceDiff = task.GetDistanceDiff();
        MapTaskCanvas.SetActive(false);
        isPhoneLocked = false;
        endCanvas.SetActive(true);

        // 儲存紀錄
        SaveRecord();
    }
    public void SaveRecord()
    {
        // 紀錄成績
        r.phoneActiveTime = phoneActiveTime;
        r.pickUpNumber = pickUpNumber;
        r.taskTime = Mathf.Round(taskTime);
        r.ID = VROption.ID;
        r.isTesting = isTesting;
        r.distanceMoved = Mathf.Round(distanceMoved);
        r.condition = condition;
        r.path = path;

        // 如果是正式測試才要紀錄的資訊
        if (isTesting)
        {
            r.pressLog = pressLogs;
            r.angleDiff = angleDiff;
            r.distanceDiff = distanceDiff;
        }

        SaveData.SaveFile(r);
    }

    private IEnumerator StartTimer()
    {
        Debug.Log("start timer");
        while (isArrived != true && startCanvas.activeSelf != true)
        {
            taskTime += Time.deltaTime;
            // 計算移動距離
            distanceMoved += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            yield return null;
        }
    }

    private void StartRecordPath()
    {
        path.Add(transform.position);
    }

    private IEnumerator ChangeCameraSize()
    {
        float speed = 5;
        float size = 50.0f;

        while (MapCamera.orthographicSize > size)
        {
            MapCamera.orthographicSize -= speed;
            yield return null;
        }
    }

    public void GoToNextStage()
    {
        if(VROption.finish_idx == 7)
        {
            nextStageBtn.GetComponentInChildren<Text>().text = "結束實驗";
            Application.Quit();
        }
        VROption vr = new VROption();
        vr.nextStage();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowDirectionConfirmPanel()
    {
        Transform panel = DirectionCanvas.transform.Find("ConfirmPanel");
        panel.gameObject.SetActive(true);
    }

    public void HideDirectionConfirmPanel()
    {
        Transform panel = DirectionCanvas.transform.Find("ConfirmPanel");
        panel.gameObject.SetActive(false);
    }

    public void ShowMapConfirmPanel()
    {
        Transform panel = MapTaskCanvas.transform.Find("ConfirmPanel");
        panel.gameObject.SetActive(true);
    }

    public void HideMapConfirmPanel()
    {
        Transform panel = MapTaskCanvas.transform.Find("ConfirmPanel");
        panel.gameObject.SetActive(false);
    }
}
