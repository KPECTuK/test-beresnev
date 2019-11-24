using System;
using UnityEngine;

public abstract class ControllerScreen : MonoBehaviour
{
	private const float SPEED_F = 2f;

	private CanvasGroup _group;

	private void Awake()
	{
		_group = GetComponent<CanvasGroup>() ?? throw new Exception($"can't find cant find handler for: {GetType().Name}");
		_group.alpha = 0f;
		_group.interactable = false;
	}

	public bool FadeIn()
	{
		var result = _group.alpha + SPEED_F * Time.deltaTime;
		if(result < 1f)
		{
			_group.alpha = result;
			return false;
		}

		_group.alpha = 1f;
		_group.interactable = true;

		return true;
	}

	public bool FadeOut()
	{
		_group.interactable = false;
		var result = _group.alpha - SPEED_F * Time.deltaTime;
		if(result > 0f)
		{
			_group.alpha = result;
			return false;
		}

		_group.alpha = 0f;

		return true;
	}
}