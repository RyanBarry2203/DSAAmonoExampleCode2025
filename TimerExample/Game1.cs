using Engine.Engines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TimerExample
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Vector2 _textPos;
        private Vector2 _targetPos;
        private SpriteFont message;
        private SoundEffect _sound;
        Color _color;
        byte _alpha = 255;
        // We use a time span to create a 2 second count
        TimeSpan _timer = new TimeSpan(0, 0, 2);
        // Text state is one of 3 possible states
        enum TEXTSTATE { STILL,MOVING,FINISHED};
        // A variable to keep track of the text state
        TEXTSTATE _textState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            new InputEngine(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            message = Content.Load<SpriteFont>("font");
            _sound = Content.Load<SoundEffect>("0");
            //SetDataOptions the initial position for the text
            _textPos = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
            // Set the target position for movement to 100 pixels above the starting position
            _targetPos = GraphicsDevice.Viewport.Bounds.Center.ToVector2() - new Vector2(0, 100);
            // create the initial colour RGBA all 255 to begin whihch is whiteand opaque
            _color = new Color((byte)_alpha, (byte)_alpha, (byte)_alpha, (byte)_alpha);
            // The initial state for the text is still
            _textState = TEXTSTATE.STILL;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // We transition from a STILL state to a MOVING state to a FINISHED state
            switch (_textState)
            {
                case TEXTSTATE.STILL:
                    _textPos = GraphicsDevice.Viewport.Bounds.Center.ToVector2();
                    // Transition to Moving using S key
                    if (InputEngine.IsKeyPressed(Keys.S))
                    {
                        _sound.Play();
                        _textState = TEXTSTATE.MOVING;
                    }
                    break;
                case TEXTSTATE.MOVING:
                    // while moving if the timer has not counted down
                    if (_timer.TotalMilliseconds > 0)
                    {
                        // The elapsed gametime is the time since the last update
                        // You can can subtract one TimeSpan object from another and it adjusts all the values
                        // seconds, milliseconds, ticks etc.
                        _timer = _timer - gameTime.ElapsedGameTime;
                        // We move the text pos by 0.01f using LERP (Linear interpolation)
                        _textPos = Vector2.Lerp(_textPos, _targetPos, 0.01f);
                        // We adjust the Alpha value by 5 while we are moving
                        if(_alpha > 0)
                            _alpha -= 5;
                        // Adjust the A for Alpha value of the current text color to the new alpha value
                        // making it more transparent
                        _color.A = _alpha;
                    }
                    // if the timespan timer has counted down fully we transition to the finished state
                    else {
                        _textState = TEXTSTATE.FINISHED;
                    }
                    break;
                    // if we are finished reset the values for the next possible movement and transition
                    // to the STILL state 
                    case TEXTSTATE.FINISHED:
                    _timer = new TimeSpan(0, 0, 2);
                    _alpha = 255;
                    _color.A = _alpha;
                    _textState = TEXTSTATE.STILL;
                    break;
                default:
                    break;
            }
                
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // In order to Blend the text we have to use BlendState.Additive
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            // We draw the val using the message font at the updated position 
            _spriteBatch.DrawString(message,"val",_textPos, _color);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}