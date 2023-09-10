using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarLoader_Player : MonoBehaviour
{
    public delegate void NewCarLoadedDelegate();
    public event NewCarLoadedDelegate OnNewCarLoaded;

    [Header("PLAYER")]
    [SerializeField] private Transform _carPosition;
    [SerializeField] private List<CarDesciption> _carPrefabs;

    private Transform _poolNode;
    private CarDesciption _loadedCar;
    private CarDesciption _editorLoadedCar;

    public CarDesciption LoadedCar
    {
        get => _loadedCar;
        set => _loadedCar = value;
    }

    private void Awake()
    {
        if(_editorLoadedCar != null)
        {
            Destroy(_editorLoadedCar.gameObject);
        }

        GameObject newNode = new GameObject("CarPool");
        newNode.SetActive(false);
        newNode.transform.SetParent(_carPosition);
        newNode.transform.localPosition = Vector3.zero;
        newNode.transform.localEulerAngles = Vector3.zero;
        _poolNode = newNode.transform;

        foreach (var prefab in _carPrefabs)
        {
            var newCarGO = Instantiate(prefab, _poolNode);
            newCarGO.name = prefab.name;
            newCarGO.transform.localPosition = Vector3.zero;
            newCarGO.transform.localEulerAngles = Vector3.zero;
        }

        LoadSavedData();
    }

    public void LoadCar(int carLevel)
    {
        //Remove prev car
        if (LoadedCar != null)
        {
            var childGO = LoadedCar.transform;
            childGO.parent = _poolNode;
        }

        for (int i = 0; i < _poolNode.childCount; i++)
        {
            var carGO = _poolNode.GetChild(i);
            CarDesciption desc = carGO.GetComponent<CarDesciption>();
            if(desc.Level == carLevel)
            {
                desc.IsMiniature = false;

                carGO.SetParent(_carPosition);
                LoadedCar = desc;

                if (OnNewCarLoaded != null)
                {
                    OnNewCarLoaded();
                }
                break;
            }
        }
    }

    public void LoadCar()
    {
        if (_editorLoadedCar != null)
        {
            DestroyImmediate(_editorLoadedCar.gameObject);
        }

        var newCarGO = Instantiate(_carPrefabs[0], _carPosition);
        newCarGO.name = _carPrefabs[0].name;
        newCarGO.transform.localPosition = Vector3.zero;
        newCarGO.transform.localEulerAngles = Vector3.zero;
        _editorLoadedCar = newCarGO.GetComponent<CarDesciption>();
    }

    private void LoadSavedData()
    {
        var carLvl = PlayerPrefs.GetInt("LoadedCar.Level", 0);

        if(carLvl != 0)
        {
            LoadCar(carLvl);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("LoadedCar.Level", LoadedCar.Level);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CarLoader_Player))]
public class CarLoader_Player_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(Application.isPlaying)
        {
            return;
        }

        CarLoader_Player myTarget = (CarLoader_Player)target;

        if (myTarget == null) return;

        GUILayout.Space(20f);

        if (GUILayout.Button("Load Car", GUILayout.Height(35f)))
        {
            myTarget.LoadCar();
        }
    }
}

#endif
