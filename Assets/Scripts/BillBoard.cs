using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera mCamera;


    private void Awake() {
        mCamera = Camera.main;
    }

    private void LateUpdate() {
        Vector3 targetPos = transform.position + mCamera.transform.rotation * Vector3.forward;
        Vector3 targetOrientation = mCamera.transform.rotation * Vector3.up;
        transform.LookAt(targetPos, targetOrientation);
    }
}
