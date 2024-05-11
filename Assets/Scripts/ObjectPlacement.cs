using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ObjectCreatePlacement : MonoBehaviour, IInteractionManagerMode
{
    [SerializeField] private GameObject[] _spawnedObjectPrefabs;
    [SerializeField] private GameObject _ui;

    public Item CreatedItem;
    public LayerMask placementLayer2;
    public LayerMask placementLayer;
    private int _spawnedObjectType = -1;
   
    public List<GameObject> arrayObject = new();
    public List<string> stringObject = new();

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
        
        if (_spawnedObjectType == -1)
            return;

        Touch touch = touches[0];
        bool overUI = touch.position.IsPointOverUIObject();
        bool flag;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !overUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
            {
                if (hit.collider.tag == "Item")
                {
                    if (hit.collider.TryGetComponent<Item>(out var item))
                    {
                        flag = item.IsOpen();
                        if (flag)
                        {
                            CalcPositionInsideBox(hit);
                        }
                    }
                }
                else
                {
                    CalcPositionInsideBox(hit);

                }
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer2))
            {
                if (hit.collider.tag == "Item")
                {
                    Collider tableCollider = hit.collider; 
                    Vector3 objectSize = _spawnedObjectPrefabs[_spawnedObjectType].GetComponent<Renderer>().bounds.size;
                    Vector3 spawnPosition = hit.point + (hit.normal * tableCollider.bounds.extents.y) + (hit.normal * objectSize.y / 2);
                    Instantiate(_spawnedObjectPrefabs[_spawnedObjectType], spawnPosition, Quaternion.identity, hit.collider.transform);
                }
            }

        }
    }

    public void CalcPositionInsideBox(RaycastHit hit)
    {
        Bounds bounds = hit.collider.bounds;
        Vector3 boxSize = bounds.size;

        Vector3 newPosition = hit.point;
        newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x + boxSize.x / 2, bounds.max.x - boxSize.x / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y + boxSize.y / 2, bounds.max.y - boxSize.y / 2);
        newPosition.z = Mathf.Clamp(newPosition.z, bounds.min.z + boxSize.z / 2, bounds.max.z - boxSize.z / 2);

        
        GameObject newObject = Instantiate(_spawnedObjectPrefabs[_spawnedObjectType], newPosition, _spawnedObjectPrefabs[_spawnedObjectType].transform.rotation, hit.collider.transform);

        //newObject.transform.parent = hit.collider.transform;
    }
}
