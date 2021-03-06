﻿#region using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace SleepyScientist
{
    class GameObject
    {
        #region Attributes

        // The rectangle position of the game object
        private RectangleVector _rectPosition;

        // The image of the game object
        private Texture2D _image;

        // The animations of the game object.
        private AnimationSet _animations;

        // The direction of the game object
        private int _direction;

        #endregion

        #region Properties

        public RectangleVector RectPosition { get { return _rectPosition; } set { _rectPosition = value; } }
        
        // Get or set the image of the game object
        public virtual Texture2D Image {
            get {
                if (_animations != null)
                    return _animations.CurAnimation.CurrentImage();
                else
                    return _image;
            }
            set { _image = value; }
        }

        // Get or set the x-coordinate of the game object
        public float X { get { return _rectPosition.X; } set { _rectPosition.X = value; } }
        
        // Get or set the y-coordinate of the game object
        public float Y { get { return _rectPosition.Y; } set { _rectPosition.Y = value; } }

        // Get or set the width of the game object
        public float Width { get { return _rectPosition.W; } set { _rectPosition.W = value; } }
        
        // Get or set the height of the game object
        public float Height { get { return _rectPosition.Z; } set { _rectPosition.Z = value; } }

        // Get or set the AnimationSet of the game object.
        public AnimationSet Animations { get { return _animations; } set { _animations = value; } }

        // Get or set the direction of the game object
        public int Direction { get { return _direction; } set { _direction = value; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor of a game object
        /// </summary>
        /// <param name="x">Starting x-coordinate</param>
        /// <param name="y">Starting y-coordinate</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public GameObject(float x, float y, float width, float height, int direction) 
        {
            _rectPosition = new RectangleVector(x, y, width, height);
            _direction = direction;
            _animations = null;
        }

        public GameObject(int x, int y, int width, int height, Texture2D image)
            : this(x, y, width, height, 1)
        {
            this._image = image;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draw the game object
        /// </summary>
        /// <param name="batch">The sprite batch you want to draw on</param>
        public virtual void Draw(SpriteBatch batch, Rectangle? pos = null) {
            if (pos != null)
                switch (this.Direction)
                {
                    case -1:
                        //batch.Draw(this.Image, this.RectPosition, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        batch.Draw(this.Image, pos.Value, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    case 1:
                        batch.Draw(this.Image, pos.Value, Color.White);
                        break;
                }
            else
                switch (this.Direction)
                {
                    case -1:
                        batch.Draw(this.Image, this.RectPosition, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                        break;
                    case 1:
                        batch.Draw(this.Image, this.RectPosition, Color.White);
                        break;
                }
        }

        /// <summary>
        /// Update the GameObject, namely the animation.
        /// </summary>
        public virtual void Update()
        {
            if (_animations != null)
                _animations.Update();
        }

        #endregion
    }
}