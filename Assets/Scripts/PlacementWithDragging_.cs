using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithDragging_ : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private Camera arCamera;

    private GameObject placedObject;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private ARPlaneManager planeManager;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        dismissButton.onClick.AddListener(Dismiss);

        // get a reference to the ARPlaneManager as well
        planeManager = GetComponent<ARPlaneManager>();

    }
    private void Dismiss() => welcomePanel.SetActive(false);

    void Update()
    {
        // Don't place stuff when the welcome panel is up
        if (welcomePanel.activeSelf)
            return;

        // Are we touching the screen?
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                // might need to use the ARCamera instead. We'll see
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if(Physics.Raycast(ray, out hitObject))
                {
                    if(hitObject.transform.name.Contains("PlacedObject"))
                    {
                        onTouchHold = true;
                    }
                }
            }

            // The finger left the screen
            if(touch.phase == TouchPhase.Ended)
            {
                onTouchHold = false;
            }
        }

        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            var plane = planeManager.GetPlane(hits[0].trackableId);

            if(placedObject == null && plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            {
                placedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                // Is onTouchHold true and is it on a horizontal plane?
                if(onTouchHold && plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
                {
                    placedObject.transform.position = hitPose.position;
                    placedObject.transform.rotation = hitPose.rotation;
                }
            }
        }
    }
}
