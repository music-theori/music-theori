﻿using System.Numerics;

using theori.Graphics;
using theori.Graphics.OpenGL;

namespace theori.Gui
{
    public class Sprite : GuiElement
    {
        private Texture m_textureBacking;
        public Texture Texture
        {
            get => m_textureBacking;
            set
            {
                if (value == m_textureBacking)
                    return;

                if (m_textureBacking == null && value != null)
                    Size = new Vector2(value.Width, value.Height);
                m_textureBacking = value;
            }
        }

        private Vector4 m_color = Vector4.One;
        public Vector4 Color
        {
            get => m_color;
            set
            {
                if (value == m_color)
                    return;
                m_color = value;
            }
        }

        public Material Material = null;
        public MaterialParams MaterialParams = new MaterialParams();

        public Sprite(Texture texture)
        {
            Texture = texture;
        }

        public override void Render(GuiRenderQueue rq)
        {
            base.Render(rq);

            if (Material != null)
                rq.DrawRect(CompleteTransform, new Rect(Vector2.Zero, DrawSize), Texture ?? Texture.Empty, Material, MaterialParams, Color);
            else rq.DrawRect(CompleteTransform, new Rect(Vector2.Zero, DrawSize), Texture ?? Texture.Empty, Color);
        }
    }
}
