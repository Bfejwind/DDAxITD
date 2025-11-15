using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, List<GameObject>> spawnedPrefabsGroups = new Dictionary<string, List<GameObject>>(); 

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            //Identify prefabs by the first part
            string identifier = prefab.name.Split('_')[0];
            //Put prefabs into a list
            if (!spawnedPrefabsGroups.ContainsKey(identifier))
            {
                spawnedPrefabsGroups[identifier] = new List<GameObject>();
            }
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.SetActive(false);
            spawnedPrefabsGroups[identifier].Add(newPrefab);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if(trackedImage != null)
        {
            string imageName = trackedImage.referenceImage.name;
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                foreach (GameObject prefab in spawnedPrefabsGroups[imageName])
                {
                    //Disable the associated content
                    prefab.transform.SetParent(null);
                    prefab.SetActive(false);
                }
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log(imageName + " is being tracked.");
                foreach (GameObject prefab in spawnedPrefabsGroups[imageName])
                {
                    Debug.Log(prefab.name + " spawned");
                    //Enable the associated content
                    if (prefab.name.Contains("Main"))
                    {
                        Debug.Log("Main is shown");
                        prefab.transform.SetParent(trackedImage.transform);
                        prefab.transform.localPosition = Vector3.zero;
                        prefab.transform.localRotation = Quaternion.identity;
                        prefab.SetActive(true);
                    }
                    else if (prefab.name.Contains("End"))
                    {
                        OffsetPrefabs offset = prefab.GetComponent<OffsetPrefabs>();
                        if (offset != null)
                        {
                            prefab.transform.localPosition = trackedImage.transform.position + offset.positionOffset;
                            prefab.transform.localRotation = Quaternion.Euler(offset.rotationOffset);
                            prefab.SetActive(true);
                        }
                        else
                        {
                            prefab.transform.localPosition = Vector3.zero;
                            prefab.transform.localRotation = Quaternion.identity;
                            prefab.SetActive(true);
                            Debug.Log("No offset!Put offset for End");
                        }
                    }
                    else if (prefab.name.Contains("Start"))
                    {
                        prefab.transform.localPosition = trackedImage.transform.localPosition;
                        prefab.transform.localRotation = Quaternion.identity;
                        prefab.SetActive(true);
                    }
                }
            }
        }
    }
}
