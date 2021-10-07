using UnityEngine;
using static DataManager;

/// <summary>Manages all the game flow</summary>
public class GameManager : MonoBehaviour
{
	[Header("Settings")]
	public LogLevel dataManagerLogLevel;

	[Header("Managers")]
	public VFXManager vfxManager;
	public LevelManager levelManager;
	public TrackGenerationManager trackGenerationManager;
	public BonusManager bonusManager;

	[Header("Scene references")]
	public ShipController shipController;

	void Awake()
	{
		DataManager.SetLogLevel(dataManagerLogLevel);

		InitializeManagers();
		InitializeOthers();

		Debug.Log("Initialization done !");
	}

	void Update()
	{
		DelayedActionsManager.Update(Time.deltaTime);

		// TEST
		if(Input.GetKeyDown(KeyCode.Backspace))
			shipController.StartShip();
		// TEST
	}

	void InitializeManagers()
	{
		vfxManager.Init();
		levelManager.Init(
			(min, max) => trackGenerationManager.SetDifficulty(Random.Range(min, max))
		);
		trackGenerationManager.Init(
			delay =>
			{
				bonusManager.SpawnBonus(delay);
				shipController.BlockInput();
			},
			() => { return levelManager.GetBonusPercent(); }
		);
		bonusManager.Init(
			bonusState =>
			{
				// TODO : Manage bonus state
				if(bonusState)
				{
					// increase score multiplier

					shipController.GetBonus();
				}
				else
				{
					// stop score multiplier
				}

				shipController.UnlockInput();
			}
		);

		// TODO : Initialize managers
	}

	void InitializeOthers()
	{
		shipController.Init(() => Debug.Log("Pop end screen"));
	}
}