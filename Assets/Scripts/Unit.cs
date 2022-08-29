using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool selected;
    private GameMaster gm;

    public int tileSpeed;
    public bool hasMoved;

    public float moveSpeed;
    public int playerNumber;

    public int attackRange;
    private List<Unit> enemiesInRange = new List<Unit>();
    public bool hasAttacked;

    public GameObject weaponIcon;

    public int health;
    public int attackDamage;
    public int defenceDamage;
    public int armor;


    public DamageIcon damageIcon;

    private Animator camAnim;

    public Text kingHealth;
    public bool isKing;

    private AudioSource sound;
    public AudioClip moveSound;



    void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        sound = FindObjectOfType<AudioSource>();
        camAnim = Camera.main.GetComponent<Animator>();
        UpdateKingHealth();
    }

    public void UpdateKingHealth()
    {
        if(isKing == true)
        {
            kingHealth.text = health.ToString();
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.ToggleStatsPanel(this);
        }
    }

    private void OnMouseDown()
    {
        ResetWeaponIcons();
        if (selected)
        {
            gm.selectedUnit = null;
            selected = false;
            gm.ResetTiles();
        }
        else
        {
            if (playerNumber == gm.playerTurn)
            {
                if (gm.selectedUnit != null)
                {
                    gm.selectedUnit.selected = false;
                }
                gm.selectedUnit = this;
                selected = true;
                GetEnemies();
                GetWalkableTiles();
            }
        }

        Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
        Unit unit = col.GetComponent<Unit>();
        if(gm.selectedUnit != null)
        {
            if(gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked)
            {
                gm.selectedUnit.Attack(unit);
            }
        }

    }

    private void Attack(Unit enemy)
    {
        camAnim.SetTrigger("shake");

        hasAttacked = true;

        int enemyDamage = this.attackDamage - enemy.armor;
        int myDamage = enemy.defenceDamage - this.armor;

        if (enemyDamage >= 1)
        {
            DamageIcon instance = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            instance.Setup(enemyDamage);
            enemy.health -= enemyDamage;
            enemy.UpdateKingHealth();
        }
        if (myDamage>= 1 && Mathf.Abs(enemy.transform.position.x - this.transform.position.x) + Mathf.Abs(enemy.transform.position.y - this.transform.position.y) < 2)
        {
            DamageIcon instance = Instantiate(damageIcon, this.transform.position, Quaternion.identity);
            instance.Setup(myDamage);
            this.health -= myDamage;
            UpdateKingHealth();
        }

        if (enemy.health <= 0)
        {
            if (enemy.isKing == true)
            {
                if (enemy.playerNumber == 1)
                {
                    gm.victoryPanel2.SetActive(true);
                }
                else
                {
                    gm.victoryPanel1.SetActive(true);
                }
            }
            Destroy(enemy.gameObject);
            gm.RemoveStatsPanel(enemy);
            GetWalkableTiles();
        }
        if (this.health <= 0)
        {
            if (isKing == true)
            {
                if(playerNumber == 1)
                {
                    gm.victoryPanel2.SetActive(true);
                }
                else
                {
                    gm.victoryPanel1.SetActive(true);
                }
            }
            gm.ResetTiles();
            gm.RemoveStatsPanel(this);
            Destroy(this.gameObject);
        }

        //攻击后更新状态面板
        gm.UpdateStatsPanel();
    }


    private void GetWalkableTiles()
    {
        if (hasMoved)
        {
            return;
        }

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
            if (Mathf.Abs(tile.transform.position.x - this.transform.position.x) + Mathf.Abs(tile.transform.position.y - this.transform.position.y) <= tileSpeed)
            {
                if (tile.IsClear())
                {
                    tile.Highlight();
                }
            }
        }
    }

    private void GetEnemies()
    {
        enemiesInRange.Clear();

        if (hasAttacked)
        {
            return;
        }

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (Mathf.Abs(unit.transform.position.x - this.transform.position.x) + Mathf.Abs(unit.transform.position.y - this.transform.position.y) <= attackRange)
            {
                if (unit.playerNumber != this.playerNumber)
                {
                    enemiesInRange.Add(unit);
                    unit.weaponIcon.SetActive(true);
                }
            }
        }
    }

    public void ResetWeaponIcons()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.weaponIcon.SetActive(false);
        }
    }



    public void Move(Vector2 tilePos)
    {
        StartCoroutine(StartMovemant(tilePos));
        sound.clip = moveSound;
        sound.Play();
        gm.ResetTiles();
    }

    private IEnumerator StartMovemant(Vector2 tilePos)
    {
        ResetWeaponIcons();
        while (transform.position.x != tilePos.x)
        {
            print("Test2!");
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(tilePos.x, transform.position.y, transform.position.z), moveSpeed * Time.deltaTime);
            yield return null;
        }
        while(transform.position.y != tilePos.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, tilePos.y, transform.position.z), moveSpeed * Time.deltaTime);
            yield return null;
        }
        hasMoved = true;
        GetEnemies();
        gm.MoveStatsPanel(this);
    }

}
