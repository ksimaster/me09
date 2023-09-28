using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncomeManager : MonoBehaviour
{
    [System.Serializable]
    private class DistReward
    {
        public float TimeDelay;
        public float Reward;
    }

    [SerializeField] private float _currentMoney;
    [Header("Setup")]
    [SerializeField] private MiniatureBoard _miniatureBoard;
    [SerializeField] private GameProgression _gameProgression;
    [SerializeField] private NumberAnimator _numberAnimator;
    [SerializeField] private IncomeGainUI _incomeGainUI;
    [SerializeField] private LoopingWorld _loopingWorld;
    [SerializeField] private LeaderBoard _leaderBoard;
    [SerializeField] private CarLoader_Player _carLoader_Player;

    [Header("Setup UI")]
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _costRewardText;
    [SerializeField] private TextMeshProUGUI _upgradeTMP;
    [SerializeField] private GameObject _buyButton_Free;
    [SerializeField] private GameObject _buyButton;
    [SerializeField] private GameObject _addMoney;
    [SerializeField] private GameObject _upgradeButton;
    [SerializeField] private Button _buyCarButtonComponent;
    [SerializeField] private Button _incomeUpgradeButtonComponent;
    [SerializeField] private UIColorToggler _buyCarColorToggler;
    [SerializeField] private UIColorToggler _incomeUpgradeColorToggler;

    [Header("Income")]
    [SerializeField] private float _gainInterval = 0.33f;
    [SerializeField] private float[] _incomePerMile;
    [SerializeField] private float[] _incomeMultiplier;

    [Header("Miniature Cost")]
    [SerializeField] private float[] _costPerBuy;
    [SerializeField] private float[] _costPerUpgrade;
    [SerializeField] private float[] _costReward;

    [Header("Checkpoints")]
    [SerializeField] private DistReward _rewardPerCheckpoint;

    [Header("AdsReward")]
    [SerializeField] private TextMeshProUGUI _rewardButtonText;

    private float _updateIntervalLeft;
    private int _multiplierIndex;
    private int _costButtonIndex;
    private int _upgradeButtonIndex;
    private int _costRewardButtonIndex;
    private float _currentCheckPointTime;
    private string priceString;
    private float boostReward = 1.2f;




    private void Awake()
    {
        for (int i = 20; i < 1200; i++)
        {
            _costPerBuy[i] = Mathf.Round( 5 * i * Mathf.Log(i, 10) * Mathf.Log(i, 10) + (_costPerBuy[i - 1] + 1));

        }
        _buyButton_Free.SetActive(true);
        _buyButton.SetActive(false);
        _upgradeButton.SetActive(false);
        LoadSavedData();

        //_costText.text = _costPerBuy[_costButtonIndex].ToString();
        _costText.text = CharMoney(_costPerBuy[_costButtonIndex]);
        _upgradeTMP.text = CharMoney(_costPerUpgrade[_upgradeButtonIndex]);


        //Debug.Log("ƒÎËÌÌ‡ Ï‡ÒÒË‚‡: " + _costPerBuy.Length);
    }

    public void AddMoney()
    {
        if(_costButtonIndex < 17)
        {
            _currentMoney += 1000;
        } else
        {
            _currentMoney += _costPerBuy[_costButtonIndex] * boostReward;
        }

        PlayerPrefs.SetFloat("_currentMoney", _currentMoney);
        PlayerPrefs.Save();
        //_rewardButtonText = 
    }

    public string CharMoney(float price)
    {
        if (price < 1000) priceString = price.ToString("0") + " $";
        if (price > 1000) priceString = (price / 1000).ToString("0.0") + " K$";
        if (price > 1000000) priceString = (price / 1000000).ToString("0.0") + " M$";
        if (price > 1000000000) priceString = (price / 1000000000).ToString("0.0") + " B$";
        if (price > 1000000000000) priceString = (price / 1000000000000).ToString("0.0") + " KB$";
        if (price > 1000000000000000) priceString = (price / 1000000000000000).ToString("0.0") + " MB$";
        if (price > 1000000000000000000) priceString = (price / 1000000000000000000).ToString("0.0") + " BB$";

        return priceString;
    }
   
    public void TryBuy()
    {
        if (_costButtonIndex < _costPerBuy.Length)
        {
            float buyCost = _costPerBuy[_costButtonIndex];

            if (buyCost <= _currentMoney)
            {
                _currentMoney -= buyCost;

                _miniatureBoard.LoadMiniature(1);

                _costButtonIndex++;
                _costButtonIndex = Mathf.Clamp(_costButtonIndex, 0, _costPerBuy.Length);

                UpdateButtonText();
            }
        }
    }

    public void TryUpgrade()
    {
        if (_upgradeButtonIndex < _costPerUpgrade.Length)
        {
            float buyCost = _costPerUpgrade[_upgradeButtonIndex];

            if (buyCost <= _currentMoney)
            {
                _currentMoney -= buyCost;

                _multiplierIndex++;

                _upgradeButtonIndex++;
                _upgradeButtonIndex = Mathf.Clamp(_multiplierIndex, 0, _costPerUpgrade.Length);

                UpdateButtonText();
            }
        }
    }

    public void Update()
    {
        //CASH TICK
        if (_updateIntervalLeft >= 0)
        {
            _updateIntervalLeft -= Time.deltaTime;

            //UPDATE TICK
            if (_updateIntervalLeft < 0f)
            {
                _updateIntervalLeft = _gainInterval;

                OnUpdateTick();
            }
        }

        //CHECKPOINT TICK
        if (_gameProgression.Current_MPH > 10f)
        {
            _currentCheckPointTime += Time.deltaTime;

            if (_currentCheckPointTime >= _rewardPerCheckpoint.TimeDelay)
            {
                _currentCheckPointTime = 0f;

                CheckpointReached();
            }
        }

        // UI Button State
        UpdateButtonState();
    }

    private void OnUpdateTick()
    {
        if (_carLoader_Player.LoadedCar == null) return;

        int carLevel = _carLoader_Player.LoadedCar.Level;
        int multIndex = Mathf.Clamp(_upgradeButtonIndex, 0, _costPerUpgrade.Length - 1);
        float incomeGain = _gameProgression.Current_MPH * (_incomePerMile[carLevel] / 60f) * _incomeMultiplier[multIndex];
        _currentMoney += incomeGain;

        _numberAnimator.SetTargetValue(_currentMoney);
        if (incomeGain > 0.1f)
        {
            _incomeGainUI.ShowIncomeGainElement(string.Format("+{0}", incomeGain.ToString("F1")));
        }
    }

    private void CheckpointReached()
    {
        _loopingWorld.ActivateCheckpoint();
        _incomeGainUI.ShowIncomeGainElement(string.Format("+{0} ◊≈ œŒ»Õ“!", _rewardPerCheckpoint.Reward.ToString("F1")));

        _currentMoney += _rewardPerCheckpoint.Reward;

        SaveData();
        //Debug.Log("REWARD: +" + _rewardPerCheckpoint.Reward + " Cash!");
    }

    private void UpdateButtonState()
    {
        if (_costButtonIndex != _costPerBuy.Length)
        {
            bool buyState = _currentMoney >= _costPerBuy[_costButtonIndex];
            _buyCarButtonComponent.interactable = buyState;
            _buyCarColorToggler.ToggleColor(buyState);
        }
        if (_upgradeButtonIndex != _costPerUpgrade.Length)
        {
            bool upgradeState = _currentMoney >= _costPerUpgrade[_upgradeButtonIndex];
            _incomeUpgradeButtonComponent.interactable = upgradeState;
            _incomeUpgradeColorToggler.ToggleColor(upgradeState);
        }
    }

    private void UpdateButtonText()
    {
        //Reached max
        if (_costButtonIndex == _costPerBuy.Length)
        {
            _costText.text = "Ã¿ —.";
        }
        else
        {
            //  _costText.text = _costPerBuy[_costButtonIndex].ToString();
            _costText.text = CharMoney(_costPerBuy[_costButtonIndex]);
            
            if (_costButtonIndex < 17)
            {
                _rewardButtonText.text = "+1000 $";
            }
            else
            {
                _rewardButtonText.text = "+" + CharMoney(_costPerBuy[_costButtonIndex] * boostReward);
            }
        }


        //Reached max
        if (_upgradeButtonIndex == _costPerUpgrade.Length)
        {
            _upgradeTMP.text = "Ã¿ —.";
        }
        else
        {
            //_upgradeTMP.text = _costPerUpgrade[_upgradeButtonIndex].ToString();
            _upgradeTMP.text = CharMoney(_costPerUpgrade[_upgradeButtonIndex]);
        }

        if (_costButtonIndex > 0)
        {
            _buyButton_Free.SetActive(false);
            _buyButton.SetActive(true);
            _upgradeButton.SetActive(true);
        }
    }

    private void LoadSavedData()
    {
        _currentMoney = PlayerPrefs.GetFloat("_currentMoney", 0f);

        _multiplierIndex = PlayerPrefs.GetInt("_multiplierIndex", 0);
        _costButtonIndex = PlayerPrefs.GetInt("_costButtonIndex", 0);
        _upgradeButtonIndex = PlayerPrefs.GetInt("_upgradeButtonIndex", 0);

        if (_costButtonIndex > 0)
        {
            _buyButton_Free.SetActive(false);
            _buyButton.SetActive(true);
            _upgradeButton.SetActive(true);
        }

        UpdateButtonText();
    }

    private void SaveData()
    {
        PlayerPrefs.SetFloat("_currentMoney", _currentMoney);

        PlayerPrefs.SetInt("_multiplierIndex", _multiplierIndex);
        PlayerPrefs.SetInt("_costButtonIndex", _costButtonIndex);
        PlayerPrefs.SetInt("_upgradeButtonIndex", _upgradeButtonIndex);

        _leaderBoard.SaveData();
        _gameProgression.SaveData();
        _carLoader_Player.SaveData();
        _miniatureBoard.SaveData();

        PlayerPrefs.Save();
    }
}
