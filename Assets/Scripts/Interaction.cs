using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour, IInteractionManagerMode
{
    [SerializeField] private GameObject _ui;

    public float raycastDistance = 1f;
    public void Activate()
    {
        _ui.SetActive(true);
    }

    public void Deactivate()
    {
        _ui.SetActive(false);
    }

    public void TouchInterecrion(Touch[] touches)
    {
        Touch touch = touches[0];
        bool overUI = touch.position.IsPointOverUIObject();

            if (touch.phase != TouchPhase.Began || overUI)
                return;
            TryInteractWithAnimation(touch.position);
    }

    public void BackToDefaultScreen()
    {
        InteractionManager.Instance.SelectMode(0);
    }

    public void TryInteractWithAnimation(Vector2 pos)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Ray ray = InteractionManager.Instance.ARCamera.ScreenPointToRay(pos);
        RaycastHit hitObject;

        if (Physics.Raycast(ray, out hitObject))
        {
            if (hitObject.collider.CompareTag("Item"))
            {
                Item item = hitObject.collider.GetComponent<Item>();
                item.Interaction();
            }
        }
    }
}
