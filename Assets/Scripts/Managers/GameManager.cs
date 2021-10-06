using UnityEngine;
using static DataManager;

/// <summary>Manages all the game flow</summary>
public class GameManager : MonoBehaviour
{
	[Header("Settings")]
	public LogLevel dataManagerLogLevel;

	// [Header("Managers")]
	[Header("Scene references")]
	public ShipController shipController;

	void Awake()
	{
		DataManager.SetLogLevel(dataManagerLogLevel);

		InitializeManagers();
		InitializeOthers();

		Debug.Log("Initialization done !");
	}

	// TEST
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Backspace))
			shipController.StartShip();
	}
	// TEST

	void InitializeManagers()
	{
		// TODO : Initialize managers
	}

	void InitializeOthers()
	{
		shipController.Init();
	}
}