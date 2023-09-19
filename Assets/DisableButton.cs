using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    public GameObject buttonObject;
    private bool isButtonDisabled = false;

    public void DisableButtonFor60Seconds()
    {
        if (!isButtonDisabled)
        {
            buttonObject.SetActive(false);
            isButtonDisabled = true;
            Invoke("EnableButton", 60f);
        }
    }

    private void EnableButton()
    {
        buttonObject.SetActive(true);
        isButtonDisabled = false;
    }
}
