using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CameraEdit : MonoBehaviour
{
    [SerializeField] private Camera captureCamera;

    [SerializeField] private GameObject positionInput;
    [SerializeField] private GameObject rotationInput;

    //储存录像机位置
    private TMPro.TMP_InputField[] _inputFields;
    //储存录像机方位
    private TMPro.TMP_InputField[] _captureRotation;
    void Start()
    {
        _inputFields = new TMP_InputField[3];
        _captureRotation = new TMP_InputField[3];
        for (int i = 0; i < 3; i++)
        {
            _inputFields[i] = positionInput.transform.GetChild(i).GetComponent<TMP_InputField>();
            _captureRotation[i]=rotationInput.transform.GetChild(i).GetComponent<TMP_InputField>();
        }

    }

    void LateUpdate()
    {
        for (int i = 0; i < _inputFields.Length; i++)
        {
            _inputFields[i].SetTextWithoutNotify(captureCamera.transform.position[i].ToShortString());
            _captureRotation[i].SetTextWithoutNotify(captureCamera.transform.rotation[i].ToShortString());
        }
    }

    public void SetCameraPosition()
    {
        Vector3 position = new Vector3();
        position.x = float.Parse(_inputFields[0].text);
        position.y = float.Parse(_inputFields[1].text);
        position.z = float.Parse(_inputFields[2].text);
        captureCamera.transform.position = position;

    }

    public void SetCameraRotation()
    {
        Quaternion rotation = new Quaternion();
        rotation.x = float.Parse(_captureRotation[0].text);
        rotation.y = float.Parse(_captureRotation[1].text);
        rotation.z = float.Parse(_captureRotation[2].text);
        captureCamera.transform.rotation = rotation;
    }
}
