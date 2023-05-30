using System.Collections;
using Tasks.Task_2;
using TMPro;
using UnityEngine;

public class KeypadTask : MonoBehaviour
{
    public CodeTask codeTask;
    
    public TextMeshProUGUI _cardCode;
    public TextMeshProUGUI _inputCode;
    public int _codeLength = 8;
    public float _codeResetTimeInSeconds = 0.5f;
    public bool _isResetting;

    private void OnEnable()
    {
        string code = string.Empty;
        for(int i = 0; i<_codeLength; i++)
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

        if (_inputCode.text == _cardCode.text.Substring(_cardCode.text.Length - _codeLength))
        {
            _inputCode.text = "CORRECT";
            StartCoroutine(ResetCode());
            codeTask.CorrectCode();

        }
        else if (_inputCode.text.Length >= _codeLength)
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
    }

}
