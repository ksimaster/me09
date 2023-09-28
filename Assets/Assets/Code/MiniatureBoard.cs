using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniatureBoard : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CarLoader_Player _carLoader_Player;

    [Header("Dragging")]
    [SerializeField] private float _heightOffset = 1f;
    [SerializeField] private float _transitionSpeed = 10f;

    [Header("Miniatures")]
    [SerializeField] private int _poolSize = 5;
    [SerializeField] private LayerMask _carLayer;
    [SerializeField] private List<CarDesciption> _carPrefabs;

    [Header("Board")]
    [SerializeField] private LayerMask _boardLayer;
    [SerializeField] private LayerMask _slotLayer;
    [SerializeField] private MiniatureSlot[] _slots;

    private Transform _poolNode;
    private CarDesciption _draggedCar;

    [HideInInspector] public int CurrentCarLevel;

    public CarDesciption DraggedCar { get => _draggedCar; set => _draggedCar = value; }

    private void Awake()
    {
        GameObject newNode = new GameObject("MiniaturePool");
        newNode.SetActive(false);
        newNode.transform.SetParent(transform);
        newNode.transform.localPosition = Vector3.zero;
        newNode.transform.localEulerAngles = Vector3.zero;
        _poolNode = newNode.transform;

        for (int i = 0; i < _poolSize; i++)
        {
            foreach (var prefab in _carPrefabs)
            {
                var newCarGO = Instantiate(prefab, newNode.transform);
                newCarGO.name = prefab.name;
                newCarGO.transform.localPosition = Vector3.zero;
                newCarGO.transform.localEulerAngles = Vector3.zero;

                var carDesc = newCarGO.GetComponent<CarDesciption>();
                carDesc.SetPoolParent(_poolNode);
            }
        }

        LoadSavedData();
    }

    public void Update()
    {
        bool inputLocked = false;

        if (!inputLocked)
        {
            //START DRAG
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, _carLayer))
                {
                    var collider = hitInfo.collider;
                    _draggedCar = collider.GetComponent<CarDesciption>();
                }
            }

            //DO DRAG
            if (_draggedCar != null)
            {
                Vector3 newPos = _draggedCar.transform.position;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, _boardLayer))
                {
                    newPos = hitInfo.point + Vector3.up * _heightOffset;
                }

                _draggedCar.transform.position = Vector3.Lerp(_draggedCar.transform.position, newPos, Time.deltaTime * _transitionSpeed);
            }

            //END DRAG
            if (Input.GetMouseButtonUp(0))
            {
                if (_draggedCar != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(ray, out hitInfo, 10f, _slotLayer))
                    {
                        var collider = hitInfo.collider;
                        var foundSlot = collider.GetComponent<MiniatureSlot>();
                        if (foundSlot != null)
                        {
                            bool slotEmpty = (foundSlot.PositionPoint.childCount == 0);
                            if (slotEmpty)
                            {
                                //DROP INTO EMPTY SLOT
                                _draggedCar.transform.SetParent(foundSlot.PositionPoint);
                                _draggedCar.transform.localPosition = Vector3.zero;
                            }
                            else
                            {
                                //NOT EMPTY SLOT
                                var carInSlot = foundSlot.PositionPoint.GetChild(0);
                                var otherCarDescription = carInSlot.GetComponent<CarDesciption>();
                                bool notSameCar = (_draggedCar != otherCarDescription);
                                if (notSameCar && otherCarDescription.Level == _draggedCar.Level)
                                {
                                    //ITS A MATCH
                                    otherCarDescription.PoolBack();
                                    _draggedCar.PoolBack();

                                    int highierLevel = _draggedCar.Level + 1;
                                    var newCar = LoadMiniature(highierLevel, foundSlot.PositionPoint);
                                }
                            }
                        }
                    }
                    else if(Physics.Raycast(ray, out hitInfo, 10f, _boardLayer) && (CurrentCarLevel != _draggedCar.Level))
                    {
                        _draggedCar.PoolBack();
                        _carLoader_Player.LoadCar(_draggedCar.Level);
                        CurrentCarLevel = _draggedCar.Level;
                    }

                    _draggedCar.transform.localPosition = Vector3.zero;
                    _draggedCar = null;
                }
            }
        }
    }

    public CarDesciption LoadMiniature(int carLevel, Transform newParent = null)
    {
        for (int i = 0; i < _poolNode.childCount; i++)
        {
            var carGO = _poolNode.GetChild(i);
            CarDesciption desc = carGO.GetComponent<CarDesciption>();
            if (desc.Level == carLevel)
            {
                MiniatureSlot foundSlot = null;

                if (newParent != null)
                {
                    Debug.Log("Машины заполнены");
                    foundSlot = newParent.GetComponent<MiniatureSlot>();
                }

                if (foundSlot == null)
                {
                    foundSlot = GetEmptySlot();
                }
                carGO.SetParent(foundSlot.PositionPoint);
                carGO.localPosition = Vector3.zero;
                carGO.localEulerAngles = Vector3.zero;
                carGO.localScale = Vector3.one;

                desc.IsMiniature = true;
                return desc;
            }
        }

        return null;
    }

    private MiniatureSlot GetEmptySlot()
    {
        MiniatureSlot result = null;

        foreach(var slot in _slots)
        {
            if(slot.PositionPoint.childCount == 0)
            {
                result = slot;
                break;
            }
        }

        return result;
    }

    private void LoadSavedData()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            int slotLvl = PlayerPrefs.GetInt("Slot" + i, 0);
            if(slotLvl > 0)
            {
                LoadMiniature(slotLvl, _slots[i].PositionPoint);
            }
        }
    }

    public void SaveData()
    {
        int slotIndex = 0;
        foreach (var slot in _slots)
        {
            int lvl = 0;
            if (slot.PositionPoint.childCount > 0)
            {
                var child = slot.PositionPoint.GetChild(0);
                if (child != null)
                {
                    var carDesc = child.GetComponent<CarDesciption>();
                    lvl = carDesc.Level;
                }
            }
            PlayerPrefs.SetInt("Slot" + slotIndex, lvl);

            slotIndex++;
        }
    }
}
