using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncomeGainUI : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _initialSize = 10;
    [SerializeField] private float _timeDuration = 1;
    [SerializeField] private float _upSpeed = 1;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    private List<GameObject> _pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            GameObject obj = Instantiate(_prefab, transform);
            obj.SetActive(false);
            _pool.Add(obj);
        }

        _prefab.SetActive(false);
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in _pool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.position = _prefab.transform.position;
                obj.transform.SetAsLastSibling();
                return obj;
            }
        }

        GameObject newObj = Instantiate(_prefab, transform);
        newObj.SetActive(true);
        newObj.transform.SetAsLastSibling();
        _pool.Add(newObj);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ShowIncomeGainElement(string text)
    {
        var newItem = GetObject();
        TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
        tmp.text = text+ "$";
        StartCoroutine(Process(tmp, _timeDuration));
    }

    private IEnumerator Process(TextMeshProUGUI tmpObj, float time)
    {
        float timeUsed = 0f;
        tmpObj.color = startColor;
        while (timeUsed < time)
        {
            // Move the object upwards by adding a small amount to its y position each frame
            tmpObj.transform.position += Vector3.up * Time.deltaTime * _upSpeed;
            tmpObj.color = Color.Lerp(startColor, endColor, timeUsed / time);
            timeUsed += Time.deltaTime;
            yield return null;
        }

        // do something to the object
        //yield return new WaitForSeconds(time);
        ReturnObject(tmpObj.gameObject);
    }
}
