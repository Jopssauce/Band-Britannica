using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitPanel : MonoBehaviour,IPointerDownHandler
{
    #region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		Destroy(this.gameObject);
	}
	#endregion
}
