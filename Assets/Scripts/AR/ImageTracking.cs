using System.Collections.Generic;
using AlchemyAR.Alchemy;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AlchemyAR.AR
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ImageTracking : MonoBehaviour
    {
        [SerializeField] private Vector3 refSize = new Vector3(0.1f, 0.1f, 0.1f);
    
        [SerializeField] private GameObject[] objectsToPlace;

        public readonly Dictionary<string, GameObject> ARObjects = new Dictionary<string, GameObject>();
    
        private ARTrackedImageManager _trackedImageManager;

        [SerializeField] private GameObject labelPrefab;

        public void ResetAllObjects()
        {
            foreach (var obj in ARObjects.Values)
            {
                if (obj.TryGetComponent(out Ingredient ingredient))
                    ingredient.ResetValues();
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
                ARObjects.Add(newObject.name, newObject);
            }
        }

        private GameObject CreateObject(GameObject obj)
        {
            var newObject = Instantiate(obj, Vector3.zero, Quaternion.identity);
            newObject.name = obj.name;

            // Add Ingredient script component to object
            if (!newObject.CompareTag("Potion") && !newObject.TryGetComponent(out TemperatureCrystal _) && !newObject.TryGetComponent(out Ingredient _))
                newObject.AddComponent<Ingredient>();
            
            // Add label to object
            var label = Instantiate(labelPrefab, new Vector3(0, 0, -0.6f), Quaternion.identity, newObject.transform);
            label.GetComponent<TMP_Text>().text = newObject.name;
        
            // Scale last to not break label position
            ResizeObject(newObject, label);
            
            newObject.SetActive(false);
        
            return newObject;
        }

        private void ResizeObject(GameObject obj, GameObject label)
        {
            var objSize = new Vector3();
            
            if (obj.TryGetComponent(out Renderer r))
                objSize = r.bounds.size;
            else if (obj.TryGetComponent(out Collider c))
                objSize = c.bounds.size;

            var objScale = obj.transform.localScale;
            
            var resize = refSize.x / objSize.x * objScale.x;

            obj.transform.localScale = new Vector3(resize, resize, resize);

            // Fix label transform
            label.transform.localScale = refSize / resize;
            label.transform.position *= refSize.x / resize;
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
                ARObjects[trackedImage.referenceImage.name].SetActive(false);
            }
        }

        private void UpdateTrackedImage(ARTrackedImage trackedImage)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
                AssignGameObjectToTrackedImage(trackedImage.referenceImage.name, trackedImage.transform.position);
            else
                ARObjects[trackedImage.referenceImage.name].SetActive(false);
        }

        private void AssignGameObjectToTrackedImage(string imageName, Vector3 imagePosition)
        {
            if (!ARObjects.ContainsKey(imageName)) return;
        
            var currentObject = ARObjects[imageName];
            currentObject.SetActive(true);
            currentObject.transform.position = imagePosition;
        }
    }
}
