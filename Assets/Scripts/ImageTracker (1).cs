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
    public bool notSpawned;
    public string exercise;
    public string statType;
    private void Start()
    {
        notSpawned = true;
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
            exercise = GameManagerScript.Instance.exerciseChoice;
            if (!spawnedPrefabsGroups.ContainsKey(exercise))
            {
                return;
            }
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                foreach (GameObject prefab in spawnedPrefabsGroups[exercise])
                {
                    //Disable the associated content
                    prefab.transform.SetParent(null);
                    prefab.SetActive(false);
                }
                notSpawned = true;
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                foreach (GameObject prefab in spawnedPrefabsGroups[exercise])
                {
                    //Enable the associated content
                    if (prefab.name.Contains("Main"))
                    {
                        prefab.transform.SetParent(trackedImage.transform);
                        prefab.transform.localPosition = Vector3.zero;
                        prefab.transform.localRotation = Quaternion.identity;
                        prefab.SetActive(true);
                    }
                    else if (notSpawned)
                    {
                        if (prefab.name.Contains("End"))
                        {
                            prefab.transform.SetParent(null);
                            OffsetPrefabs offset = prefab.GetComponent<OffsetPrefabs>();
                            if (offset != null)
                            {
                                prefab.transform.position = trackedImage.transform.position + offset.positionOffset;
                                prefab.transform.rotation = Quaternion.Euler(offset.rotationOffset);
                                prefab.SetActive(true);
                            }
                            else
                            {
                                prefab.transform.position = Vector3.zero;
                                prefab.transform.rotation = Quaternion.identity;
                                prefab.SetActive(true);
                                Debug.Log("No offset!Put offset for End");
                            }
                        }
                        else if (prefab.name.Contains("Start"))
                        {
                            prefab.transform.SetParent(null);
                            prefab.transform.position = trackedImage.transform.position;
                            prefab.transform.rotation = Quaternion.identity;
                            prefab.SetActive(true);
                        }
                        else if (prefab.name.Contains("Bear"))
                        {
                            prefab.transform.SetParent(null);
                            prefab.transform.position = trackedImage.transform.position;
                            prefab.transform.rotation = Quaternion.identity;
                            prefab.SetActive(true);
                        }
                    }
                }
                notSpawned = false;
            }
        }
    }
}
