using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _textMeshProHealth;
    [SerializeField] private TextMeshProUGUI _textMeshProWave;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.wave += SetTextWave;
        CharacterController.health_txt += SetTextHealth;
    }
    public void SetTextWave(int number)
    {
        _textMeshProWave.text = "Wave: " + number;
    }
    public void SetTextHealth(int number)
    {
        _textMeshProHealth.text = "Health: " + number;
    }
}
