using UnityEngine;

[CreateAssetMenu(fileName = "CursorTextures", menuName = "Core/UI/CursorTextures")]
public class CursorTexturesSO : ScriptableObject
{
	public CursorTexture[] cursors;

	private void OnValidate()
	{
		for (int i = 0; i < cursors.Length; i++)
		{
			if (cursors[i].frameDuration == 0) // �⺻���� �������� ���� ���
			{
				cursors[i].frameDuration = 0.1f;
			}
		}
	}
}