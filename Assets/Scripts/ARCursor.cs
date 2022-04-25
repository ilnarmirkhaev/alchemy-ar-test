using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCursor : MonoBehaviour
{
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager;
    private GameObject _cursorObject;
    [SerializeField] private bool useCursor;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        _cursorObject = gameObject.transform.GetChild(0).gameObject;
        _cursorObject.SetActive(useCursor);
        
        if (Camera.main == null)
            Debug.LogWarning("Missing main camera");
    }

    private void Update()
    {
        if (useCursor)
            UpdateCursor();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            Instantiate(objectToPlace, _transform.position, _transform.rotation);
    }

    private void UpdateCursor()
    {
        var screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            _transform.position = hits[0].pose.position;
            _transform.rotation = hits[0].pose.rotation;
        }
    }
}
