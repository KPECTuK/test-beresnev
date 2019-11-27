using System;
using App;
using Model;
using UnityEngine;

public sealed class ControllerApp : MonoBehaviour
{
	private ControllerScreenGame _screenGame;
	private ControllerScreenMenu _screenMenu;

	private int _indexTransition;
	private ITransition[] _fsm;

	private Repository _repository;

	private void Awake()
	{
		_screenGame = transform.Find("screen-game")?.gameObject.AddComponent<ControllerScreenGame>() ?? throw new Exception("can't find screen GAME");
		_screenMenu = transform.Find("screen-menu")?.gameObject.AddComponent<ControllerScreenMenu>() ?? throw new Exception("can't find screen MENU");

		_repository = new Repository();

		_fsm = new ITransition[]	
		{
			new TransitionScreen(_screenGame, _screenMenu),
			new TransitionMenuInProgress(_screenMenu),
			new TransitionScreen(_screenMenu, _screenGame),
			new TransitionGameInProgress(_screenGame, new StrategyDefault()),
		};
	}

	private void Update()
	{
		_indexTransition += _fsm[_indexTransition].Execute(_repository) ? 1 : 0;
		_indexTransition %= _fsm.Length;
	}

	private void OnDestroy()
	{
		for(var index = 0; index < _fsm.Length; index++)
		{
			_fsm[index].Release();
		}
	}
}
