using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public enum planeState { Set, NotSet };
public class FocusedAreaPlacement : MonoBehaviour {

  
    public UnityARSessionNativeInterface currentSession;
      public static FocusedAreaPlacement instance;

    //Testing
    bool PlanesFound = false;
    public bool planeSet = false;
    //camera object
    Vector3 CamPos;
    Quaternion CamRot;
    [Header ("ARKit Helpers")]
    public GameObject planeSearcher;
    public GameObject cloudSearcher;

    [Header("AR Plane setup")]
    int selectedPlaneValue;
    private UnityARAnchorManager unityARAnchorManager;
    List<ARPlaneAnchorGameObject> arpags = new List<ARPlaneAnchorGameObject>();
    public GameObject GroundPlane;
    GameObject newFloor;
    public planeState planePlacementState;

    public Rect TouchArea;
    //testing

    //finding the place to place
    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                for(int i =0;i<arpags.Count;i++)
                {
                    if (hitResult.anchorIdentifier == arpags[i].planeAnchor.identifier)
                    {
                        selectedPlaneValue = i;
                      //  planePlacementState = planeState.Set;
                       // print(planePlacementState);

                        PlaceFloorPlane(UnityARMatrixOps.GetPosition(hitResult.worldTransform));
                        return true;
                    }
                }
              
            }
        }
        return false;
    }

    private void Awake()
    {
         instance = this;
        TouchArea = GameManager.instance.TouchArea;
    }

    void Start () {
    
        unityARAnchorManager = new UnityARAnchorManager();
  
	}

    void Update()
    {

        
            if (Input.touchCount > 0 && planePlacementState == planeState.NotSet)
                TouchInput();

            if (planePlacementState == planeState.NotSet)
            {
                CheckingPlanes();
            }
 

    }

    void TouchInput()
    {
        Touch tap = Input.GetTouch(0);

        if(TouchArea.Contains(tap.position))
        {
            var screenPosition = Camera.main.ScreenToViewportPoint(tap.position);
            ARPoint point = new ARPoint
            {
                x = screenPosition.x,
                y = screenPosition.y
            };

            ARHitTestResultType[] resultTypes =
                {
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                        ARHitTestResultType.ARHitTestResultTypeFeaturePoint
        };

            foreach (ARHitTestResultType resultType in resultTypes)
            {
                if (HitTestWithResultType(point, resultType))
                {
                    return;
                }
            }
        }
    }

    void UITesting()
    {       
        CamPos = Camera.main.transform.localPosition;
        CamRot = Camera.main.transform.localRotation;
 
    }


    void CheckingPlanes()
    {
        arpags = unityARAnchorManager.GetCurrentPlaneAnchors();
        if (arpags.Count > 0)
        {
            PlanesFound = true;
        }        
    }

    public void PlaceFloorPlane(Vector3 HitResultPosition)
    {
      
        planeSearcher.SetActive(false);
        cloudSearcher.SetActive(false);

        newFloor = Instantiate(GroundPlane,HitResultPosition, arpags[selectedPlaneValue].gameObject.transform.rotation) as GameObject;
        newFloor.transform.position = HitResultPosition;
            //UnityARMatrixOps.GetPosition(arpags[selectedPlaneValue].planeAnchor.transform);
            Vector3 dir = transform.TransformVector(CamPos) - newFloor.transform.position;
            newFloor.transform.rotation = UnityARMatrixOps.GetRotation(arpags[selectedPlaneValue].planeAnchor.transform);
            planePlacementState = planeState.Set;
            newFloor.transform.rotation =  Quaternion.LookRotation(dir, newFloor.transform.up);
            Quaternion rot = newFloor.transform.rotation;
            rot.x = 0;
            rot.z = 0;
            newFloor.transform.rotation = rot;
            
        StartCoroutine(Ready());
    }

    IEnumerator Ready()
    {
        yield return new WaitForSeconds(1f);
        planeSet = true;

    }

    public void ResetFloorPosition()
    {
        newFloor.SetActive(false);
        planePlacementState = planeState.NotSet;
        planeSearcher.SetActive(true);
        cloudSearcher.SetActive(true);
        planeSet = false;
    }
}