public class ActionScore : Action
{
	public int points;

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;
	}

	protected override void Activate()
	{
		gameManager.scoreManager.SpawnFloater(points);
		ActionComplete();
	}
}