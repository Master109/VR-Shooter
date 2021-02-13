using UnityEngine;

namespace Extensions
{
	public static class TextureExtensions
	{
		public static Rect GetRect (this Texture texture)
		{
			return Rect.MinMaxRect(0, 0, texture.width, texture.height);
		}

		public static Vector2Int GetSize (this Texture texture)
		{
			return new Vector2Int(texture.width, texture.height);
		}
	}
}