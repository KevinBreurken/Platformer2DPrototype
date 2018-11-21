using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField]
    private Transform[] cameraTargets;
    [SerializeField]
    private float smoothTime;
    [SerializeField]
    private float maxSpeed;

    private Vector2 cameraVelocity;

    void Awake ()
    {
        if(cameraTargets.Length > 0)
        transform.position = cameraTargets[0].position;
    }

    void Update ()
    {
        if(cameraTargets.Length > 0)
        transform.position = Vector2.SmoothDamp(transform.position, GetCenterBetweenCameraTargets(), ref cameraVelocity, smoothTime, maxSpeed, Time.deltaTime);
    }

    public Vector2 GetCenterBetweenCameraTargets ()
    {
        if (cameraTargets.Length == 1)
            return cameraTargets[0].transform.position;

        Vector3 combinedPositions = Vector3.zero;
        for (int i = 0; i < cameraTargets.Length; i++)
        {
            combinedPositions += cameraTargets[i].transform.position;
        }
        combinedPositions = combinedPositions / 2;

        return combinedPositions;
    }

}
