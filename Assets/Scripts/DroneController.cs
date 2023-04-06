using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private PathInfo pathInfo;

    [SerializeField] private float speed=0.5f;
    [SerializeField] private float waypointTolerance=0.01f;


    private CharacterController _characterController;
    private int _currentWayPointIndex = 0;

    [HideInInspector]
    public bool startCapture=false;

    // Start is called before the first frame update
    void Start()
    {
        _nextPosition = transform.position;
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Fly();
    }

    private Vector3 _nextPosition;
    private void Fly()
    {
        if (!startCapture)
        {
            return;
        }
        if (pathInfo != null)
        {
            if (AtWayPoint())
            {
                CycleWayPoint();
                Debug.Log("Switch to next Point");
            }

            _nextPosition = GetCurrentWayPoint();
            
        }

        _nextPosition = _nextPosition - transform.position;
        // Debug.Log(_nextPosition);
        // _characterController.Move(_nextPosition * speed * Time.deltaTime);
        _nextPosition = _nextPosition * speed * Time.deltaTime;
        transform.position += _nextPosition;
    }
    
    private bool AtWayPoint()
    {
        float d = Vector3.Distance(transform.position, GetCurrentWayPoint());
        return d < waypointTolerance;
    }
    
    private void CycleWayPoint()
    {
        _currentWayPointIndex = pathInfo.GetNextIndex(_currentWayPointIndex);
    }
    
    private Vector3 GetCurrentWayPoint()
    {
        return pathInfo.GetWayPoint(_currentWayPointIndex);
    }
}
