using UnityEngine;
using UnityEngine.UI;

public class ControllerUIStack : MonoBehaviour, ILayoutGroup
{
	public bool ResizeLast;

	public void SetLayoutHorizontal()
	{
		var pos = 0f;
		for(var index = 0; index < transform.childCount; index++)
		{
			var component = transform.GetChild(index).GetComponent<RectTransform>();
			if(ReferenceEquals(null, component))
			{
				continue;
			}

			component.anchoredPosition = Vector2.down * pos;
			pos += component.rect.height;
			if(ResizeLast && index == transform.childCount - 1)
			{
				var @this = GetComponent<RectTransform>();
				if(!ReferenceEquals(null, @this))
				{
					var rect = @this.rect;
					component.sizeDelta = new Vector2(
						0,
						rect.height - pos);
				}
			}
		}
	}

	public void SetLayoutVertical() { }
}
