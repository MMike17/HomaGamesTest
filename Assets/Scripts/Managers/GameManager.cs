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
	public ScoreManager scoreManager;

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
			shipController.transform.position.z,
			() => scoreManager.AddScore(levelManager.GetScorePerObstacle()),
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
				if(bonusState)
				{
					scoreManager.AddMultiplier();
					shipController.GetBonus();
				}
				else
					scoreManager.CancelMultiplier();

				shipController.UnlockInput();
			}
		);

		// TODO : Initialize managers
	}

	void InitializeOthers()
	{
		// TODO : Link end screen to player
		shipController.Init(() => Debug.Log("Pop end screen"));
	}
}