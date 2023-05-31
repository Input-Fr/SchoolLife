using System.Collections;
using Tasks.Task_2;
using TMPro;
using UnityEngine;

public class KeypadTask : MonoBehaviour
{
    public CodeTask codeTask;
    
    private const int CodeLength = 6;
    
    public TextMeshProUGUI _cardCode;
    public TextMeshProUGUI _inputCode;
    public float _codeResetTimeInSeconds = 0.5f;
    public bool _isResetting;

    private void OnEnable()
    {
        string code = string.Empty;
        for(int i = 0; i<CodeLength; i++)
        {
            code += Random.Range(1,10);
        }
        _cardCode.text = $"SECRET CODE : {code}";
        _inputCode.text = string.Empty;
    }

    public void ButtonClick(int number)
    {
        if (_isResetting) return;

        _inputCode.text += number;

        if (_inputCode.text == _cardCode.text.Substring(_cardCode.text.Length - CodeLength))
        {
            _inputCode.text = "CORRECT";
            StartCoroutine(ResetCode());

        }
        else if (_inputCode.text.Length >= CodeLength)
        {
            _inputCode.text = "FAILED";
            StartCoroutine(ResetCode());
        }
    }

    private IEnumerator ResetCode()
    {
        _isResetting = true;
        yield return new WaitForSeconds(_codeResetTimeInSeconds);
        _inputCode.text = string.Empty;
        _isResetting = false;
        codeTask.CorrectCode();
    }

}
