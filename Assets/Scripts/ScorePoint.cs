public class ScorePoint{
	
	public int id;
	public string playerName;
	public int score;

	public ScorePoint(int a,string b,int c){
		id = a;
		playerName = b;
		score = c;
	}
	public int GetID(){
		return id;
	}
	public string GetName(){
		return playerName;
	}
	public int GetScore(){
		return score;
	}


}