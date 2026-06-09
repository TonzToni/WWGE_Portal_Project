using System;
using UnityEngine;

public class PortalRender : MonoBehaviour
{
    [Header("Portal One")]
    public GameObject portalOne;
    public GameObject portalOneRender;
    public Camera portalOneCam;

    [Header("Portal Two")]
    public GameObject portalTwo;
    public GameObject portalTwoRender;
    public Camera portalTwoCam;


    private Camera mainCamera;

    private RenderTexture entryPortalTex;
    private RenderTexture exitPortalTex;

    void Awake()
    {
        // find cameras
        mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();

        // create render texture for both portals, the RenderTextureFormat specifically needs to be set to match main camera
        entryPortalTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
        exitPortalTex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBFloat);
    }

    void Start()
    {
        // assign render texture to camera
        portalOneCam.targetTexture = exitPortalTex;
        portalTwoCam.targetTexture = entryPortalTex;

        // set portals mat texture to render texture
        portalOne.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", entryPortalTex);
        portalTwo.GetComponentInChildren<Renderer>().material.SetTexture("_MainTex", exitPortalTex);
    }

    void LateUpdate()
    {
        // sets up texture and camera befoe main player camera
        SetupPortalRender();
    }

    // sets up portal right before main camera render
    private void SetupPortalRender()
    {
            // match camera render texture resolution with game resolution
            // configure portal cam transforms
            // put zNear at the portal plane so it doesnt render objects behind portal

            // entry portal
            ResizeRenderTex(entryPortalTex, portalTwoCam);
            CameraTransform(portalTwoCam, portalOne);
            SetPortalClippingPlane(portalTwoCam, portalTwoRender.transform);
            
            // exit portal
            ResizeRenderTex(exitPortalTex, portalOneCam);
            CameraTransform(portalOneCam, portalTwo);
            SetPortalClippingPlane(portalOneCam, portalOneRender.transform);
    }

    void ResizeRenderTex(RenderTexture tex, Camera cam)
    {
        if (tex.width != Screen.width || tex.height != Screen.height)
        {
            // allows for window size changes dynamically without affecting portal
            tex.Release();
            tex.width = Screen.width;
            tex.height = Screen.height;
            cam.ResetAspect();
        }

        // keep fov the same on all cams
        cam.fieldOfView = mainCamera.fieldOfView;
    }

    void CameraTransform(Camera cam, GameObject portal)
    {
        // calculate player position as if it were child to portal
        Vector3 camPosOffset = portal.transform.InverseTransformPoint(mainCamera.transform.position);

        // set calculated position with inverted x / z so its behind portal
        cam.transform.localPosition = new Vector3(-camPosOffset.x, camPosOffset.y, -camPosOffset.z);

        // get both portal camera to copy main camera rotation reletive to there respective portals
        cam.transform.localRotation = Quaternion.Inverse(portal.transform.rotation) * mainCamera.transform.rotation;

        // apply a 180 degree offset so it faces portal
        cam.transform.localEulerAngles -= new Vector3(0, 180f, 0);
    }

    

    /**************************************************************************************
    finds and sets the cameras zNear using the Oblique projection matrix

    though id class this as a required standard algorithm, the struture came from 
    'Daniel Ilett (2019), https://danielilett.com/2019-12-18-tut4-3-matrix-matching/'
    **************************************************************************************/
    void SetPortalClippingPlane(Camera portalCam, Transform portalSurface)
    {
        // finds exact location of renderplane, the 2f hardcoded represents diference of space between portals
        Vector3 portalPlaneOffset = portalSurface.position - portalSurface.forward * (Math.Abs(portalSurface.localPosition.z) * 2);

        // create a clip plane where the render plane is and converts to world space coordinates for projection matrix
        Plane p = new Plane(-portalSurface.forward, portalPlaneOffset);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);

        // creates a new projection matrix with the new zNear calculation
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlane;

        // overrite the standard projection matrix with the new sqewed zNear at render plane
        portalCam.projectionMatrix = portalCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}
