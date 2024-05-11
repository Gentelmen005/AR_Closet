using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    [SerializeField] private string _displayName;
    [SerializeField] private string _description;

    private int _number = -1;

    public string Name {
        get {
            if (_number >= 0)
            {
                return _displayName + " " + _number.ToString();
            }
            else
            {
                return _displayName;
            }
        }
    }

    public string Description {
        get {
            return _description;
        }
    }

    public void GiveNumber(int number)
    {
        _number = number;
    }
}
