using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WayPointsEdit : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private Transform wayPoints;

    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private GameObject pointPrefab;

    [SerializeField] private Transform drone;
    // Start is called before the first frame update
    void Start()
    {
        wayPoints.transform.position=Vector3.zero;
        for (int i = 0; i < wayPoints.childCount; i++)
        {
            GameObject gameObject = Instantiate(uiPrefab);
            gameObject.transform.SetParent(gridLayoutGroup.transform);
            gameObject.transform.GetChild(0).GetChild(1).GetComponent<Text>().text="节点"+i.ToShortString();
            for (int j = 0; j < 3; j++)
            {
                gameObject.transform.GetChild(0).GetChild(0).GetChild(j).GetComponent<TMP_InputField>().
                    onValueChanged.AddListener(OnInputFieldValueChanged);


            }
        }


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

    public void AddPoint()
    {

    }

    public void DeletePoint()
    {

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
}
