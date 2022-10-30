using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIPage
{
    [SerializeField]
    private TMPro.TMP_Text ScoreText;

    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public void Pause()
    {
        Game.Instance.Pause();
    }

    private void Update()
    {
        ScoreText.text = string.Format("{0:n0}", Game.Instance.Score);
    }
}
