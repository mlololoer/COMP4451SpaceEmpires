using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject special_attack;
	public Transform playerBattleStation;
	public Transform enemyBattleStation;
	public List<int> enemymoves;
	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		PureUnit player = CrossSceneManager.playerUnit;
		PureUnit ai = CrossSceneManager.aiUnit;
		if (player == null || ai == null) {
			Debug.Log("Null passed to battle system");
		}
		StartCoroutine(SetupBattle(player, ai));
		enemymoves = new List<int>();
	}

	IEnumerator SetupBattle(PureUnit player, PureUnit ai)
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		playerUnit.unitName = player.unitName;
		playerUnit.unitClass = player.unitClass;
		playerUnit.hlth_upgrade = player.hlth_upgrade;
		playerUnit.dmg_upgrade = player.dmg_upgrade;
		playerUnit.damage = player.damage;
		playerUnit.maxHP = player.maxHP;
		playerUnit.currentHP = player.currentHP;

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();
		enemyUnit.unitName = ai.unitName;
		enemyUnit.unitClass = ai.unitClass;
		enemyUnit.hlth_upgrade = ai.hlth_upgrade;
		enemyUnit.dmg_upgrade = ai.dmg_upgrade;
		enemyUnit.damage = ai.damage;
		enemyUnit.maxHP = ai.maxHP;
		enemyUnit.currentHP = ai.currentHP;

		dialogueText.text = "An enemy " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);
		if (playerUnit.unitClass == ShipType.FRIGATE)
        {
			special_attack.GetComponent<Text>().text = "Rapid Fire";
		}
		else if (playerUnit.unitClass == ShipType.DESTROYER)
		{
			special_attack.GetComponent<Text>().text = "Rail Gun";
		}
		else if (playerUnit.unitClass == ShipType.CRUISER)
		{
			special_attack.GetComponent<Text>().text = "Barrage";
		}

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		//depth of two as not to be too hard for the player
		//as such highest value is considered having higher health
		int PspecAVG = 0;
		int AIspecAVG = 0;
		int PCQBSD = 0;
		int AICQBSD = 0;
		if (playerUnit.unitClass == ShipType.FRIGATE)
		{
            if (playerUnit.dmg_upgrade == true)
            {
				PspecAVG = 20;
				PCQBSD = 7; 
			}
            else
            {
				PspecAVG = 15;
				PCQBSD = 2;

			}
		}
		else if (playerUnit.unitClass == ShipType.DESTROYER)
		{
			if (playerUnit.dmg_upgrade == true)
			{
				PspecAVG = 45;
				PCQBSD = 17;
			}
			else
			{
				PspecAVG = 37;
				PCQBSD = 12;
			}
		}
		else if (playerUnit.unitClass == ShipType.CRUISER)
		{
			if (playerUnit.dmg_upgrade == true)
			{
				PspecAVG = 34;
				PCQBSD = 12;
			}
			else
			{
				PspecAVG = 27;
				PCQBSD = 7;
			}
		}
		if (enemyUnit.unitClass == ShipType.FRIGATE)
		{
			if (enemyUnit.dmg_upgrade == true)
			{
				AIspecAVG = 20;
				AICQBSD = 7;
			}
			else
			{
				AIspecAVG = 15;
				AICQBSD = 2;
			}
		}
		else if (enemyUnit.unitClass == ShipType.DESTROYER)
		{
			if (enemyUnit.dmg_upgrade == true)
			{
				AIspecAVG = 45;
				AICQBSD = 17;
			}
			else
			{
				AIspecAVG = 37;
				AICQBSD = 12;
			}
		}
		else if (enemyUnit.unitClass == ShipType.CRUISER)
		{
			if (enemyUnit.dmg_upgrade == true)
			{
				AIspecAVG = 34;
				AICQBSD = 12;
			}
			else
			{
				AIspecAVG = 27;
				AICQBSD = 7;
			}
		}


		//value rounded up for ease of life
		int P1 = playerUnit.currentHP - AIspecAVG;
		int P2 = playerUnit.currentHP - enemyUnit.damage;
		int P3a = enemyUnit.currentHP - AICQBSD;
		int P3b = playerUnit.currentHP - (int)(1.5 * enemyUnit.damage);
		//first layer
		int P11 = 0;
		int P12 = 0;
		int P13a = 0;
		int P13b = 0;
		int P21 = 0;
		int P22 = 0;
		int P23a = 0;
		int P23b = 0;
		int P31 = 0;
		int P32 = 0;
		int P33a = 0;
		int P33b = 0;
		int P1val = 0;
		int P2val = 0;
		int P3val = 0;
		int P11val = 0;
		int P12val = 0;
		int P13val = 0;
		int P21val = 0;
		int P22val = 0;
		int P23val = 0;
		int P31val = 0;
		int P32val = 0;
		int P33val = 0;
		//second
		if (P1 > 0)
        {
			P11 = enemyUnit.currentHP - PspecAVG;
			P12 = enemyUnit.currentHP - playerUnit.damage;
			P13a = P1 - PCQBSD;
			P13b = enemyUnit.currentHP - (int)(1.5 * playerUnit.damage);
			if (P11 < 1) { P11val = -1000; }
			else { P11val = P11 - P1; }
			if (P12 < 1) { P12val = -1000; }
			else { P12val = P12 - P1; }
			if (P13b < 1) { P13val = -1000; }
			else if (P13a < 1) { P13val = 1000; }
			else { P13val = P13b - P13a; }

			if ((P13val < P12val) && (P13val < P11val)) { P1val = P13val; }
			else if ((P12val < P13val) && (P12val < P11val)) { P1val = P12val; }
			else { P1val = P11val; }
			if (enemymoves.Count != 0) 
			{
				if (enemymoves[enemymoves.Count - 1] == 1) { P1val = P1val - 10; }
				if (enemymoves.Count > 1){ if (enemymoves[enemymoves.Count - 2] == 1) { P1val = P1val - 5; } }
			}


		}
		else
        {
			P1val = 1000;
		}
		if (P2 > 0)
        {
			P21 = enemyUnit.currentHP - PspecAVG;
			P22 = enemyUnit.currentHP - playerUnit.damage;
			P23a = P2 - PCQBSD;
			P23b = enemyUnit.currentHP - (int)(1.5 * playerUnit.damage);
			if (P21 < 1) { P21val = -1000; }
			else { P21val = P21 - P2; }
			if (P22 < 1) { P22val = -1000; }
			else { P22val = P22 - P2; }
			if (P23b < 1) { P23val = -1000; }
			else if (P23a < 1) { P23val = 1000; }
			else { P23val = P23b - P23a; }

			if ((P23val < P22val) && (P23val < P21val)){ P2val = P23val; }
			else if ((P22val < P23val) && (P22val < P21val)){ P2val = P22val; }
			else { P2val = P21val; }
			if (enemymoves.Count != 0) 
			{
				if (enemymoves[enemymoves.Count - 1] == 2) { P2val = P2val - 10; }
				if (enemymoves.Count > 1) { if (enemymoves[enemymoves.Count - 2] == 2) { P2val = P2val - 5; } }
			}


		}
		else
        {
			P2val = 1000;
		}
		if ((P3a > 0) && (P3b > 0))
        {
			P31 = P3a - PspecAVG;
			P32 = P3a - playerUnit.damage;

			P33a = P3b - PCQBSD;
			P33b = P3a - (int)(1.5 * playerUnit.damage);
			if (P31 < 1) { P31val = -1000; }
			else { P31val = P31 - P3b; }
			if (P32 < 1) { P32val = -1000; }
			else { P32val = P32 - P3b; }
			if (P33b < 1) { P33val = -1000; }
			else if (P33a < 1) { P33val = 1000; }
			else { P33val = P33b - P33a; }

			if ((P33val < P32val) && (P33val < P31val)){ P3val = P33val; }
			else if ((P32val < P33val) && (P32val < P31val)){ P3val = P32val; }
			else { P3val = P31val; }
            if (enemymoves.Count != 0) 
			{ 
				if(enemymoves[enemymoves.Count - 1] == 3) { P3val = P3val - 10; }
				if (enemymoves.Count > 1) { if (enemymoves[enemymoves.Count - 2] == 3) { P3val = P3val - 5; } }
			}
		}
		else if ((P3a < 1)||(enemyUnit.currentHP < (enemyUnit.maxHP/4)))
		{
			P3val = -1000;
        }
		else if (P3b < 1)
        {
			P3val = 1000;
        }
		bool isDead1 = false;
		bool isDead = false;
		if ((P3val > P2val) && (P3val > P1val))
		{
			dialogueText.text = "They have approached our broadside";
			yield return new WaitForSeconds(1f);
			if (enemyUnit.unitClass == ShipType.FRIGATE)
			{
				isDead = playerUnit.TakeDamage((int)(1.5 * enemyUnit.damage));
				if (enemyUnit.dmg_upgrade)
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(5, 9));
				}
				else
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(0, 4));
				}
			}
			if (enemyUnit.unitClass == ShipType.DESTROYER)
			{
				isDead = playerUnit.TakeDamage((int)(1.5 * enemyUnit.damage));
				if (enemyUnit.dmg_upgrade)
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(15, 19));
				}
				else
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(10, 14));
				}
			}
			if (enemyUnit.unitClass == ShipType.CRUISER)
			{
				isDead = playerUnit.TakeDamage((int)(1.5 * enemyUnit.damage));
				if (enemyUnit.dmg_upgrade)
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(10, 14));
				}
				else
				{
					isDead1 = enemyUnit.TakeDamage(Random.Range(5, 9));
				}
			}
			enemymoves.Add(3);
			enemyHUD.SetHP(enemyUnit.currentHP);
			playerHUD.SetHP(playerUnit.currentHP);
		}
		else if ((P2val > P3val) && (P2val > P1val))
		{
			dialogueText.text = enemyUnit.unitName + " attacks!";

			yield return new WaitForSeconds(1f);

			isDead = playerUnit.TakeDamage(enemyUnit.damage);
			enemymoves.Add(2);
			playerHUD.SetHP(playerUnit.currentHP);
		}
		else
		{
			if (enemyUnit.unitClass == ShipType.FRIGATE)
			{
				if (enemyUnit.dmg_upgrade == false)
				{
					isDead = playerUnit.TakeDamage(Random.Range(11, 20));
				}
				else
				{
					isDead = playerUnit.TakeDamage(Random.Range((21), (30)));
				}
				dialogueText.text = "Rapid attack was performed on us";
			}
			else if (enemyUnit.unitClass == ShipType.DESTROYER)
			{
				if (enemyUnit.dmg_upgrade == false)
				{
					int prob = Random.Range(0, 99);
					if (prob > 24)
					{
						isDead = playerUnit.TakeDamage(50);
						dialogueText.text = "Railgun shot us";
					}
					else
					{
						dialogueText.text = "Railgun just missed us";
					}
				}
				else
				{
					int prob = Random.Range(0, 99);
					if (prob > 39)
					{
						isDead = playerUnit.TakeDamage(75);
						dialogueText.text = "Railgun shot at us";
					}
					else
					{
						dialogueText.text = "Railgun barely missed us";
					}
				}
			}
			else if (enemyUnit.unitClass == ShipType.CRUISER)
			{
				if (enemyUnit.dmg_upgrade == false)
				{
					int count = 0;
					for (int i = 0; i < 6; i++)
					{
						int prob = Random.Range(0, 99);
						if (prob > 9)
						{
							count += 1;
						}
					}
					isDead = playerUnit.TakeDamage((5 * count));
					dialogueText.text = "They fired 6 missiles and" + count + " hit us.";
				}
				else
				{
					int count = 0;
					for (int i = 0; i < 8; i++)
					{
						int prob = Random.Range(0, 99);
						if (prob > 14)
						{
							count += 1;
						}
					}
					isDead = playerUnit.TakeDamage((5 * count));
					dialogueText.text = "They fired 8 missiles and" + count + " hit the target.";
				}
			
			}
			enemymoves.Add(1);
			yield return new WaitForSeconds(1f);
		}
		enemyHUD.SetHP(enemyUnit.currentHP);
		playerHUD.SetHP(playerUnit.currentHP);
		yield return new WaitForSeconds(1f);

		if (isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

	}
	
	IEnumerator PlayerSpecial()
	{
		bool isDead1 = false;
		if (playerUnit.unitClass == ShipType.FRIGATE)
		{
			if (playerUnit.dmg_upgrade == false)
            {
				isDead1 = enemyUnit.TakeDamage(Random.Range(11, 20));
			}
            else
            {
				isDead1 = enemyUnit.TakeDamage(Random.Range((16), (25)));
			}
			dialogueText.text = "Rapid attack was performed";

		}
		else if (playerUnit.unitClass == ShipType.DESTROYER)
		{
			if (playerUnit.dmg_upgrade == false)
			{		
				int prob = Random.Range(0, 99);
				if (prob > 24)
                {
					isDead1 = enemyUnit.TakeDamage(50);
					dialogueText.text = "Railgun shot the target";
				}
				else
				{
					dialogueText.text = "Railgun missed the target";
				}
			}
			else
			{
				int prob = Random.Range(0, 99);
				if (prob > 39)
				{
					isDead1 = enemyUnit.TakeDamage(75);
					dialogueText.text = "Railgun shot the target";
				}
                else
                {
					dialogueText.text = "Railgun missed the target";
				}
			}
		}
		else if (playerUnit.unitClass == ShipType.CRUISER)
		{
			if (playerUnit.dmg_upgrade == false)
			{
				int count = 0;
				for (int i = 0; i < 6; i++)
				{
					int prob = Random.Range(0, 99);
					if (prob > 9)
                    {
						count += 1;
                    }
				}
				isDead1 = enemyUnit.TakeDamage((5 * count));
				dialogueText.text = "6 Missiles Fired and" + count + " hit the target.";
			}
			else
			{
				int count = 0;
				for (int i = 0; i < 8; i++)
				{
					int prob = Random.Range(0, 99);
					if (prob > 14)
					{
						count += 1;
					}
				}
				isDead1 = enemyUnit.TakeDamage((5 * count));
				dialogueText.text = "8 Missiles Fired and" + count + " hit the target.";
			}
		}

		enemyHUD.SetHP(enemyUnit.currentHP);


		yield return new WaitForSeconds(2f);

		if (isDead1)
		{
			state = BattleState.WON;
			EndBattle();
		}
		else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}


	void EndBattle()
	{
		CrossSceneManager.battleFinished = true;
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
			CrossSceneManager.battleOutcome = true;
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
			CrossSceneManager.battleOutcome = false;
		}
		SceneManager.LoadScene ("Main");
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		if (playerUnit.hlth_upgrade == false)
		{
			playerUnit.Heal(5);
		}
        else
        {
			playerUnit.Heal(10);
		}
		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "The fleet has been repaired!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}
	IEnumerator PlayerCQB()
    {
		bool isDead = false;
		bool isDead1 = false;
		yield return new WaitForSeconds(1f);
		if (playerUnit.unitClass == ShipType.FRIGATE)
		{
			isDead = enemyUnit.TakeDamage((int)(1.5 * playerUnit.damage));
			if (playerUnit.dmg_upgrade)
			{
				isDead1 = enemyUnit.TakeDamage(Random.Range(5, 9));
			}
			else
			{
				isDead1 = enemyUnit.TakeDamage(Random.Range(0, 4));
			}
		}
		if (playerUnit.unitClass == ShipType.DESTROYER)
		{
			isDead = enemyUnit.TakeDamage((int)(1.5 * playerUnit.damage));
			if (playerUnit.dmg_upgrade)
			{
				isDead1 = playerUnit.TakeDamage(Random.Range(15, 19));
			}
			else
			{
				isDead1 = playerUnit.TakeDamage(Random.Range(10, 14));
			}
		}
		if (playerUnit.unitClass == ShipType.CRUISER)
		{
			isDead = enemyUnit.TakeDamage((int)(1.5 * playerUnit.damage));
			if (playerUnit.dmg_upgrade)
			{
				isDead1 = playerUnit.TakeDamage(Random.Range(10, 14));
			}
			else
			{
				isDead1 = playerUnit.TakeDamage(Random.Range(5, 9));
			}
		}
		dialogueText.text = "We have entered close quarters";
		enemyHUD.SetHP(enemyUnit.currentHP);
		playerHUD.SetHP(playerUnit.currentHP);


		yield return new WaitForSeconds(2f);

		if (isDead)
		{
			state = BattleState.WON;
			EndBattle();
		}
		else if (isDead1)
		{
			state = BattleState.LOST;
			EndBattle();
		}
		else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}
	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

	public void OnRamButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerSpecial());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}
	public void OnBroadsideButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerCQB());
	}



}
