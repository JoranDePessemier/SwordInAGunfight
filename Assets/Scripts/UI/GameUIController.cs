using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private Image[] _heartImages;

    [SerializeField]
    private Sprite _emptyHeart;

    [SerializeField]
    private Sprite _halfHeart;

    [SerializeField]
    private Sprite _fullHeart;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    public void SetHp(int hp)
    {
        int halfHp = hp / 2;
        int extraHp = hp % 2;

        for (int i = 0; i < _heartImages.Length; i++)
        {
            if(i < halfHp)
            {
                _heartImages[i].sprite = _fullHeart;
            }
            else if (i < halfHp + extraHp)
            {
                _heartImages[i].sprite = _halfHeart;
            }
            else
            {
                _heartImages[i].sprite = _emptyHeart;
            }
        }
    }

    public void SetScore(int score)
    {
        _scoreText.text = $"{score:0000}";
    }

    public void SetTimer(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);

        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
