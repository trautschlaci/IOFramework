using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePresenter : MonoBehaviour
{
    public Player player;
    public TextMeshPro nameText;

    private int direction = 1;

    public virtual void Update()
    {
        nameText.text = player.PlayerName;
        nameText.transform.rotation = Camera.main.transform.rotation;
        var scale = nameText.transform.localScale;
        if (player.transform.localScale.x * direction < 0)
        {
            direction *= -1;
            scale.x *= -1;
        }
        nameText.transform.localScale = scale;
    }
}
