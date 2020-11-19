using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNamePresenter : MonoBehaviour
{
    public Player player;
    public TextMeshPro nameText;

    public virtual void Update()
    {
        nameText.text = player.PlayerName;
        nameText.transform.rotation = Camera.main.transform.rotation;
        var scale = nameText.transform.localScale;
        scale.x = player.transform.localScale.x;
        nameText.transform.localScale = scale;
    }
}
