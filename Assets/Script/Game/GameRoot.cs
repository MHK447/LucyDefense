using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BanpoFri;

public class GameRoot : Singleton<GameRoot>
{
	[SerializeField]
	private Transform MainUITrans;
	[SerializeField]
	private Transform HUDUITrans;
	[SerializeField]
	public Canvas MainCanvas;
	[SerializeField]
	private Canvas WorldCanvas;
	[SerializeField]
	private GameObject CheatWindow;
	[HideInInspector]
	public LoadingBasic Loading;
	[SerializeField]
	private AdManager AdManager;

	public RectTransform GetMainCanvasTR { get { return MainCanvas.transform as RectTransform; } }
	public UISystem UISystem { get; private set; } = new UISystem();
	public UserDataSystem UserData { get; private set; } = new UserDataSystem();
	public InGameSystem InGameSystem { get; private set; } = new InGameSystem();
	public SnapshotCamera SnapshotCam { get; private set; } = null;
	public PluginSystem PluginSystem { get; private set; } = new PluginSystem();
	public PlayTimeSystem PlayTimeSystem { get; private set; } = new PlayTimeSystem();
	public EffectSystem EffectSystem { get; private set; } = new EffectSystem();
	public TutorialSystem TutorialSystem { get; private set; } = new TutorialSystem();
	public InGameBattleSystem InGameBattleSystem { get; private set; } = new InGameBattleSystem();
	public InGameUnitUpgradeSystem UnitUpgradeSystem { get; private set; } = new InGameUnitUpgradeSystem();
	public UnitSkillSystem UnitSkillSystem { get; private set; } = new UnitSkillSystem();
	public SkillCardSystem SkillCardSystem { get; private set; } = new SkillCardSystem();
	public OutGameUnitUpgradeSystem OutGameUnitUpgradeSystem { get; private set; } = new OutGameUnitUpgradeSystem();
	public SelectGachaWeaponSkillSystem GachaSkillSystem { get; private set; } = new SelectGachaWeaponSkillSystem();


	public AdManager GetAdManager { get { return AdManager; } }

	public GameObject UILock;
	private static bool InitTry = false;
	public static bool LoadComplete { get; private set; } = false;
	private float deltaTime = 0f;
	public InGameType CurInGameType { get; private set; } = InGameType.Main;
	private Queue<System.Action> TouchStartActions = new Queue<System.Action>();
	public Queue<System.Action> TitleCloseActions = new Queue<System.Action>();

	private int loadcount = 0;

	public static bool IsInit()
	{
		if (instance != null && !InitTry)
			Load();

		return instance != null;
	}

	public static void Load()
	{
		InitTry = true;
		Addressables.InstantiateAsync("GameRoot").Completed += (handle) => {
			instance = handle.Result.GetComponent<GameRoot>();
			instance.name = "GameRoot";
		};
	}

	public void AddTouchAction(System.Action cb)
	{
		TouchStartActions.Enqueue(cb);
	}

	public void AddTitleCloseAction(System.Action cb)
	{
		TitleCloseActions.Enqueue(cb);
	}


	IEnumerator waitEndFrame(System.Action callback)
	{
		yield return new WaitForEndOfFrame();

		callback?.Invoke();
	}

	public void WaitEndFrameCallback(System.Action callback)
	{
		StartCoroutine(waitEndFrame(callback));
	}

	void InitUILoading()
	{
		// 로딩 팝업 어드레서블 로드
		var path = UISystem.GetUIPath<LoadingBasic>();
		if (!string.IsNullOrEmpty(path))
		{
			Addressables.InstantiateAsync(path, MainUITrans, false).Completed += (handle) =>
			{
				Loading = handle.Result.GetComponent<LoadingBasic>();
				loadcount++;
			};
		}
	}

	private void Update()
	{
		if (!LoadComplete)
			return;

		UserData.Update();
		PlayTimeSystem.Update();
		GachaSkillSystem.Update();

		if (deltaTime >= 1f) // one seconds updates;
		{

			deltaTime -= 1f;
		}
		deltaTime += Time.deltaTime;

		if (Input.GetMouseButtonUp(0))
		{
			if (InGameSystem.CurInGame != null)
			{
				Vector2 localPos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(MainCanvas.transform as RectTransform, Input.mousePosition,
					MainCanvas.worldCamera, out localPos);

			
				if (!IsPointerOverUIObject(Input.mousePosition))
				{
				}


				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
				{


					if (TouchStartActions.Count > 0)
					{
						var cnt = TouchStartActions.Count;
						for (int i = 0; i < cnt; i++)
						{
							TouchStartActions.Dequeue().Invoke();
						}
					}
				}
			}
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			//if (TutorialSystem.IsActive())
			//	return;

			var uibase = UISystem.GetOpenPopupLastUI();
			if (uibase != null)
			{
				UISystem.ClosePopupBackBtn();
			}
			else
			{
				
			}
		}
	}

	public bool IsPointerOverUIObject(Vector2 touchPos)
	{
		UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition
			= new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);

		eventDataCurrentPosition.position = touchPos;

		List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();


		UnityEngine.EventSystems.EventSystem.current
		.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

	public void DestroyObj(GameObject obj)
	{
		Destroy(obj);
	}

	IEnumerator Start()
	{
		if (instance == null)
		{
			Load();
			Destroy(this.gameObject);
			yield break;
		}
		//TouchStartActions.Clear();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//PluginSystem.Init();
		//InAppPurchaseManager = GetComponent<InAppPurchaseManager>();
		//if (InAppPurchaseManager != null)
		//	InAppPurchaseManager.Init();
		//SnapshotCam = SnapshotCamera.MakeSnapshotCamera("SnapShot");
		//SnapshotCam.transform.SetParent(this.transform);
		//SnapshotCam.transform.position = new Vector3(0f, 0f, -1f);
		//yield return TimeSystem.GetGoogleTime(null);
		UISystem.SetMainUI(MainUITrans);
		UISystem.SetHudUI(HUDUITrans);
		UISystem.SetWorldCanvas(WorldCanvas);
		UISystem.LockScreen = UILock;
		yield return LoadGameData();
		//Application.deepLinkActivated += OnDeepLinkActivated;
		if (!string.IsNullOrEmpty(Application.absoluteURL))
		{
			// Cold start and Application.absoluteURL not null so process Deep Link.
			//OnDeepLinkActivated(Application.absoluteURL);
		}
	}

	private IEnumerator LoadGameData()
	{
		
		yield return Config.Create();
		yield return Tables.Create();
		yield return SoundPlayer.Create();

		// 로딩 팝업 어드레서블 로드
	    loadcount = 0;
		InitUILoading();

		yield return new WaitUntil(() => loadcount == 1);
		UserData.Load();
		InGameSystem.ChangeMode(CurInGameType);
		InGameBattleSystem.Create();
		UnitUpgradeSystem.Create();
		GachaSkillSystem.Create();

		LoadComplete = true;

		InitSystem();
	}

	public void BgmOn()
	{
		if (GameRoot.instance.CurInGameType == InGameType.Event)
		{
			SoundPlayer.Instance.PlayBGM("bgm_pirate", true);
			SoundPlayer.Instance.BgmSwitch(UserData.Bgm);
		}
		else
		{
			SoundPlayer.Instance.PlayBGM("bgm", true);
			SoundPlayer.Instance.BgmSwitch(UserData.Bgm);
		}
	}

	public void ChangeIngameType(InGameType type, bool changeData = false)
	{

		CurInGameType = type;
		//InGameSystem.ChangeMode(CurInGameType);
		if (changeData)
		{
			DataState dataState = DataState.None;
			switch (CurInGameType)
			{
				case InGameType.Main:
					{
						dataState = DataState.Main;
					}
					break;
				case InGameType.Event:
					{
						dataState = DataState.Event;
					}
					break;
			}
			if (dataState != DataState.None)
				UserData.ChangeDataMode(dataState);
		}
	}

	void InitSystem()
	{
		//&& GameRoot.Instance.UserData.CurMode.StageData.StorageData.Count < 1
		//startGame
		//var count = GameRoot.instance.UserData.GetRecordCount(Config.RecordCountKeys.Init);
		if (GameRoot.Instance.UserData.CurMode.StageData.StageIdx == 1)
		{
			//if (UserData.UUID == 0)
			//	UserData.SetUUID(TpUtility.GetUUID());

			//GameRoot.instance.UserData.AddRecordCount(Config.RecordCountKeys.Init, 1);
			//InGameSystem.NextGameStage(true);




			SetNativeLanguage();

			//PluginSystem.InitMax(() =>
			//{
			//	PluginSystem.AnalyticsProp.AllEvent(IngameEventType.None, "install");
			//});

			//GameRoot.instance.UserData.CurMode.StageData.SetStageIdx(1);

		}
		else
		{

			//PluginSystem.InitMax();

			//Config.Instance.UpdateFallbackOrder(UserData.Language);
		}

		//ProjectUtility.Init();
		InGameBattleSystem.Create();
		UnitUpgradeSystem.Create();
	}

	private void SetNativeLanguage()
	{
	}

	public void SetCheatWindow(bool value)
	{
		if (CheatWindow != null)
			CheatWindow.SetActive(value);
	}

	IEnumerator waitTimeAndCallback(float time, System.Action callback)
	{
		yield return new WaitForSeconds(time);
		callback?.Invoke();
	}

	IEnumerator waitFrameAndCallback(int frame, System.Action callback)
	{
		for (int i = 0; i < frame; i++)
			yield return new WaitForEndOfFrame();
		callback?.Invoke();


	}

	public void WaitTimeAndCallback(float time, System.Action callback)
	{
		StartCoroutine(waitTimeAndCallback(time, callback));
	}

	public void WaitFrameAndCallback(int frame, System.Action callback)
	{
		StartCoroutine(waitFrameAndCallback(frame, callback));
	}

	public Vector3 GetRewardEndPos(int rewardType, int rewardIdx)
	{
		return Vector3.zero;
	}



	private void OnApplicationPause(bool pause)
	{
		if (!LoadComplete)
			return;




		if (pause)
		{

			PluginSystem.OnApplicationPause(pause);

			
				
		}
		else
		{
			


			//if (time.Equals(default(System.DateTime)))
			//{
			//	return;
			//}


			if (InGameSystem.GetInGame<InGameTycoon>() == null) return;
			if (InGameSystem.GetInGame<InGameTycoon>().curInGameStage == null) return;
			if (InGameSystem.GetInGame<InGameTycoon>().curInGameStage.IsLoadComplete == false) return;

			
		}
	}


#if UNITY_EDITOR
		private void OnApplicationQuit()
		{
			PluginSystem.OnApplicationPause(true);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
#endif

	}
