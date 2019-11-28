using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class ControllerScreen : MonoBehaviour
{
	private const float SWITCH_SPEED_F = 2f;

	private CanvasGroup _group;

	protected RectTransform RectTransform;

	protected void Awake()
	{
		RectTransform = GetComponent<RectTransform>();
		_group = GetComponent<CanvasGroup>();
		_group.alpha = 0f;
		_group.interactable = false;
	}

	// animation controller implementation, might be a strategy

	public bool FadeIn()
	{
		var result = _group.alpha + SWITCH_SPEED_F * Time.deltaTime;
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
		var result = _group.alpha - SWITCH_SPEED_F * Time.deltaTime;
		if(result > 0f)
		{
			_group.alpha = result;
			return false;
		}

		_group.alpha = 0f;

		return true;
	}
}
