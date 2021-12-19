using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
   
    public void OnAttackRange(Vector3 positon, float range)
    {
        gameObject.SetActive(true);
        float diameter = range * 2.0f;
        transform.localScale = Vector3.one * diameter;
        transform.position = positon;
    }
    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
}
