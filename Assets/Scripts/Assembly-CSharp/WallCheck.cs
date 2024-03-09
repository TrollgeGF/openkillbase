using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
	public bool onWall = false;

	public Vector3 poc;

	public List<Collider> cols = new List<Collider>();

	private List<Collider> colsToDelete = new List<Collider>();

	private void Update()
	{
		if (onWall && cols.Count == 0)
		{
			onWall = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.layer == 8 || other.gameObject.layer == 24) && !other.isTrigger)
		{
			onWall = true;
			cols.Add(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.gameObject.layer == 8 || other.gameObject.layer == 17 || other.gameObject.layer == 24) && cols.Contains(other))
		{
			cols.Remove(other);
		}
	}

	public Vector3 GetClosestPoint()
	{
		poc = Vector3.zero;
		float num = 100f;
		if (cols.Count > 1)
		{
			foreach (Collider col in cols)
			{
				if (col != null && col.enabled && col.gameObject.activeInHierarchy && (col.gameObject.layer == 8 || col.gameObject.layer == 24) && !col.isTrigger)
				{
					Vector3 a = col.ClosestPoint(base.transform.position);
					if (Vector3.Distance(a, base.transform.position) < num && Vector3.Distance(a, base.transform.position) < 5f)
					{
						num = Vector3.Distance(a, base.transform.position);
						poc = a;
					}
					else if (Vector3.Distance(a, base.transform.position) >= 5f)
					{
						colsToDelete.Add(col);
					}
				}
				else
				{
					colsToDelete.Add(col);
				}
			}
		}
		else if (cols.Count == 1 && cols[0] != null && cols[0].enabled && cols[0].gameObject.activeInHierarchy && Vector3.Distance(cols[0].ClosestPoint(base.transform.position), base.transform.position) < 5f)
		{
			poc = cols[0].ClosestPoint(base.transform.position);
		}
		else if (Vector3.Distance(cols[0].ClosestPoint(base.transform.position), base.transform.position) < 5f)
		{
			colsToDelete.Add(cols[0]);
		}
		if (colsToDelete.Count > 0)
		{
			foreach (Collider item in colsToDelete)
			{
				cols.Remove(item);
			}
		}
		colsToDelete.Clear();
		return poc;
	}
}
