using UnityEngine;
using static DataManager;

/// <summary>Manages all the game flow</summary>
public class GameManager : MonoBehaviour
{
	const string SAVE_FILE_NAME = "playerScore";

	[Header("Settings")]
	public LogLevel dataManagerLogLevel;
	public bool isBuild;
	public int maxScoreHistory;

	[Header("Managers")]
	public VFXManager vfxManager;
	public LevelManager levelManager;
	public TrackGenerationManager trackGenerationManager;
	public BonusManager bonusManager;
	public ScoreManager scoreManager;
	public MainMenuManager mainMenuManager;
	public EndScreenManager endScreenManager;

	[Header("Scene references")]
	public ShipController shipController;

	bool newHigh;

	PlayerScore playerScore;

	void Awake()
	{
		DataManager.SetLogLevel(dataManagerLogLevel);
		DataManager.SetRealease(isBuild);

		newHigh = false;

		LoadLocalData();

		InitializeManagers();
		InitializeOthers();

		Debug.Log("Initialization done !");
	}

	void LoadLocalData()
	{
		if(DataManager.DoesFileExist(SAVE_FILE_NAME))
			playerScore = DataManager.LoadObjectAtPath<PlayerScore>(SAVE_FILE_NAME);
		else
		{
			playerScore = new PlayerScore();
			DataManager.SaveObject(playerScore, SAVE_FILE_NAME);
		}
	}

	void Update()
	{
		DelayedActionsManager.Update(Time.deltaTime);
	}

	void InitializeManagers()
	{
		vfxManager.Init();
		trackGenerationManager.Init(
			shipController.transform.position.z,
			() =>
			{
				shipController.PassObstacle();
				scoreManager.AddScore(levelManager.GetScorePerObstacle());

				if(!newHigh)
				{
					vfxManager.PassedObstacleAnim();

					if(playerScore.highscore != 0 && scoreManager.GetCurrentScore() > playerScore.highscore)
					{
						vfxManager.NewHighAnim();
						newHigh = true;
					}
				}
			},
			() => levelManager.BlendToNewLevel(),
			(duration, opportunityWindowSize) =>
			{
				bonusManager.SpawnBonus(duration, opportunityWindowSize);
				shipController.BlockInput();
			},
			() => { return levelManager.GetBonusPercent(); }
		);
		levelManager.Init(
			(min, max) => trackGenerationManager.SetDifficulty(Random.Range(min, max))
		);
		bonusManager.Init(
			bonusState =>
			{
				if(bonusState)
				{
					if(!newHigh)
						vfxManager.BonusAnim();

					scoreManager.AddMultiplier();
					shipController.GetBonus();
				}
				else
					scoreManager.CancelMultiplier();

				shipController.UnlockInput();
			}
		);
		scoreManager.Init();
		mainMenuManager.Init(
			() =>
			{
				shipController.StartShip();
				trackGenerationManager.StartGame();
			},
			playerScore.highscore
		);
		endScreenManager.Init(
			maxScoreHistory,
			() =>
			{
				endScreenManager.HidePanel();

				scoreManager.StartGame();
				trackGenerationManager.StartGame();
				shipController.StartShip();

				newHigh = false;
			}
		);
	}

	void InitializeOthers()
	{
		shipController.Init(() =>
		{
			trackGenerationManager.PauseGame();

			float lastHighScore = playerScore.highscore;

			playerScore.AddScore(scoreManager.GetCurrentScore(), maxScoreHistory);
			DataManager.SaveObject(playerScore, SAVE_FILE_NAME);

			endScreenManager.DisplayData(playerScore.scoresHistory, lastHighScore);
			vfxManager.ResetEffects();
		});
	}
}