using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarDesciption : MonoBehaviour
{
    [Header("General")]
    public int Level = 1;

    [Header("Miniatures")]
    [SerializeField] private bool _isMiniature;
    [SerializeField] private GameObject _levelNumber;
    [SerializeField] private TextMeshPro _numberText;

    private Transform _poolNode;

    public bool isMainOn;
    public GameObject miniCar;

    public bool IsMiniature
    {
        get { return _isMiniature; }
        set
        {
            _numberText.text = Level.ToString();
            _levelNumber.SetActive(value);
            _isMiniature = value;
        }
    }

    public void SetPoolParent(Transform poolNode)
    {
        miniCar.SetActive(false);

        _poolNode = poolNode;
    }

    public void PoolBack()
    {
        miniCar.SetActive(false);
        transform.SetParent(_poolNode);
    }
}
