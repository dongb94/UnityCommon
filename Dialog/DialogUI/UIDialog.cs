using System.Collections;
using System.Collections.Generic;
using UI2020;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIDialog : AbstractUI
{
#region <Fields>
	
	public Image LeftImage;
	public Image RightImage;
	public Label Name;
	public Label Script;
	public DialogSkipTrigger Skip;
	public DialogEventTrigger EventListener;

	private Transform leftSpriteTransform, rightSpriteTransform;
	private Vector3 leftSpriteOriginPos, rightSpriteOriginPos;
	
#endregion
	
#region <CallBack>

	public void Initialize()
	{
		Skip.Initialize();
		EventListener.Initialize();

		leftSpriteTransform = LeftImage.gameObject.transform;
		leftSpriteOriginPos = leftSpriteTransform.localPosition;
		
		rightSpriteTransform = RightImage.gameObject.transform;
		rightSpriteOriginPos = rightSpriteTransform.localPosition;
		
		SetActive(false);
	}

#endregion

#region <Methods>

	public void SetName(string name)
	{
		Name.text = name;
	}

	public void SetDialog(string dialog)
	{
		Script.text = dialog;
	}

	#region <Method/LeftImage>
	
	public void SetLeftImage(DialogManager.Character leftImage, bool Active)
	{
		SetLeftImage(leftImage);
		SetLeftImage(Active);
	}
	
	public void SetLeftImage(DialogManager.Character leftImage)
	{
		if (leftImage != DialogManager.Character.Empty)
		{
			// LeftImage.spriteName = leftImage.ToString(); TODO load and apply sprite
			leftSpriteTransform.localPosition = leftSpriteOriginPos;
			leftSpriteTransform.gameObject.SetActive(true);
		}
		else
		{
			leftSpriteTransform.gameObject.SetActive(false);
		}
	}
	
	public void SetLeftImage(bool Active)
	{
		LeftImage.color = Active ? Color.white : Color.gray;
	}
	
	#endregion

	#region <Method/RightImage>
	
	public void SetRightImage(DialogManager.Character rightImage, bool Active)
	{
		SetRightImage(rightImage);
		SetRightImage(Active);
	}
	
	public void SetRightImage(DialogManager.Character rightImage)
	{
		if (rightImage != DialogManager.Character.Empty)
		{
			// RightImage.spriteName = rightImage.ToString(); TODO load and apply sprite
			rightSpriteTransform.localPosition = rightSpriteOriginPos;
			rightSpriteTransform.gameObject.SetActive(true);
		}
		else
		{
			rightSpriteTransform.gameObject.SetActive(false);
		}
	}
	
	public void SetRightImage(bool Active)
	{
		RightImage.color = Active ? Color.white : Color.gray;
	}
	
	#endregion
	
#endregion
	
}