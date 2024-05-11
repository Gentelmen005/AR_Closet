using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectCreationMode : MonoBehaviour, IInteractionManagerMode
{
    [SerializeField] private GameObject[] _spawnedObjectPrefabs;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _targetMarkerPrefab;

    public int _spawnedObjectType = -1;
    private GameObject _targetMarker;
    public List<GameObject> arrayObject = new();
    public List<string> stringObject = new();

    private void Start()
    {
        // create target marker
        _targetMarker = Instantiate(
            original: _targetMarkerPrefab,
            position: Vector3.zero,
            rotation: _targetMarkerPrefab.transform.rotation
        );
        _targetMarker.SetActive(false);
    }

    public void Activate()
    {
        _ui.SetActive(true);
        _spawnedObjectType = -1;
    }

    public void Deactivate()
    {
        _ui.SetActive(false);
        _spawnedObjectType = -1;
    }

    public void BackToDefaultScreen()
    {
        InteractionManager.Instance.SelectMode(0);
    }

    public void SetSpawnedObjectType(int spawnedObjectType)
    {
        _spawnedObjectType = spawnedObjectType;
    }

    public void TouchInterecrion(Touch[] touches)
    {
        // if none are yet selected, return
        if (_spawnedObjectType == -1)
            return;

        Touch touch = touches[0];
        bool overUI = touch.position.IsPointOverUIObject();

        if (touch.phase == TouchPhase.Began)
        {
            if (!overUI)
            {
                ShowMarker(true);
                MoveMarker(touch.position);
            }
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (_targetMarker.activeSelf)
                MoveMarker(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            if (_targetMarker.activeSelf)
            {
                SpawnObject(touch);
                ShowMarker(false);
            }
        }
    }
    private void ShowMarker(bool value)
    {
        _targetMarker.SetActive(value);
    }

    private void MoveMarker(Vector2 touchPosition)
    {
        _targetMarker.transform.position = InteractionManager.Instance.GetARRaycastHits(touchPosition)[0].pose.position;
    }

    private void SpawnObject(Touch touch)
    {
        //if (CheckObjectList())
        //{
            GameObject createdObject = Instantiate(
                original: _spawnedObjectPrefabs[_spawnedObjectType],
                position: InteractionManager.Instance.GetARRaycastHits(touch.position)[0].pose.position,
                rotation: _spawnedObjectPrefabs[_spawnedObjectType].transform.rotation

            );
            stringObject.Add(createdObject.name);
            createdObject.AddComponent<ARAnchor>();
            StartCoroutine(LerpObjectScale(Vector3.zero, createdObject.transform.localScale, 1.0f, createdObject));
        //}
    }
    IEnumerator LerpObjectScale(Vector3 a, Vector3 b, float time, GameObject lerpObject)
    {
        float i = 0.0f;
        float rate = (1.0f / time);
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            lerpObject.transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }

    //private bool CheckObjectList()
    //{
    //    for (int i = 0; i < stringObject.Count; i++)
    //    {
    //        if (stringObject[i] == _spawnedObjectPrefabs[_spawnedObjectType].name + "(Clone)")
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}
}
