using UnityEngine;
using static DataManager;

/// <summary>Manages all the game flow</summary>
public class GameManager : MonoBehaviour
{
	const string SAVE_FILE_NAME = "playerScore";

	[Header("Settings")]
	public LogLevel dataManagerLogLevel;
	public int maxScoreHistory;

	[Header("Managers")]
	public VFXManager vfxManager;
	public LevelManager levelManager;
	public TrackGenerationManager trackGenerationManager;
	public BonusManager bonusManager;
	public ScoreManager scoreManager;

	[Header("Scene references")]
	public ShipController shipController;

	PlayerScore playerScore;

	void Awake()
	{
		DataManager.SetLogLevel(dataManagerLogLevel);

		InitializeManagers();
		InitializeOthers();

		Debug.Log("Initialization done !");
	}

	void LoadLocalData()
	{
		if(DataManager.DoesFileExist(SAVE_FILE_NAME))
			playerScore = DataManager.LoadObjectAtPath<PlayerScore>(SAVE_FILE_NAME);
		else
			playerScore = new PlayerScore();
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
		shipController.Init(() =>
		{
			playerScore.scoresHistory.Add(scoreManager.GetCurrentScore());
			playerScore.TrimHistory(maxScoreHistory);

			DataManager.SaveObject(playerScore, SAVE_FILE_NAME);

			// TODO : Link end screen to player and give all score history
		});
	}
}