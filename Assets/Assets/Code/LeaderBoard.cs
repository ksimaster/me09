using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class LeaderBoard : MonoBehaviour
{
    [System.Serializable]
    public struct BoardEntry
    {
        public TextMeshProUGUI NameField;
        public TextMeshProUGUI RankField;
        public Transform Overlay;
    }

    [System.Serializable]
    public class BoardMemeber
    {
        public string Name;
        public CarDesciption CarPrefab;
        public float OvertakeTime;
        public bool IsBehind;
    }

    [Header("Init")]
    [SerializeField] private BoardEntry[] _leaderBoardRow;
    [SerializeField] private List<BoardMemeber> _boardMemebers;
    [SerializeField] private List<BoardMemeber> _boardMemebersEN;
    [SerializeField] private List<BoardMemeber> _boardMemebersTR;


    private int[] _rankList;

    private void Awake()
    {
        LoadSavedData();

        if (YandexGame.EnvironmentData.language == "en")
        {
            _boardMemebers = _boardMemebersEN;
        }
        if (YandexGame.EnvironmentData.language == "tr")
        {
            _boardMemebers = _boardMemebersTR;
        }
        _rankList = new int[_leaderBoardRow.Length];

       
    }

    public List<BoardMemeber> BoardMemebers
    {
        get => _boardMemebers;
        set => _boardMemebers = value;
    }

    private void Update()
    {
        UpdateBoards();
    }

    public void UpdateBoards()
    {
        int currentPlayerPosition = 0;
        for (int i = 0; i < _boardMemebers.Count; i++)
        {
            if (_boardMemebers[i].IsBehind)
            {
                currentPlayerPosition++;
            }
            else
            {
                break;
            }
        }

        int playerPosOnBoard = 0;
        if (currentPlayerPosition == 1) playerPosOnBoard = 1;
        if (currentPlayerPosition >= 2) playerPosOnBoard = 2;
        if (currentPlayerPosition == _boardMemebers.Count) playerPosOnBoard = 3;

        _rankList[playerPosOnBoard] = currentPlayerPosition;
        if (playerPosOnBoard == 0)
        {
            _rankList[0] = currentPlayerPosition;
            _rankList[1] = currentPlayerPosition;
            _rankList[2] = currentPlayerPosition + 1;
            _rankList[3] = currentPlayerPosition + 2;
        }

        if (playerPosOnBoard == 1)
        {
            _rankList[0] = currentPlayerPosition - 1;
            _rankList[1] = currentPlayerPosition;
            _rankList[2] = currentPlayerPosition;
            _rankList[3] = currentPlayerPosition + 1;
        }

        if (playerPosOnBoard == 2)
        {
            _rankList[0] = currentPlayerPosition - 2;
            _rankList[1] = currentPlayerPosition - 1;
            _rankList[2] = currentPlayerPosition;
            _rankList[3] = currentPlayerPosition;
        }

        if (playerPosOnBoard == 3)
        {
            _rankList[0] = currentPlayerPosition - 3;
            _rankList[1] = currentPlayerPosition - 2;
            _rankList[2] = currentPlayerPosition - 1;
            _rankList[3] = currentPlayerPosition;
        }

        bool playerSet = false;
        for (int i = 0; i < _leaderBoardRow.Length; i++)
        {
            int rivalIndex = _rankList[i];

            if (playerPosOnBoard == i)
            {
                if (YandexGame.EnvironmentData.language == "ru")
                {
                    _leaderBoardRow[i].NameField.text = "рш";
                }
                if (YandexGame.EnvironmentData.language == "en")
                {
                    _leaderBoardRow[i].NameField.text = "YOU";
                }
                if (YandexGame.EnvironmentData.language == "tr")
                {
                    _leaderBoardRow[i].NameField.text = "SEN";
                }


                _leaderBoardRow[i].RankField.text = "#" + (_boardMemebers.Count - (rivalIndex)).ToString();
                _leaderBoardRow[i].Overlay.gameObject.SetActive(true);
                playerSet = true;
            }
            else
            {
                int rankNumber = 0;
                if (playerSet) rankNumber = 1;
                _leaderBoardRow[i].NameField.text = _boardMemebers[rivalIndex].Name;
                _leaderBoardRow[i].RankField.text = "#" + (_boardMemebers.Count - (rivalIndex + rankNumber)).ToString();
                _leaderBoardRow[i].Overlay.gameObject.SetActive(false);
            }
        }
    }

    public CarDesciption GetNextRivalInFront()
    {
        CarDesciption carDesciption = null;
        for (int i = 0; i < _boardMemebers.Count; i++)
        {
            if (!_boardMemebers[i].IsBehind)
            {
                return _boardMemebers[i].CarPrefab;
            }
        }

        return carDesciption;
    }

    private void LoadSavedData()
    {
        foreach (var member in _boardMemebers)
        {
            member.IsBehind = PlayerPrefs.GetInt(member.Name) == 1;
        }
    }

    public void SaveData()
    {
        foreach(var member in _boardMemebers)
        {
            PlayerPrefs.SetInt(member.Name, member.IsBehind? 1 : 0);
        }
    }
}



