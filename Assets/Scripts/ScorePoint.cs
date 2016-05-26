public class ScorePoint{
	
	int id;
	string playerName;
	int score;

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