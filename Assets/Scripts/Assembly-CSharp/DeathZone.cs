using UnityEngine;

public class DeathZone : MonoBehaviour
{
	private NewMovement pm;

	private AudioSource aud;

	public GameObject sawSound;

	private bool crusher;

	public string deathType;

	public bool dontExplode;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			pm = other.GetComponent<NewMovement>();
			pm.GetHurt(pm.hp, false);
			if (sawSound != null)
			{
				Object.Instantiate(sawSound, other.transform.position, Quaternion.identity);
			}
			base.enabled = false;
		}
		else if (other.gameObject.tag == "Enemy" || other.gameObject.layer == 10)
		{
			EnemyIdentifier enemyIdentifier = other.gameObject.GetComponentInParent<EnemyIdentifier>();
			if (enemyIdentifier == null)
			{
				enemyIdentifier = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			}
			if (GetComponentInParent<Piston>() != null)
			{
				crusher = true;
			}
			if (!(enemyIdentifier != null))
			{
				return;
			}
			if (!enemyIdentifier.exploded && sawSound != null)
			{
				Object.Instantiate(sawSound, other.transform.position, Quaternion.identity);
			}
			if (!enemyIdentifier.exploded)
			{
				enemyIdentifier.hitter = "deathzone";
				if (!enemyIdentifier.dead)
				{
					GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>().AddPoints(80, deathType);
				}
				else
				{
					GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>().AddPoints(20, "<color=grey>" + deathType + "</color>");
				}
				if (!dontExplode)
				{
					enemyIdentifier.Explode();
				}
			}
		}
		else if (other.gameObject.tag == "Coin")
		{
			other.GetComponent<Coin>().GetDeleted();
		}
	}
}
