using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }
public enum WeaponType { Cannon = 0, Laser,}
public class TowerWeapon : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private TowerTemplate towerTemplate;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;
    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;
    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform hitEffect;
    [SerializeField]
    private LayerMask targetLayer;

    private Tile ownerTile;
    private int level = 0;
    public float Level => level + 1;
    /*[SerializeField]
    private float attackDamage = 1f;
    
    [SerializeField]
    private float attackRate = 0.5f;
    [SerializeField]
    private float attackRange = 2.0f;*/
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget;
    private EnemySpawner enemySpawner;
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int MaxLevel => towerTemplate.weapon.Length;
    private SpriteRenderer spriteRenderer;
    private PlayerGold  playerGold;
    /*public float Damage => attackDamage;
   
    public float Rate => attackRate;
    public float Range => attackRange;*/
    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        this.playerGold = playerGold;
        this.enemySpawner = enemySpawner;
        this.ownerTile = ownerTile;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeState(WeaponState.SearchTarget);
    }
    public void ChangeState(WeaponState newState)
    {
        
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        //Debug.Log(weaponState);
        StartCoroutine(weaponState.ToString());

    }
    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }
    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }
    private Transform FindClosestAttackTarget()
    {
      
        float closestDistSqr = Mathf.Infinity;
        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }
    
        return attackTarget;
        
    }
    private bool IsPossibleToAttackTarget()
    {
   
        if (attackTarget != null)
        {
            return false;
        }
        float distance = Vector3.Distance(attackTarget.transform.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }
        return true;
    }
    private IEnumerator SearchTarget()
    {
        while (true)
        {
            /*float closestDistSqr = Mathf.Infinity;
            for(int i=0; i < enemySpawner.EnemyList.Count; ++i)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }*/
            attackTarget = FindClosestAttackTarget();

            if (attackTarget != null)
            {
        
                if(weaponType == WeaponType.Cannon)
                { 
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType==WeaponType.Laser)
                {
                    
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }
            yield return null;
        }
    }
    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            /*if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            */
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
            SpawnProjectile();
        }
    }
    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while (true)
        {
           // Debug.Log("Start");
            if (IsPossibleToAttackTarget() == false)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }
           // Debug.Log("Spawn");
            SpawnLaser();
            yield return null;

        }
    }
    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }
    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }
    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);
        for(int i=0; i < hit.Length; i++)
        {
            if (hit[i].transform == attackTarget)
            {
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
            }
        }
    }
    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }
    public bool Upgrade()
    {
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }
        level++;
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;
        if(weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        return true;
    }
    public void Sell()
    {
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        ownerTile.IsBuilTower = false;
        Destroy(gameObject);
    }
}
