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
    public int sellPrice => towerTemplate.weapon[level].sell;
    public int upgradePrice => towerTemplate.weapon[level+1].cost;
    private SpriteRenderer spriteRenderer;
    private PlayerGold  playerGold;
    private bool isAttack = false;
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
       // Debug.Log("Stoped " + weaponState.ToString());
        weaponState = newState;
        //Debug.Log(weaponState);
        StartCoroutine(weaponState.ToString());
      //  Debug.Log("Started " + weaponState.ToString());

    }
    private void Update()
    {

        attackTarget = FindClosestAttackTarget();
        if (attackTarget != null)
        {
            RotateToTarget();
        }
        else if(!isAttack)
        {
            if(weaponType == WeaponType.Cannon)
                StartCoroutine("TryAttackCannon");
            else
                StartCoroutine("TryAttackLaser");
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
        isAttack = true;
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
            /*            if (IsPossibleToAttackTarget() == false)
                        {
                            ChangeState(WeaponState.SearchTarget);
                        }*/
            SpawnProjectile();
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
  
        }
    }
    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.transform.localScale = new Vector3(level + 1, level + 1, level + 1);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }
    private IEnumerator TryAttackLaser()
    {
        EnableLaser();
        isAttack = true;
        while (true)
        {
            // Debug.Log("Start");
            if (attackTarget==null)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                isAttack = false;
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
                
                if (level == 1) {
                    Color c = lineRenderer.material.color;
                    c.r = 0;
                    c.g = 255;
                    c.b = 0;
                    lineRenderer.startColor = c;
                    lineRenderer.endColor = c;
                }
                if(level == 2)
                {
                    Color c = lineRenderer.material.color;
                    c.r = 155;
                    c.g = 0;
                    c.b = 255;
                    lineRenderer.startColor = c;
                    lineRenderer.endColor = c;
                }
                
                lineRenderer.startWidth = level*0.01f +0.05f;
                lineRenderer.endWidth = level*0.1f + 0.05f;
                hitEffect.localScale = new Vector3(1 + level * 0.2f, 1 + level * 0.2f, 1 + level * 0.2f);
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
            }
        }
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
