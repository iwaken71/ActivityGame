using UnityEngine;
using System.Collections;

[System.Serializable]
public class IntRect {
	
#region Instance Variables
	public	int	x;
	public	int	y;
	public	int	width;
	public	int height;
#endregion
	
#region Constructor	
	public IntRect () {
		this.x		= 0;
		this.y		= 0;
		this.width	= 1;
		this.height	= 1;
	}
#endregion
	
}
