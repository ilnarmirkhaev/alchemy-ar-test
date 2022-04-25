using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField] private Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);
    
    [SerializeField] private GameObject[] objectsToPlace;

    private Dictionary<string, GameObject> _arObjects = new Dictionary<string, GameObject>();
    
    private ARTrackedImageManager _trackedImageManager;

    [SerializeField] private GameObject textPrefab;

    public void HideAllObjects()
    {
        foreach (var obj in _arObjects.Values)
        {
            obj.SetActive(false);
        }
    }

    private void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // Setup game objects in dictionary
        foreach (var obj in objectsToPlace)
        {
            var newObject = CreateObject(obj);
            _arObjects.Add(newObject.name, newObject);
        }
    }

    private GameObject CreateObject(GameObject obj)
    {
        var newObject = Instantiate(obj, Vector3.zero, Quaternion.identity);
            
        newObject.name = obj.name;
        newObject.transform.localScale = scaleFactor;
        newObject.SetActive(false);
        
        // Add text to object
        var text = Instantiate(textPrefab, newObject.transform.position, Quaternion.identity, newObject.transform);
        text.GetComponent<TMP_Text>().text = newObject.name;
        
        return newObject;
    }

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImage(trackedImage);
        }
        
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }
        
        foreach (var trackedImage in eventArgs.removed)
        {
            _arObjects[trackedImage.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        // something else, text, etc.
        AssignGameObjectToTrackedImage(trackedImage.referenceImage.name, trackedImage.transform.position);
    }

    private void AssignGameObjectToTrackedImage(string imageName, Vector3 imagePosition)
    {
        if (!_arObjects.ContainsKey(imageName)) return;
        
        var currentObject = _arObjects[imageName];
        currentObject.SetActive(true);
        currentObject.transform.position = imagePosition;
        
        // what if too many objects?
        // disable?
    }
}
