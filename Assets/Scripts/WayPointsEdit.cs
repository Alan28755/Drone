using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class WayPointsEdit : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private Transform wayPoints;

    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private GameObject pointPrefab;

    private Transform _drone;

    [SerializeField] private Button addPoint;

    [SerializeField] private Button deletePoint;


    //路径信息
    private WayPointsInfo _wayPointsInfo;

    // Start is called before the first frame update
    void Start()
    {
        wayPoints.transform.position=Vector3.zero;
        for (int i = 0; i < wayPoints.childCount; i++)
        {
            GameObject go = Instantiate(uiPrefab,gridLayoutGroup.transform);
            go.transform.GetChild(0).GetChild(1).GetComponent<Text>().text="节点"+i.ToShortString();
            for (int j = 0; j < 3; j++)
            {
                go.transform.GetChild(0).GetChild(0).GetChild(j).GetComponent<TMP_InputField>().
                    onValueChanged.AddListener(OnInputFieldValueChanged);
            }
        }

        gridLayoutGroup.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "出发点";

        addPoint.onClick.AddListener(AddPoint);
        deletePoint.onClick.AddListener(DeletePoint);

        _drone = GameObject.FindWithTag("Drone").transform;


    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < wayPoints.childCount; i++)
        {
            Transform uiWaypoint = gridLayoutGroup.transform.GetChild(i).GetChild(0).GetChild(0);
            Transform wayPoint = wayPoints.transform.GetChild(i);
            for (int j = 0; j < 3; j++)
            {
                TMP_InputField tmpInputField = uiWaypoint.transform.GetChild(j).GetComponent<TMP_InputField>();
                tmpInputField.SetTextWithoutNotify(wayPoint.transform.position[j].ToShortString());
            }
        }


    }

    private void AddPoint()
    {
        int count = wayPoints.childCount;
        Transform lastPoint = wayPoints.transform.GetChild(count-1);
        Vector3 loc = lastPoint.position;
        float r = lastPoint.GetComponent<SphereCollider>().radius;
        Vector3 newloc = loc + Vector3.up * r;
        GameObject point =Instantiate(pointPrefab, newloc,quaternion.identity,wayPoints.transform);
        count++;


        wayPoints.GetChild(count - 2).GetComponent<DroneWaypointScript>().waypointList[0] = point;
        point.GetComponent<DroneWaypointScript>().waypointList[0] = wayPoints.GetChild(1).gameObject;

        GameObject uiPoint = Instantiate(uiPrefab, gridLayoutGroup.transform);
        uiPoint.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "节点" + count.ToShortString();
        //订阅
        for (int j = 0; j < 3; j++)
        {
            uiPoint.transform.GetChild(0).GetChild(0).GetChild(j).GetComponent<TMP_InputField>().
                onValueChanged.AddListener(OnInputFieldValueChanged);
        }

    }

    private void DeletePoint()
    {
        int count = wayPoints.childCount;
        //获取最后一行坐标ui
        GameObject uiPoint = gridLayoutGroup.transform.GetChild(count - 1).gameObject;
        //取消订阅
        for (int j = 0; j < 3; j++)
        {
            uiPoint.transform.GetChild(0).GetChild(0).GetChild(j).GetComponent<TMP_InputField>().
                onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
        //销毁该ui
        Destroy(uiPoint);

        GameObject lastPoint = wayPoints.GetChild(count - 2).gameObject;
        lastPoint.GetComponent<DroneWaypointScript>().waypointList[0] = wayPoints.GetChild(1).gameObject;
        GameObject point = wayPoints.GetChild(count - 1).gameObject;
        //检查下一个点是否是要销毁的对象
        if (_drone.GetComponent<FlyingDroneScript>().nextWaypoint == point)
        {
            _drone.GetComponent<FlyingDroneScript>().nextWaypoint = wayPoints.GetChild(1).gameObject;
        }
        Destroy(point);

    }

    private void OnInputFieldValueChanged(string value)
    {

        GameObject go = EventSystem.current.currentSelectedGameObject;
        int j = go.transform.GetSiblingIndex();
        int i = go.transform.parent.parent.parent.GetSiblingIndex();
        Vector3 pos = wayPoints.GetChild(i).position;
        pos[j] = float.Parse(value);
        wayPoints.GetChild(i).position = pos;
    }

    private void LoadWayPointsInfo()
    {
        var json = SavingSystem.LoadFromPlayerPrefs("wayPoints");
        var saveData = JsonUtility.FromJson<WayPointsInfo>(json);

        _wayPointsInfo.positions.Clear();
        _wayPointsInfo.positions = new List<Vector3>(saveData.positions.ToArray());


    }

}
