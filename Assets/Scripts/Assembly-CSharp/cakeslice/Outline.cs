using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace cakeslice
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Renderer))]
	public class Outline : MonoBehaviour
	{
		public int color;

		public bool eraseRenderer;

		[HideInInspector]
		public int originalLayer;

		[HideInInspector]
		public Material[] originalMaterials;

		public Renderer Renderer { get; private set; }

		private void Awake()
		{
			Renderer = GetComponent<Renderer>();
		}

		private void OnEnable()
		{
			IEnumerable<OutlineEffect> enumerable = from c in Camera.allCameras.AsEnumerable()
				select c.GetComponent<OutlineEffect>() into e
				where e != null
				select e;
			foreach (OutlineEffect item in enumerable)
			{
				item.AddOutline(this);
			}
		}

		private void OnDisable()
		{
			IEnumerable<OutlineEffect> enumerable = from c in Camera.allCameras.AsEnumerable()
				select c.GetComponent<OutlineEffect>() into e
				where e != null
				select e;
			foreach (OutlineEffect item in enumerable)
			{
				item.RemoveOutline(this);
			}
		}
	}
}
