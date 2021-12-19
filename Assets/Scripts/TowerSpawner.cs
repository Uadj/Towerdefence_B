using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private int towerBuildGold = 50;
    [SerializeField]
    private PlayerGold playerGold;
    public void SpawnTower(Transform tileTransform)
    {
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();
        if (tile.IsBuilTower)
        {
            return;
        }
        tile.IsBuilTower = true;
        playerGold.CurrentGold -= towerBuildGold;
       // Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }

}
