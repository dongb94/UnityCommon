using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using k514;
using UnityEngine;

/// <summary>
/// TODO Byeon : JSON 파일 형식 확정
/// </summary>
public class DialogManager : Singleton<DialogManager>
{
	#region <Consts>

	// TextAsset을 불러올 헤드 디렉터리
	private string basePath = "Json/Script/";

	#endregion

	#region <Fields>

	private Language currentLanguage;
	private Action _onExitAction;
	// JSON 형식의 TestAsset을 파싱하면, 문맥스크립트<DialogData>가 큐 형태로 반환된다.
	private Queue<DialogData> currentDialog;
	// 현재 포커싱 중인 문맥 스크립트
	private DialogData currentScript;

	#endregion
	
	#region <enum>

	// 현재 대사 또는 문장의 화자
	public enum Character
	{
		Empty,
		Default,
		Katarina,
		Psyous,
		Hanagi,
		count,
	}

	// 언어팩
	public enum Language
	{
		Korean,
		count
	}
	
	#endregion

	#region <Callback>
	
	public override void OnCreated()
	{
		currentLanguage = Language.Korean;
	}

	public override void OnInitiate()
	{
		
	}
	
	#endregion

	#region <Method/ScriptPreset>

	public Queue<DialogData> GetParsingDialogQueue(string path)
	{
		var json = Resources.Load<TextAsset>(basePath + currentLanguage.ToString() + "/" + path);
		return DialogParser.Parse(json.text);
	}

	public void SetLanguage(Language language)
	{
		currentLanguage = language;
		/// TODO : sync()호출하여 인터페이스 언어 변경
	}

	public void SetTrigger(Queue<DialogData> p_TargetDialog, Action exitEvent = null)
	{
		currentDialog = p_TargetDialog;
		_onExitAction = exitEvent;
		
		if (currentDialog.Count == 0)
		{
			ExitDialog();
		}
		else
		{
			// TODO UI update
			//HUDManager.GetInstance.SetScriptMode();
			StartDialog();
		}
	}
	public void SetTrigger(string path, Action exitEvent = null)
	{
		currentDialog = GetParsingDialogQueue(path);
		_onExitAction = exitEvent;
		
		if (currentDialog.Count == 0)
		{
			ExitDialog();
		}
		else
		{
			// TODO UI update
			// HUDManager.GetInstance.SetScriptMode();
			StartDialog();
		}
	}

	#endregion

	#region <Method/Script>
	
	private void InitializeDialog()
	{
		var data = currentScript;
		// TODO UI update
//		HUDManager.GetInstance.DialogUI.SetLeftImage(data.LeftCharacter, data.LeftActive);
//		HUDManager.GetInstance.DialogUI.SetRightImage(data.RightCharacter, data.RightActive);
//		HUDManager.GetInstance.DialogUI.SetName(data.Name);
		DisplayNextText();
	}

	private void DisplayNextText()
	{
		if (currentScript.TextGroup.Count != 0) ;
		// TODO UI update
		// HUDManager.GetInstance.DialogUI.SetDialog(currentScript.TextGroup.Dequeue());
	}

	public void NextEvent()
	{
		if (currentScript.TextGroup.Count == 0)
		{
			if (currentDialog.Count == 0)
			{
				ExitDialog();
			}
			else
			{
				StartDialog();
			}
		}
		else
		{
			DisplayNextText();
		}
	}

	public void StartDialog()
	{
		currentScript = currentDialog.Dequeue();
		InitializeDialog();
	}

	public void ExitDialog()
	{
		// TODO UI update
		// HUDManager.GetInstance.Deactivate = HUDManager.HUDState.Script;
		_onExitAction?.Invoke();
	}
	
	#endregion

}