using UnityEngine;

public class ZombieIgnorizer : MonoBehaviour
{
	public EnemyIdentifier[] eids;

	private void Start()
	{
		EnemyIdentifier[] array = eids;
		foreach (EnemyIdentifier enemyIdentifier in array)
		{
			enemyIdentifier.ignoredByEnemies = true;
		}
	}
}
