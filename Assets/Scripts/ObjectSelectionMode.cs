using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectSelectionMode : MonoBehaviour, IInteractionManagerMode
{
    [Tooltip("UI objects to disable")]
    [SerializeField] private GameObject _ui;

    private SpawnedObject _selectedObject = null;
    private bool _needResetTouch = false;
    public List<string> stringObjectfordelete = new();

    public void Activate()
    {
        _ui.SetActive(true);
        _selectedObject = null;
    }

    public void Deactivate()
    {
        _ui.SetActive(false);
        _selectedObject = null;
    }

    public void BackToDefaultScreen()
    {
        InteractionManager.Instance.SelectMode(0);
    }

    public void TouchInterecrion(Touch[] touches)
    {
        Touch touch = touches[0];
        bool overUI = touch.position.IsPointOverUIObject();

        if (_needResetTouch)
        {
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                _needResetTouch = false;
            else
                return;
        }

        if (_selectedObject == null)
        {
            if (touch.phase != TouchPhase.Began || overUI)
                return;

            TrySelectObject(touch.position);
            _needResetTouch = true;
        }
        else
        {
            if (touches.Length == 1)
            {
                MoveSelectedObject(touches);
            }
            else if (touches.Length == 2)
            {
                RotateSelectedObject(touch, touches[1]);
            }
        }
    }

    private void TrySelectObject(Vector2 pos)
    {
        Ray ray = InteractionManager.Instance.ARCamera.ScreenPointToRay(pos);
        RaycastHit[] hitObject = Physics.RaycastAll(ray);
        if (!Physics.Raycast(ray))
            return;

        foreach (RaycastHit hit in hitObject)
        {
            if (hit.collider.gameObject.CompareTag("CreatedObject"))
            {
                GameObject selectedObject = hit.collider.gameObject;
                _selectedObject = selectedObject.GetComponent<SpawnedObject>();
                if (!_selectedObject)
                    throw new MissingComponentException("[OBJECT_SELECTION_MODE] " + selectedObject.name + " has no description!");
            }
        }
    }
    private void MoveSelectedObject(Touch[] touches)
    {
        Touch touch = touches[0];
        bool overUI = touch.position.IsPointOverUIObject();
        if (touch.phase == TouchPhase.Began)
        {
            if (!overUI)
            {
                ARAnchor anchor = _selectedObject.GetComponent<ARAnchor>();
                if (anchor != null)
                    Destroy(anchor);
                return;
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            _selectedObject.AddComponent<ARAnchor>();
            return;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (!overUI)
            {
                if (_selectedObject != null)
                {
                    _selectedObject.transform.position = InteractionManager.Instance.GetARRaycastHits(touch.position)[0].pose.position;
                }
            }
        }
        
    }

    private void RotateSelectedObject(Touch touch1, Touch touch2)
    {
        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float distance = Vector2.Distance(touch1.position, touch2.position);
            float distancePrev = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);
            float delta = distance - distancePrev;

            if (Mathf.Abs(delta) > 0.0f)
                delta *= 0.1f;
            else
                delta *= -0.1f;

            _selectedObject.transform.rotation *= Quaternion.Euler(0.0f, delta, 0.0f);
        }
    }

    public void DeleteSelectedObject()
    {
        ObjectCreationMode creationMode = GetComponent<ObjectCreationMode>();

        if (_selectedObject != null)
        {
            StartCoroutine(LerpObjectScale(_selectedObject.transform.localScale, Vector3.zero, 1.0f, _selectedObject.gameObject));
            for (int i = 0; i < creationMode.stringObject.Count; i++)
            {
                if (creationMode.stringObject[i] == _selectedObject.name + "(Clone)")
                {
                    creationMode.stringObject.Remove(creationMode.stringObject[i]);
                   
                }
            }
            
        }
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
        Destroy(lerpObject);
    }


}
