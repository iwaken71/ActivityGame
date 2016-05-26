using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsoleAnimation : MonoBehaviour {

#region Constants
	public	const int		PLAYING		= 0;
	public	const int		FINISHED	= -1;
#endregion
	
#region Instance Variables
	public	int				spriteIndex	= 0;
	
	public	bool			random		= false;
	public	bool			loop		= false;
	
	public	List<IntRect>	frames		= new List<IntRect> ( 0 );
	private	int				frame		= 0;
	public	IntRect			Frame		{ get { return frames[frame]; } }
	public	int				FrameIndex	{ get { return frame; } set { frame = value; } }
	
	private	int				status		= PLAYING;
	public	int				Status		{ get { return status; } }
#endregion
	
#region Initialization
	public void InitWithAnimation ( ConsoleAnimation animation ) {
		
		this.spriteIndex	= animation.spriteIndex;
		
		this.random			= animation.random;
		this.loop			= animation.loop;
		
		this.frames			= animation.frames;
		this.frame			= animation.frame;
		
		this.status			= animation.status;
		
	}
#endregion
	
#region Playing
	public void Reset ()	{ frame = 0; status = PLAYING; }
	
	public void Play ()	{ 

		if ( random == true )
			frame = Random.Range ( 0, frames.Count );
		else if ( loop == true )
			frame = ( frame + 1 ) % frames.Count;
		else {
			frame += 1;
			if ( frame >= frames.Count ) { frame = frames.Count - 1; status = FINISHED; }
		}
	
	}
#endregion
	
}
