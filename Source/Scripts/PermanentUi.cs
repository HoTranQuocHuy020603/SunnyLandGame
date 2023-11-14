using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PermanentUi : MonoBehaviour
{
    public int cherries = 0;
    public float health = 15;
    public float healthMax = 15;

    public TextMeshProUGUI cherryText;
    public Image healthBar;

    public static PermanentUi perm;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (!perm)
        {
            perm = this; 
        }
        else
            Destroy(gameObject);
    }
    public void Resett()
    {
        health = 15;
        healthBar.fillAmount = health / healthMax; 
        cherries = 0;
        cherryText.text = cherries.ToString();
    }
}
