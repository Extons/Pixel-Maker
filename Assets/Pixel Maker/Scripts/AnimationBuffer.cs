using System.Collections.Generic;
using UnityEngine;

namespace PixelMaker
{
    public class AnimationBuffer
    {
        #region Private members

        private List<Texture2D> _buffer = null;

        private List<Texture2D> Buffer => _buffer = _buffer ?? new List<Texture2D>();

        #endregion Private members

        #region Public members

        public int Length => Buffer.Count;

        #endregion Public members

        #region API

        public void FillTextures(List<Texture2D> textures)
        {
            textures.AddRange(Buffer);
        }

        public void AddBuffer(Texture2D texture)
        {
            Buffer.Add(texture);
        } 
        
        public void GetSpriteSheet(out Texture2D sheet, Color background, int width, int height, FilterMode filterMode, int rows, int columns)
        {
            int index = 0;
            sheet = new Texture2D(width * columns, height * rows);
            sheet.filterMode = filterMode;

            for(int x = 0; x < sheet.width; x++)
            {
                for(int y = 0; y < sheet.height; y++)
                {
                    sheet.SetPixel(x, y, background);
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (index < Buffer.Count)
                    {
                        var selected = Buffer[index++];

                        if (selected == null)
                        {
                            continue;
                        }

                        for (int x = 0; x < selected.width; x++)
                        {
                            for (int y = 0; y < selected.height; y++)
                            {
                                var pixel = selected.GetPixel(x, y);
                                int xTarget = (j * width) + x;
                                int yTarget = ((rows - 1) - i) * height + y;

                                sheet.SetPixel(xTarget, yTarget, pixel);
                            }
                        }
                    }

                }
            }
            sheet.Apply();
        }

        public void Clear() => Buffer.Clear();

        #endregion API
    }
}
