using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class CommentData : ScriptableObject
{
	public List<CommentDataEntity> normal;
	public List<CommentDataEntity> homeru;
	public List<CommentDataEntity> carry;
	public List<CommentDataEntity> hint;
	public List<CommentDataEntity> end;
}
