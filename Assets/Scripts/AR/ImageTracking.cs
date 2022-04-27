using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AlchemyAR.AR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ImageTracking : MonoBehaviour
    {
        [SerializeField] private Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);
    
        [SerializeField] private GameObject[] objectsToPlace;

        private Dictionary<string, GameObject> _arObjects = new Dictionary<string, GameObject>();
    
        private ARTrackedImageManager _trackedImageManager;

        [SerializeField] private GameObject labelPrefab;

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
        
            // Add label to object
            // var label = Instantiate(labelPrefab, new Vector3(0, 0, -0.6f), Quaternion.identity, newObject.transform);
            // label.GetComponent<TMP_Text>().text = newObject.name;
        
            // Scale last not to break label position
            newObject.transform.localScale = scaleFactor;
            newObject.SetActive(false);
        
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
            if (trackedImage.trackingState == TrackingState.Tracking)
                AssignGameObjectToTrackedImage(trackedImage.referenceImage.name, trackedImage.transform.position);
            else
                _arObjects[trackedImage.referenceImage.name].SetActive(false);
        }

        private void AssignGameObjectToTrackedImage(string imageName, Vector3 imagePosition)
        {
            if (!_arObjects.ContainsKey(imageName)) return;
        
            var currentObject = _arObjects[imageName];
            currentObject.SetActive(true);
            currentObject.transform.position = imagePosition;
        }
    }
}
