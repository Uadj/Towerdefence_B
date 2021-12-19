using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    private float maxHP = 20;
    private float currentHP;
    [SerializeField]
    private Image redScreen;
    public float MaxHP => maxHP;
    public float CurrentHp => currentHP;
    private void Awake()
    {
        currentHP = maxHP;

    }
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");
        if (currentHP <= 0)
        {

        }
    }
    private IEnumerator HitAlphaAnimation()
    {
        Color color = redScreen.color;
        color.a = 0.4f;
        redScreen.color = color;
        while(color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            redScreen.color = color;
            yield return null;
        }
    }
}
