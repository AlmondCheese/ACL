using ACL.Values;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ACL.UI
{
    public class Checkbox : Component
    {
        #region Fields

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private Rectangle _currentMouse;
        private Rectangle _previousMouse;

        private bool _isHovering;

        #endregion

        #region Properties

        public event EventHandler Click = null!;
        public bool Value { get; set; } = false;
        public bool Locked { get; set;} = false;

        // Appearence

        public Texture2D CheckboxTexture {get; set;} = GameInstance.PlainTexture;
        public Color CheckboxColor { get; set; } = Color.White;
        public Color CheckboxHoverColor { get; set; } = new Color(200, 200, 200, 255);
        public Color CheckboxLockedColor { get; set; } = new Color(127, 127, 127, 255);
        public Color TextColor { get; set; } = Color.White;
        public Color TextHoverColor { get; set; } = new Color(200, 200, 200, 255);

        public string? Text { get; set; }
        public float TextScale { get; set; } = 1f;
        public SpriteFont? TextFont { get; set; }

        // Texturing
        public Rectangle TextureSourceRectangle { get; set; }
        public PositionVector TextureOffSourcePosition { get; set; } = new PositionVector(0, 0, 0, 0);
        public PositionVector TextureOffSourceSize { get; set; } = new PositionVector(.5f, .5f, 0, 0);
        public PositionVector TextureOnSourcePosition { get; set; } = new PositionVector(.5f, 0, 0, 0);
        public PositionVector TextureOnSourceSize { get; set; } = new PositionVector(.5f, .5f, 0, 0);

        #endregion

        public Rectangle CheckboxRectangle { get; set; }
        
        public Checkbox(GameInstance game) : base(game)
        {
            CheckboxRectangle = new Rectangle();
            TextureSourceRectangle = new Rectangle();
        }

        #region Methods

        public override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousMouse = _currentMouse;
            _currentMouse = Game.PlayerCursor;

            _isHovering = false;

            if (_currentMouse.Intersects(CheckboxRectangle))
            {
                _isHovering = true;

                if (_currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed && !Locked)
                {
                    Toggle();
                    Click?.Invoke(this, new EventArgs());
                }
            }

            UpdateSourceRectangles();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CheckboxRectangle = new Rectangle((int)Position.ConvertToScreenPosition(Game)[0], (int)Position.ConvertToScreenPosition(Game)[1], (int)Size.ConvertToScreenPosition(Game)[0], (int)Size.ConvertToScreenPosition(Game)[1]);
            if (CheckboxRectangle.Width != 0 && CheckboxRectangle.Height != 0)
            {
                // Draw Checkbox
                Color DrawColor;
                if (Locked) { DrawColor = CheckboxLockedColor; } else { DrawColor = _isHovering ? CheckboxHoverColor : CheckboxColor; }
                spriteBatch.Draw(CheckboxTexture, CheckboxRectangle, TextureSourceRectangle, DrawColor);
                
                // Draw Text
                if (!string.IsNullOrEmpty(Text) && TextFont != null)
                {
                    var x = CheckboxRectangle.X + CheckboxRectangle.Width * 1.5f;
                    var y = CheckboxRectangle.Y + (CheckboxRectangle.Height / 2 - TextFont.MeasureString(Text).Y);
                    var textColor = _isHovering ? TextHoverColor : TextColor;
                    spriteBatch.DrawString(TextFont, Text, new Vector2(x, y), textColor, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0.1f);
                }
            }
            
            base.Draw(gameTime, spriteBatch);
        }
        
        public void Toggle(bool? NewValue = null)
        {
            if (NewValue.HasValue) {Value = NewValue.Value;} else {Value = !Value;}
        }

        public void UpdateSourceRectangles()
        {
            Rectangle TextureBounds = new Rectangle(0, 0, CheckboxTexture.Width, CheckboxTexture.Height);
            if (!Value) { TextureSourceRectangle = new Rectangle((int)TextureOffSourcePosition.ConvertToBound(TextureBounds)[0], (int)TextureOffSourcePosition.ConvertToBound(TextureBounds)[1], (int)TextureOffSourceSize.ConvertToBound(TextureBounds)[0], (int)TextureOffSourceSize.ConvertToBound(TextureBounds)[1]); } 
            else {TextureSourceRectangle = new Rectangle((int)TextureOnSourcePosition.ConvertToBound(TextureBounds)[0], (int)TextureOnSourcePosition.ConvertToBound(TextureBounds)[1], (int)TextureOnSourceSize.ConvertToBound(TextureBounds)[0], (int)TextureOnSourceSize.ConvertToBound(TextureBounds)[1]); }
        }
        #endregion
    }
}