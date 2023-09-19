using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    public Button button;
    private bool isButtonDisabled = false;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void DisableButtonFor60Seconds()
    {
        if (!isButtonDisabled)
        {
            button.interactable = false;
            isButtonDisabled = true;
            Invoke("EnableButton", 60f);
        }
    }

    private void EnableButton()
    {
        button.interactable = true;
        isButtonDisabled = false;
    }
}