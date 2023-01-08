using UnityEngine;

	public class GameRangeItem : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer spriteRenderer;

		private Vector2Int coordinate;

		public Vector2Int Coordinate
		{
			get => coordinate;
			set
			{
				coordinate = value;
				transform.position = new Vector3(coordinate.x, coordinate.y);
			}
		}

		public Sprite Sprite
		{
			set
			{
				if (spriteRenderer != null)
					spriteRenderer.sprite = value;
			}
		}

		private bool visible;

		public bool Visible
		{
			get => visible;
			set
			{
				visible = value;
				gameObject.SetActive(visible);
			}
		}
	}
