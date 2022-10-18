using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; } //create singleton to easily access the shake camera method

    [SerializeField] private float shakeIntensity = 0f; //shake intensity
    [SerializeField] private float shakeTime = 0f; //length of time we want shaking to occurr

    private float shakeTimeTotal; //this is required for the Mathf.Lerp function below
    private bool isCameraShaking = false; //to signify if shaking has commenced

    private CinemachineVirtualCamera cmCamera;
    private CinemachineBasicMultiChannelPerlin cmCameraShake;

    private void Awake()
    {
        Instance = this; //singleton instantiation
        cmCamera = this.GetComponent<CinemachineVirtualCamera>();
        cmCameraShake = cmCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Start is called before the first frame update
    void Start()
    {
        shakeTimeTotal = shakeTime;
    }

    // Update is called once per frame
    void Update()
    {
        GraduallyDecreaseCameraShake();
    }

    /// <summary>
    /// This method is called in Update() to Lerp the camera shake back down to 0 after its initial burst
    /// </summary>
    private void GraduallyDecreaseCameraShake()
    {
        if (shakeTime > 0 && isCameraShaking)
        {
            shakeTime -= Time.deltaTime;
            cmCameraShake.m_AmplitudeGain = Mathf.Lerp(shakeIntensity, 0f, 1 - (shakeTime / shakeTimeTotal));
        }
        else
        {
            cmCameraShake.m_AmplitudeGain = 0f;
            shakeTime = shakeTimeTotal;
            isCameraShaking = false;
        }
    }

    /// <summary>
    /// this method is called in the PlayerController class the frame we impact the ground
    /// We set the shakeIntensity and trigger isCameraShaking to true
    /// This then triggers the GraduallyDecreaseCameraShake() function whichh is called in update()
    /// </summary>
    public void ShakeCamera()
    {
        cmCameraShake.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
    }
}
