using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingWorld : MonoBehaviour
{
    [Header("Initial")]
    [SerializeField] private List<GameObject> _tilePrefabs;

    [Header("Setup")]
    [SerializeField] private int _bufferSize = 4;
    [SerializeField] private float _tileSize = 500f;

    [Header("Checkpoint")]
    [SerializeField] private GameObject _checkPoint;
    [SerializeField] private Transform _checkPointStartPos;

    private float _scrollSpeedInMph = 45f;

    [Header("Options")]
    //[SerializeField] private bool _useRandomTiles;

    private List<GameObject> _availableTiles;
    private List<GameObject> _activeBuffer;

    private void Awake()
    {
        _activeBuffer = new List<GameObject>(_bufferSize);
        _availableTiles = new List<GameObject>(_bufferSize);

        while (_availableTiles.Count < _bufferSize)
        {
            for (int i = 0; i < _tilePrefabs.Count; i++)
            {
                GameObject newTile = Instantiate(_tilePrefabs[i]);
                newTile.name = _tilePrefabs[i].name;
                newTile.transform.SetParent(transform);
                newTile.SetActive(false);

                _availableTiles.Add(newTile);
            }
        }

        //Add to BUFFER
        for (int i = 0; i < _bufferSize; i++)
        {
            _activeBuffer.Add(_availableTiles[i]);
        }

        //Remove Available
        for (int i = 0; i < _activeBuffer.Count; i++)
        {
            _availableTiles.Remove(_activeBuffer[i]);
        }
        
        //POSITION BUFFER TILES
        for (int i = 0; i < _activeBuffer.Count; i++)
        {
            _activeBuffer[i].SetActive(true);
            _activeBuffer[i].transform.position = Vector3.forward * _tileSize * i;

            // Wrap the tile around to the start of the buffer if its position has exceeded the buffer length.
            if (_activeBuffer[i].transform.position.z > _tileSize * (_bufferSize - 1))
            {
                _activeBuffer[i].transform.position = Vector3.back * _tileSize;
            }
        }
    }

    private void Update()
    {
        float scrollSpeedInUnityUnitsPerSecond = _scrollSpeedInMph / 2.23694f;

        GameObject tileNeedsRemoving = null;

        for (int i = 0; i < _activeBuffer.Count; i++)
        {
            _activeBuffer[i].transform.Translate(Vector3.forward * scrollSpeedInUnityUnitsPerSecond * Time.deltaTime);
            if (_activeBuffer[i].transform.position.z > _tileSize * (_bufferSize - 1))
            {
                //Remove distant tile from the buffer
                tileNeedsRemoving = _activeBuffer[i];
                tileNeedsRemoving.SetActive(false);
            }
        }

        if (_checkPoint != null && _checkPoint.activeInHierarchy)
        {
            _checkPoint.transform.Translate(Vector3.forward * scrollSpeedInUnityUnitsPerSecond * Time.deltaTime);

            if (_checkPoint.transform.position.z > _tileSize)
            {
                _checkPoint.SetActive(false);
            }
        }

        if (tileNeedsRemoving != null)
        {
            _activeBuffer.Remove(tileNeedsRemoving);
            _availableTiles.Add(tileNeedsRemoving);

            //Add a brand new tile to the buffer
            GameObject randomAvailableTile = _availableTiles[Random.Range(0, _availableTiles.Count)];
            GameObject firstTile = GetFirstTile();
            randomAvailableTile.transform.position = firstTile.transform.position + Vector3.back * _tileSize;
            randomAvailableTile.SetActive(true);

            _availableTiles.Remove(randomAvailableTile);
            _activeBuffer.Add(randomAvailableTile);
        }
    }

    private GameObject GetFirstTile()
    {
        GameObject closestTile = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject tile in _activeBuffer)
        {
            float distance = Vector3.Distance(tile.transform.position, Vector3.zero);
            if (distance < closestDistance)
            {
                closestTile = tile;
                closestDistance = distance;
            }
        }

        return closestTile;
    }

    public void UpdateSpeed(float mph)
    {
        _scrollSpeedInMph = mph;
    }

    public void ActivateCheckpoint()
    {
        _checkPoint.SetActive(true);
        _checkPoint.transform.position = _checkPointStartPos.position;
    }
}
