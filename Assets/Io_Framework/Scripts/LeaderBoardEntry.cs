using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardEntry : MonoBehaviour
{
    public Text PositionText;
    public Text NameText;
    public Text ScoreText;
    public Image image;

    private Color _normalColor;

    void Awake()
    {
        _normalColor = image.color;
    }

    public void Set(int position, string playerName, int score, bool isOwnScore)
    {
        PositionText.text = (position+1) + ".";
        NameText.text = playerName;
        ScoreText.text = score.ToString();
        SwitchHighlight(isOwnScore);
    }

    private void SwitchHighlight(bool value)
    {
        if (value)
        {
            image.color = new Color(0.5f, 0.8f, 0.5f);
        }
        else
        {
            image.color = _normalColor;
        }
    }
}
