﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace SleepyScientist
{
    public enum STATE { MAIN_MENU, LEVEL_SELECT, PLAY, PAUSE, INSTRUCTIONS }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        #region Attributes

        // State variables
        private static STATE _state = STATE.MAIN_MENU;
        public static STATE PrevState = _state;
        public static STATE State
        {
            get{ return _state; }
            // Save previous value and get new value
            set{ PrevState = _state; _state = value; }
        }
        private Menu _menu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _spriteFont;

        // Screen dimensions
        public static int screenWidth;
        public static int screenHeight;

        // Scroll wheel state
        private int _curScrollWheel;
        private int _deltaScrollWheel;

        // GameObjects
        private Scientist _sleepy;
        private List<Floor> _floors;
        private List<Ladder> _ladders;
        private List<Stairs> _stairs;
        private List<Pit> _pits;
        private List<Invention> _inventions;

        // Textures
        private Texture2D _stairsTexture;
        private Texture2D _ladderTexture;
        private Texture2D _floorTexture;
        private Texture2D _rocketSkateboardTexture;
        private Texture2D _eggBeaterTexture;
        private Texture2D _jackintheboxTexture;
        private Texture2D _bedTexture;
        private Texture2D _wallTexture;
        private Texture2D _railingTexture;
        private Texture2D _pitLaserLeft;
        private Texture2D _pitLaserRight;
        private Texture2D _pitLaserTile;
        private Texture2D _pitTerminal;
        private Texture2D _doorOpenTexture;
        private Texture2D _doorClosedTexture;

        // Mouse Input
        public static MouseState _prevMouseState;
        public static MouseState _curMouseState;

        // Keyboard Input
        public static KeyboardState _prevKeyboardState;
        public static KeyboardState _curKeyboardState;

        // Debug Messages

        // Camera
        private Camera _camera;

        // Test
        private bool _begin = false;
        private int _levelNumber = 1;
        private int _totalLevels = 7;
        private Room level = null;

        #endregion

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // Turns off full screen and sets the background
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = GameConstants.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.SCREEN_HEIGHT;
            graphics.ApplyChanges();

            // Save the new screen dimensions locally
            screenWidth = GameConstants.SCREEN_WIDTH;
            screenHeight = GameConstants.SCREEN_HEIGHT;

            // Initialize scroll wheel position.
            _curScrollWheel = Mouse.GetState().ScrollWheelValue;

            // Initialize test "Level" objects.
            _floors = new List<Floor>();
            _ladders = new List<Ladder>();
            _stairs = new List<Stairs>();
            _pits = new List<Pit>();
            _inventions = new List<Invention>();

            // Initialize Camera.
            _camera = new Camera();

            // Initialize menus
            _menu = new Menu();
            _menu.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the font for the Messages and give it to the MessageLayer.
            _spriteFont = Content.Load<SpriteFont>("Font/defaultFont");
            MessageLayer.Font = _spriteFont;

            // Set up the TextHelper.
            TextHelper.Batch = spriteBatch;
            TextHelper.Font = _spriteFont;
            TextHelper.TextColor = Color.White;
            TextHelper.Alignment = TextHelper.TextAlignment.Center;

            // Load animation sets.
            AnimationLoader.Load("ScientistAnimationSet.xml", Content);

            // Load content of other GameObjects.
            _ladderTexture = this.Content.Load<Texture2D>("Image/ladder");
            _floorTexture = this.Content.Load<Texture2D>("Image/floor");
            _stairsTexture = this.Content.Load<Texture2D>("Image/stairs");
            _rocketSkateboardTexture = this.Content.Load<Texture2D>("Image/skateboard");
            _eggBeaterTexture = this.Content.Load<Texture2D>("Image/eggbeater");
            _jackintheboxTexture = this.Content.Load<Texture2D>("Image/jack_inthe_box");
            _bedTexture = this.Content.Load<Texture2D>("Image/bed");
            _wallTexture = this.Content.Load<Texture2D>("Image/wall");
            _railingTexture = this.Content.Load<Texture2D>("Image/railing");
            _doorOpenTexture = this.Content.Load<Texture2D>("Image/door_open");
            _doorClosedTexture = this.Content.Load<Texture2D>("Image/door_closed");

            // Load textures for the pits
            _pitLaserLeft = this.Content.Load<Texture2D>("Image/laser_left_end");
            _pitLaserRight = this.Content.Load<Texture2D>("Image/laser_right_end");
            _pitLaserTile = this.Content.Load<Texture2D>("Image/laser_tile");
            _pitTerminal = this.Content.Load<Texture2D>("Image/battery_holder");
            
            // Make these textures static
            GameConstants.FLOOR_TEXTURE = _floorTexture;
            GameConstants.STAIR_TEXTURE = _stairsTexture;
            GameConstants.LADDER_TEXTURE = _ladderTexture;
            GameConstants.ROCKETBOARD_TEXTURE = _rocketSkateboardTexture;
            GameConstants.EGG_TEXTURE = _eggBeaterTexture;
            GameConstants.JACK_TEXTURE = _jackintheboxTexture;
            GameConstants.BED_TEXTURE = _bedTexture;
            GameConstants.RAILING_TEXTURE = _railingTexture;
            GameConstants.PIT_LEFT_END_TEXTURE = _pitLaserLeft;
            GameConstants.PIT_RIGHT_END_TEXTURE = _pitLaserRight;
            GameConstants.PIT_TERMINAL_TEXTURE = _pitTerminal;
            GameConstants.PIT_TILE_TEXTURE = _pitLaserTile;
            GameConstants.DOOR_OPEN_TEXTURE = _doorOpenTexture;
            GameConstants.DOOR_CLOSED_TEXTURE = _doorClosedTexture;
            
            // Create the level.
            Room level = LevelLoader.Load(_levelNumber);

            // This startx is a test to see if the loader broke
            int startx = level.StartX;

            // Create the scientist
            _sleepy = new Scientist("Sleepy", level.StartX, level.StartY, 50, 50, level);
            ResetCamera();

            // Store all the GameObjects.
            foreach (Floor floor in level.Floors)
            {
                _inventions.AddRange(floor.Inventions);
            }

            // Load content for menus
            _menu.LoadContent(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle input.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _prevMouseState = _curMouseState;
            _curMouseState = Mouse.GetState();
            _prevKeyboardState = _curKeyboardState;
            _curKeyboardState = Keyboard.GetState();

            #region Play
            if (_state == STATE.PLAY)
            {
                // Update mouse
                _deltaScrollWheel = Mouse.GetState().ScrollWheelValue - _curScrollWheel;

                if (_deltaScrollWheel != 0)
                {
                    _curScrollWheel = Mouse.GetState().ScrollWheelValue;
                    // If scroll up.
                    if (_deltaScrollWheel > 0)
                    {
                        // Zoom in.
                        _camera.ZoomToLocation(Mouse.GetState().X, Mouse.GetState().Y);
                        // Camera resumes following target if player is not moving an invention.
                        if (GameConstants.MOVING_INVENTION == false)
                            _camera.ShouldFollowTarget = true;
                    }
                    // If scroll down.
                    else
                    {
                        // Zoom out only if player is moving an invention.
                        if ( GameConstants.MOVING_INVENTION == true )
                            _camera.Zoom(GameConstants.ZOOM_ROOM_VIEW);
                    }
                }

                Point convertedMousePos = _camera.ToGlobal(new Point(_curMouseState.X, _curMouseState.Y));
				if (_begin)
            	{
                    // Update Game Time.
		            if (GameConstants.MOVING_INVENTION) { Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds / 2); }
		            else { Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds); }
					
		            foreach (Invention invention in _inventions)
		            {
		                invention.Update();
		                Rectangle convertedInventionPos = _camera.ToLocal(invention.RectPosition);
					
		                if (_prevMouseState.LeftButton == ButtonState.Pressed && 
		                _curMouseState.LeftButton == ButtonState.Released &&
		                convertedInventionPos.Contains(new Point(_curMouseState.X, _curMouseState.Y)))
			            {
		                    invention.Clicked = true;
		                    GameConstants.MOVING_INVENTION = true;
						
		                    _camera.ShouldFollowTarget = false;
		                    _camera.Zoom(GameConstants.ZOOM_ROOM_VIEW);
		                    break;
		                }

		                if (invention.Clicked && _prevMouseState.LeftButton == ButtonState.Pressed &&
		                    _curMouseState.LeftButton == ButtonState.Released)
		                {
		                    invention.HasTarget = true;
		                    invention.Clicked = false;
		                    invention.TargetX = convertedMousePos.X;
		                    invention.TargetY = convertedMousePos.Y;
		                    invention.VeloX = GameConstants.DEFAULT_INVENTION_X_VELOCITY;
		                    invention.DeterminePath();
		                    GameConstants.MOVING_INVENTION = false;

		                    _camera.ShouldFollowTarget = true;
		                    _camera.Zoom(GameConstants.ZOOM_INVENTION_VIEW);
		                }
		            }

		            _sleepy.Update();
		            MessageLayer.Update(gameTime.ElapsedGameTime.TotalSeconds);
		            if (_camera.ShouldFollowTarget == false)
		                _camera.UpdateCameraScroll(_curMouseState.X, _curMouseState.Y);

		            _camera.Update();

					// Check if the user won
		            if (_sleepy.Winner)
		            {
		                _begin = false;
		                if (_levelNumber == _totalLevels) { _levelNumber = 1; }
		                else { _levelNumber++; }
		                this.Reset();

		                // Load the current level
		                Room level = LevelLoader.Load(_levelNumber);
		                this.level = level;

		                // Create the scientist and set his image
		                _sleepy = new Scientist("Sleepy", level.StartX, level.StartY, 50, 50, level);
                        ResetCamera();
		                this.Load();
		            }

		            // Check if the user lost
		            if (_sleepy.Loser)
		            {
		                _begin = false;
		                this.Reset();

		                // Load the current level
		                Room level = LevelLoader.Load(_levelNumber);
		                this.level = level;

		                //Create the scientist and set his image
		                // Create the scientist and set his image
                        _sleepy = new Scientist("Sleepy", level.StartX, level.StartY, 50, 50, level);
                        ResetCamera();
		                this.Load();
		            }
				}
                else if (_curMouseState.LeftButton == ButtonState.Released &&
                    _prevMouseState.LeftButton == ButtonState.Pressed)
                {
                    _begin = true;

                    // Zoom into the scientist.
                    _camera.Zoom(GameConstants.ZOOM_ROOM_VIEW);
                    _camera.ShouldFollowTarget = true;
                    _camera.Update();
                }

                MessageLayer.Update(gameTime.ElapsedGameTime.TotalSeconds);
            }
            #endregion

            // Update menus
            _menu.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (_state == STATE.PLAY || _state == STATE.PAUSE)
            {
		        // Draw the background
		        GameObject wallTile;
		        for (int x = 0; x < GameConstants.SCREEN_WIDTH; x += 50)
		        {
		            for (int y = 0; y < GameConstants.SCREEN_HEIGHT; y += 50)
		            {
		                wallTile = new GameObject(x, y, 50, 50, GameConstants.DEFAULT_DIRECTION);
		                wallTile.Image = _wallTexture;
		                wallTile.Draw(spriteBatch);
		            }
		        }
				
                _camera.DrawGameObjects(spriteBatch, _sleepy.Room.GetGameObjects());
                _camera.DrawGameObject(spriteBatch, _sleepy );
            }

            // Draw menus
            _menu.Draw(spriteBatch);

            //MessageLayer.ClearMessages();
            //MessageLayer.AddMessage(new Message("-The objective of the game is to get the scientist to his bed in each level.\n-You may pick up inventions by clicking on them, and move the inventions by clicking on the place you want to move them.", 0, 0));
            //MessageLayer.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Resets all of the drawable objects in the level
        /// </summary>
        public void Reset()
        {
            _stairs.Clear();
            _ladders.Clear();
            _inventions.Clear();
            _floors.Clear();
            _pits.Clear();
        }

        /// <summary>
        /// Resets the camera to view the entire room and target the new scientist.
        /// Should be called at the start of each level.
        /// </summary>
        public void ResetCamera()
        {
            _camera.FollowTarget = _sleepy;
            _camera.Zoom(GameConstants.ZOOM_ROOM_VIEW);
            _camera.ShouldFollowTarget = false;
            _camera.Update();
        }

        /// <summary>
        /// Loads all of the drawable objects in the level
        /// </summary>
        public void Load()
        {
            // Store all the GameObjects.
            // This should be inside of the Level Class when we get to it.
            foreach (Floor floor in this.level.Floors)
            {
                _floors.Add(floor);
                _ladders.AddRange(floor.Ladders);
                _stairs.AddRange(floor.Stairs);
                _pits.AddRange(floor.Pits);
                _inventions.AddRange(floor.Inventions);
            }
        }
    }
}