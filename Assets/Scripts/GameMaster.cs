using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public Unit selectedUnit;
    public int playerTurn;

    public GameObject selectedSquare;

    public Image playerIndicator;
    public Sprite player1Indicator;
    public Sprite player2Indicator;

    public int player1Gold = 100;
    public int player2Gold = 100;

    public Text player1GoldText;
    public Text player2GoldText;

    public BarrackItem purchasedItem;

    public GameObject statsPanel;
    public Vector2 statsPanelShift;
    public Unit viewedUnit;

    public Text healthText;
    public Text armorText;
    public Text attackDamageText;
    public Text defenseDamageText;


    public GameObject victoryPanel1;
    public GameObject victoryPanel2;

    public void ToggleStatsPanel(Unit unit)
    {
        if (unit.Equals(viewedUnit) == false)
        {
            statsPanel.SetActive(true);
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
            viewedUnit = unit;
            UpdateStatsPanel();
        }
        else
        {
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }

    public void UpdateStatsPanel()
    {
        if(viewedUnit!= null)
        {
            healthText.text = viewedUnit.health.ToString();
            armorText.text = viewedUnit.armor.ToString();
            attackDamageText.text = viewedUnit.attackDamage.ToString();
            defenseDamageText.text = viewedUnit.defenceDamage.ToString();
        }
    }

    public void MoveStatsPanel(Unit unit)
    {
        if (unit.Equals(viewedUnit))
        {
            statsPanel.transform.position = (Vector2)unit.transform.position + statsPanelShift;
        }
    }

    public void RemoveStatsPanel(Unit unit)
    {
        if (!unit.Equals(viewedUnit))
        {
            statsPanel.SetActive(false);
            viewedUnit = null;
        }
    }


    public void ResetTiles() 
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
        }
    }
    
    public void UpdateGoldText()
    {
        if (player1GoldText != null)
        {
            player1GoldText.text = player1Gold.ToString();
        }
        if (player2GoldText != null)
        {
            player2GoldText.text = player2Gold.ToString();
        }
    }

    private void GetGoldIncome(int playerTurn)
    {
        foreach(Village village in FindObjectsOfType<Village>())
        {
            if(village.playerNumber == playerTurn)
            {
                if(playerTurn == 1)
                {
                    player1Gold += village.goldePerTurn;
                }
                else
                {
                    player2Gold += village.goldePerTurn;
                }
            }
        }
        UpdateGoldText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if(selectedUnit != null)
        {
            selectedSquare.transform.position = selectedUnit.transform.position;
            selectedSquare.SetActive(true);
        }
        else
        {
            selectedSquare.SetActive(false);
        }
    }

    private void EndTurn()
    {
        //回合结束 结算该玩家金币
        GetGoldIncome(playerTurn);

        if(playerTurn == 1)
        {
            playerTurn = 2;
            playerIndicator.sprite = player2Indicator;
        }
        else if(playerTurn == 2)
        {
            playerTurn = 1;
            playerIndicator.sprite = player1Indicator;
        }

        selectedUnit = null;

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasMoved = false;
            unit.selected = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }
        ResetTiles();

        GetComponent<Barrack>().CloseMenus();
        
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
