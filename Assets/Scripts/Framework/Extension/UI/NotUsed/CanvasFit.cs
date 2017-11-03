using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

// Fit the canvas in the world space camera's frustum;
public class CanvasFit : MonoBehaviour
{
    public Camera refCamera;
    public float planeDistance = 100.0f;

    //public Canvas refCanvas2D;

    void Awake()
    {
        if (refCamera != null)
        {
            AdjustCanvasToCameraViewport();
        }
    }
    
#if UNITY_EDITOR
    // Editor Only!
    [ExecuteInEditMode]
    void OnValidate()
    {
        if (refCamera != null)
        {
            AdjustCanvasToCameraViewport();
        }
    }
#endif

    void AdjustCanvasToCameraViewport()
    {
        float frustumH = GetFrustumHeight(refCamera, planeDistance);

        RectTransform rt = transform as RectTransform;
        //rt.sizeDelta = new Vector2(rt.sizeDelta.y * refCamera.aspect, rt.sizeDelta.y);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x / refCamera.aspect);
        rt.localScale = Vector3.one * frustumH / rt.sizeDelta.y;
        rt.localPosition = new Vector3(0, 0, planeDistance);
        //rt.localRotation = Quaternion.LookRotation(-refCamera.transform.forward); //refCamera.transform.localRotation;
    }

    public float GetFrustumHeight(Camera camera, float distance)
    {
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //var camera.fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
        //var frustumWidth = frustumHeight * camera.aspect;

        return frustumHeight;
    }
}
