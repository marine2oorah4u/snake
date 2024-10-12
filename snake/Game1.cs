using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Snake
{
    public class Game1 : Game
    {
        enum GameState { MainMenu, GamePlay, ControlsMenu, OptionsMenu, Controls2, ResolutionPicker, SkinChooser, ColorPicker, BackgroundColorPicker, ColorConfirm, GameOver };

        class Food //should be renamed to PowerUp
        {
            public Point Pos;
            public int Type;
            public bool IsShuffling = true; // Whether the food is shuffling colors
            public float shuffleTimer = 0f; // Timer to control color shuffling
            public float totalElapsedTime = 0f; // Total elapsed time since shuffling started
            public Color Color; // Current color of the food
        }

        class Snake
        {
            public Point Pos;
            public Point Vel;
            public double Speed;
            public double StepTimer;
            public int TailLength;
            public Point[] Tail;
            public float[] TailAlpha;
            public bool IsVisible { get; set; }

            private const int MaxTailLength = 1000; // Adjust this value as needed

            public Snake()
            {
                Tail = new Point[MaxTailLength];
                TailAlpha = new float[MaxTailLength];
                ResetSnake();
            }

            public void ResetSnake()
            {
                Pos = new Point(0, 0);
                Vel = new Point(0, 0);
                Speed = 0.1;
                StepTimer = 0;
                TailLength = 0;
                IsVisible = true;

                // Reset all tail alphas to 1.0f
                for (int i = 0; i < TailAlpha.Length; i++)
                {
                    TailAlpha[i] = 1.0f;
                }
            }

            public void AddTailSegment()
            {
                if (TailLength < MaxTailLength)
                {
                    Tail[TailLength] = Pos;
                    TailAlpha[TailLength] = 1.0f;
                    TailLength++;
                }
            }

            public void UpdateTailAlpha(float alpha)
            {
                for (int i = 0; i < TailLength; i++)
                {
                    TailAlpha[i] = alpha;
                }
            }
        }


        GameState gameState;
        Random random;
        SpriteFont spriteFont;
        int cellSize;
        int score;
        int defaultFoodPosX;
        int defaultFoodPosY;
        Point worldSize;
        int worldScreenTopY;
        Snake snake;
        Food[] food;
        Food defaultFood;
        Color[] foodColorList;
        SoundEffect foodMunch;
        int selectedSlider;
        int selectedResolution;
        string redString;
        bool enterKeyPressed;
        KeyboardState prevKeyboardState;
        int colorIndex = 0;
        int currentTextureIndex = 0;
        int highScore;
        bool isScorePassed;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        bool isMainMenuVisible = true;
        string[] menuOptions = { "Start Game", "Controls Menu", "Options", "Exit" };
        int SelectedMainMenuIndex = 0;

        bool isControlsMenuVisible = false;
        string[] controlsMenuOptions = { "Move Up", "Move Down", "Move Left", "Move Right", "Enter", "Back" };
        int selectedControlsMenuIndex = 0;


        bool isOptionsMenuVisible = false;
        string[] optionsMenuOptions = { "Color Picker Options", "Back" };
        int selectedOptionsMenuIndex = 0;


        bool isColorPickerOptionsVisible = false;
        string[] colorPickerOptions = { "Background Color", "Back" };
        int selectedColorPickerIndex = 0;


        SpriteFont font;
        SpriteFont mainMenuTextFont;
        Texture2D texPressEnter;
        Texture2D[] textures;
        Texture2D[] resolutionTextureSize;
        Texture2D texture;
        Texture2D texSnakeArt;
        Texture2D texSnakeGameText;
        Texture2D texO;
        Texture2D texN;
        Texture2D texS;
        Texture2D texC;
        Texture2D texB;
        Texture2D texR;
        Texture2D texM;
        Texture2D texSkinSelector;
        Texture2D texColorPickerText;
        Texture2D texControlsText;
        Texture2D texBackgroundColorText;
        Texture2D texResolutionPicker;
        Texture2D texAvailableResolutions;
        Texture2D texUp;
        Texture2D texDown;
        Texture2D texLeft;
        Texture2D texRight;
        Texture2D texEsc;
        Texture2D texBack;
        Texture2D texEnter;
        Texture2D texSlideBarBorder;
        Texture2D texChangeConfirm;
        Texture2D texGameOverText;
        Texture2D texY;
        Texture2D texBlackViper;
        Texture2D texRedSliderBar;
        Texture2D texGreenSliderBar;
        Texture2D texBlueSliderBar;
        Texture2D texSliderBarLong;
        Texture2D texBlackTexture;
        Texture2D texEnterKey;

        Color background;
        Color redPreview;
        Color redBoxPreview;
        Color greenBoxPreview;
        Color blueBoxPreview;
        Color backgroundColor = Color.DarkOliveGreen; // Default color
        Color[][] snakTailColorPresets = new Color[][]
          {
       new Color[] { Color.Red, Color.Black },
       new Color[] { Color.White, Color.Blue },
       new Color[] { Color.Cyan, Color.PaleVioletRed, Color.DarkBlue }
        };

        float xPosition = 380f; // Initial x position
        const float xPositionIncrement = 1.96078431372549f; // Increment value for moving left/right
        const float minX = 380f; // Minimum x position
        const float maxX = 880f; // Maximum x position

        float xRed = 0f; // Initial Red position
        const float xRedIncrement = 1f; // Increment value for moving left/right
        const float minRed = 0f; // Minimum Red position
        const float maxRed = 255f; // Maximum Red position

        float yPosition = 380f; // Initial x position
        const float YPositionIncrement = 1.96078431372549f; // Increment value for moving left/right
        const float minY = 380f; // Minimum x position
        const float maxY = 880f; // Maximum x position

        float xGreen = 0f; // Initial Red position
        const float xGreenIncrement = 1f; // Increment value for moving left/right
        const float minGreen = 0f; // Minimum Red position
        const float maxGreen = 255f; // Maximum Red position

        float zPosition = 380f; // Initial x position
        const float ZPositionIncrement = 1.96078431372549f; // Increment value for moving left/right
        const float minZ = 380f; // Minimum x position
        const float maxZ = 880f; // Maximum x position

        float xBlue = 0f; // Initial Red position
        const float xBlueIncrement = 1f; // Increment value for moving left/right
        const float minBlue = 0f; // Minimum Red position
        const float maxBlue = 255f; // Maximum Red position

        float xBarPosition; // Will be updated to match xPosition
        float yBarPosition; // Will be updated to match yPosition
        float zBarPosition; // Will be updated to match zPosition

        bool isFading = false; // Flag to check if the fade effect is active
        bool fadingIn = false; // Indicates whether fading in or out
        float fadeSpeed = 1.0f; // Speed of the fade effect
        float fadeAlpha = 0.0f; // Alpha value of the fade overlay


        public float[] TailAlpha; // Array to store alpha values for each tail segment



        private float flashDuration = 0.1f; // Duration of each flash cycle (seconds)
        private float totalFlashTime = 1f; // Total time for flashing effect (seconds)
        private float elapsedTime = 0f;     // Time elapsed for current flash cycle
        private float flashTimeElapsed = 0f; // Time elapsed since flashing started
        private bool isFlashing = false;    // Toggle flashing state
        private string flashText = "";      // Text to display during flashing
        private Vector2 textPosition = new Vector2(100, 100); // Position of the text
        private Color textColor = Color.White; // Default color
        private float textScale = 2f;        // Scale of the text
        private bool isTextVisible = false; // Toggle text visibility
        private bool isPressKeyPromptVisible = true; // Flag to control visibility of the prompt



        private bool isShuffling = false;       // Flag to indicate if shuffling is active
        public bool IsShuffling = true; // Whether the food is shuffling colors
        public float totalElapsedTime = 0f; // Total elapsed time since shuffling started


        private float currentCellSize;
        private float targetCellSize;
        private float transitionSpeed = 2f; // Adjust this to control the speed of the transition

        private int gameAreaWidth = 640;
        private int gameAreaHeight = 360;
        private Color borderColor = Color.DarkOliveGreen;
        private int gameAreaX;
        private int gameAreaY;



        private void UpdateGameAreaPosition()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Center the game area
            gameAreaX = (screenWidth - gameAreaWidth) / 2;
            gameAreaY = (screenHeight - gameAreaHeight) / 2;
        }


        // texture index for default size is 10, or resolutionTextureSize10


        // this is the default size used for world size
        //     private int definedWidth = 1280;
        //    private int definedHeight = 720;






        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


            // Use variables for default width and height
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

        }

        protected override void Initialize()
        {
            base.Initialize();

            random = new Random();
            KeyboardState prevKeyboardState;

            foodColorList = new Color[]
            {
Color.Red,      // Type 0
Color.Orange,   // Type 1
Color.Yellow,   // Type 2
Color.Green,    // Type 3
Color.Blue,     // Type 4
Color.Purple,   // Type 5
Color.Brown,    // Type 6
Color.Black,     // Type 7
Color.Pink
            };

            cellSize = 16;
            worldScreenTopY = 16;
            worldSize.X = GraphicsDevice.Viewport.Width / cellSize;
            worldSize.Y = (GraphicsDevice.Viewport.Height - worldScreenTopY) / cellSize;
            snake = new Snake();
            snake.Tail = new Point[worldSize.X * worldSize.Y];
            xBarPosition = xPosition;
            yBarPosition = yPosition;
            zBarPosition = zPosition;
            defaultFoodPosX = random.Next(0, worldSize.X);
            defaultFoodPosY = random.Next(0, worldSize.Y);
            Color DefaultColor = Color.White;
            int highScore = 0;
            food = new Food[1];
            for (int i = 0; i < food.Length; ++i)
            {
                food[0] = new Food();

            }



            // testing
            //food[food.Length - 1] = new Food()
            //{
            //    Type = 0,
            //};
            // testing
        }

        protected override void LoadContent()
        {
            // one time asset creation
            Window.Position = new Point(1200, 100);
            mainMenuTextFont = Content.Load<SpriteFont>("CourierNew");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foodMunch = Content.Load<SoundEffect>("munch");
            spriteFont = Content.Load<SpriteFont>("spriteFont"); // Ensure this matches your asset name

            texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[] { Color.White });

            textures = new Texture2D[2];
            textures[0] = Content.Load<Texture2D>("gameOver");
            textures[1] = Content.Load<Texture2D>("blackViper");


            resolutionTextureSize = new Texture2D[16];

            prevKeyboardState = Keyboard.GetState();


            texPressEnter = Content.Load<Texture2D>("texPressEnter");
            texSnakeArt = Content.Load<Texture2D>("snake");
            texSnakeGameText = Content.Load<Texture2D>("snakeGameText");
            texBlackTexture = Content.Load<Texture2D>("texBlackTexture");
            texO = Content.Load<Texture2D>("O");
            texN = Content.Load<Texture2D>("N");
            texS = Content.Load<Texture2D>("texS");
            texC = Content.Load<Texture2D>("texC");
            texB = Content.Load<Texture2D>("texB");
            texR = Content.Load<Texture2D>("texR");
            texM = Content.Load<Texture2D>("texM");
            texEsc = Content.Load<Texture2D>("Esc");
            texEnter = Content.Load<Texture2D>("texEnter");
            texUp = Content.Load<Texture2D>("up");
            texDown = Content.Load<Texture2D>("down");
            texLeft = Content.Load<Texture2D>("left");
            texRight = Content.Load<Texture2D>("right");
            texBack = Content.Load<Texture2D>("back");
            texGameOverText = Content.Load<Texture2D>("GameOverText");
            texY = Content.Load<Texture2D>("Y");
            texSkinSelector = Content.Load<Texture2D>("texSkinSelector");
            texColorPickerText = Content.Load<Texture2D>("texColorPickerText");
            texControlsText = Content.Load<Texture2D>("texControlsText");
            texBackgroundColorText = Content.Load<Texture2D>("texBackgroundColorText");
            texRedSliderBar = Content.Load<Texture2D>("texRedSliderBar");
            texGreenSliderBar = Content.Load<Texture2D>("texGreenSliderBar");
            texBlueSliderBar = Content.Load<Texture2D>("texBlueSliderBar");
            texSlideBarBorder = Content.Load<Texture2D>("texSlideBarBorder");
            texChangeConfirm = Content.Load<Texture2D>("texChangeConfirm");
            texSliderBarLong = Content.Load<Texture2D>("texSliderBarLong");
            texResolutionPicker = Content.Load<Texture2D>("texResolutionPicker");
            texAvailableResolutions = Content.Load<Texture2D>("texAvailableResolutions");
        }

        //do a condition check to see if the high score is passed


        Color GetFlashingColor(GameTime gameTime, Color color1, Color color2)
        {
            // Get a value that oscillates between 0 and 1 using a sine wave
            float t = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5); // 2 is the speed of the flashing
            t = (t + 1) / 2; // Normalize to 0-1 range

            // Interpolate between the two colors based on the t value
            return Color.Lerp(color1, color2, t);
        }

        void InitGamePlay()
        {
            gameState = GameState.GamePlay;
            score = 0;
            snake.ResetSnake();
            snake.Pos.X = worldSize.X / 2;
            snake.Pos.Y = worldSize.Y / 2;
            snake.Vel.X = 0;
            snake.Vel.Y = 0;

            isFadingTail = false;
            fadeTimer = 0f;

            // No need to reset TailAlpha here, as it's done in ResetSnake()

            for (int i = 0; i < food.Length; ++i)
            {
                SpawnFood(i);
            }

            isBlackFadeActive = false;
            blackFadeTimer = 0f;
            blackFadeAlpha = 1.0f;

            // Reset any other game state variables as needed
        }



        private void StopFlashing()
        {
            isFlashing = false;
            flashText = ""; // Clear the text
                            //      textColor = Color.Transparent; // Hide the text
            elapsedTime = 0f; // Reset elapsed time
                              //        flashTimeElapsed = 0f; // Reset total flashing time
        }





        float textureFadeAlpha = 0.5f; // Start semi-transparent (adjust as needed)
        float minAlpha = 0.2f; // Minimum alpha value (e.g., 0.3 for 30% opacity)
        float maxAlpha = 0.7f; // Maximum alpha value (e.g., 0.8 for 80% opacity)
        float textureFadeSpeed = 01.5f; // Speed of the fade effect (adjust as needed)
        float fadeTime = 0.8f; // Time elapsed for the fade effect
        void UpdateTextureFadeEffect(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fadeTime += elapsedTime * textureFadeSpeed;

            // Smooth transition using a sinusoidal function
            float fadeAmount = (float)(Math.Sin(fadeTime) * 0.4 + 0.5); // Value between 0 and 1

            // Interpolate between minAlpha and maxAlpha
            textureFadeAlpha = MathHelper.Lerp(minAlpha, maxAlpha, fadeAmount);

            // Optional: Reset fadeTime to avoid overflow (if needed)
            if (fadeTime > Math.PI * 2) // Complete cycle (0 to 1 and back to 0)
            {
                fadeTime -= (float)(Math.PI * 1);
            }
        }

        void InitMainMenu()
        {
            isMainMenuVisible = true;
            gameState = GameState.MainMenu;
            isBlackFadeActive = false;
            blackFadeTimer = 0f;
            blackFadeAlpha = 1.0f;


        }

        void InitControlsMenu()
        {
            gameState = GameState.ControlsMenu;
        }




        void InitOptionsMenu()
        {

            gameState = GameState.OptionsMenu;

        }





        void InitControls2()
        {
            gameState = GameState.Controls2;
        }
        void InitResolutionPicker()
        {
            gameState = GameState.ResolutionPicker;
        }

        void InitColorPicker()
        {
            gameState = GameState.ColorPicker;
        }

        void InitSkinChooser()
        {
            gameState = GameState.SkinChooser;
        }

        void InitBackgroundColorPicker()
        {
            gameState = GameState.BackgroundColorPicker;
        }

        void InitColorConfirm()
        {
            gameState = GameState.ColorConfirm;
        }

        void InitGameOver()
        {
            gameState = GameState.GameOver;

            // Stop any ongoing fade effects to prevent issues during restart
            isBlackFadeActive = false;
            blackFadeTimer = 0f;
            blackFadeAlpha = 1.0f;

            // Reset flash text and other relevant states if necessary
            flashText = ""; // Clear any previous flash text
            isFlashing = false; // Stop flashing

            // Reset tail visibility and alpha values
            for (int i = 0; i < snake.TailLength; i++)
            {
                snake.TailAlpha[i] = 1.0f; // Ensure all segments are fully visible again
            }

                           snake.IsVisible = true; // Start as visible
                TailAlpha = new float[100]; // Same size as Tail
                for (int i = 0; i < TailAlpha.Length; i++)
                {
                    TailAlpha[i] = 1.0f; // Set all tail segments to fully opaque
                }
        }

        void SpawnFood(int i)
        {
            food[i].Pos.X = random.Next(0, worldSize.X);
            food[i].Pos.Y = random.Next(0, worldSize.Y);
            food[i].Type = random.Next(0, foodColorList.Length);
        }


        void SpawnDefaultFood()
        {
            defaultFoodPosX = random.Next(0, worldSize.X);
            defaultFoodPosY = random.Next(0, worldSize.Y);
        }



        //bool isMainMenuVisible = true;
        //string[] menuOptions = { "Start Game", "Options", "Exit" };
        //int SelectedMainMenuIndex = 0;

        //bool isOptionsMenuVisible = false;
        //string[] optionsMenuOptions = { "Color Picker Options", "Back" };
        //int selectedOptionsMenuIndex = 0;





        string[] GetUpdatedMenuOptions()
        {
            string[] updatedMenuOptions = new string[menuOptions.Length];

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == SelectedMainMenuIndex)
                {
                    updatedMenuOptions[i] = $"> {menuOptions[i]} <";
                }
                else
                {
                    updatedMenuOptions[i] = menuOptions[i];
                }
            }

            return updatedMenuOptions;
        }



        //bool isControlsMenuVisible = false;
        //string[] controlsMenuOptions = { "Move Up", "Move Down", "Move Left", "Move Right", "Enter", "Back", "Escape" };
        //int selectedControlsMenuIndex = 0;




        string[] GetUpdatedControlsMenuOptions()
        {
            // Create a new array to hold the updated menu options
            string[] updatedControlsMenuOptions = new string[controlsMenuOptions.Length];

            // Iterate over the controlsMenuOptions array
            for (int i = 0; i < controlsMenuOptions.Length; i++)
            {
                // Check if the current index is the selected index
                if (i == selectedControlsMenuIndex)
                {
                    // Add the selected option indicators around the selected option
                    updatedControlsMenuOptions[i] = $"> {controlsMenuOptions[i]} <";
                }
                else
                {
                    // Copy the non-selected option
                    updatedControlsMenuOptions[i] = controlsMenuOptions[i];
                }
            }

            return updatedControlsMenuOptions;
        }



        string[] GetUpdatedOptionsMenuOptions()
        {
            // Create a new array to hold the updated menu options
            string[] updatedOptionsMenuOptions = new string[optionsMenuOptions.Length];

            // Iterate over the optionsMenuOptions array
            for (int i = 0; i < optionsMenuOptions.Length; i++)
            {
                // Check if the current index is the selected index
                if (i == selectedOptionsMenuIndex)
                {
                    // Add the selected option indicators around the selected option
                    updatedOptionsMenuOptions[i] = $"> {optionsMenuOptions[i]} <";
                }
                else
                {
                    // Copy the non-selected option
                    updatedOptionsMenuOptions[i] = optionsMenuOptions[i];
                }
            }

            return updatedOptionsMenuOptions;
        }




        void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Handle MainMenu
            if (gameState == GameState.MainMenu && isMainMenuVisible)
            {
                if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up))
                {
                    SelectedMainMenuIndex = (SelectedMainMenuIndex - 1 + menuOptions.Length) % menuOptions.Length;
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down))
                {
                    SelectedMainMenuIndex = (SelectedMainMenuIndex + 1) % menuOptions.Length;
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    switch (SelectedMainMenuIndex)
                    {
                        case 0:
                            // Start Game
                            gameState = GameState.GamePlay;
                            isMainMenuVisible = false;
                            InitGamePlay();
                            System.Threading.Thread.Sleep(200);  // Pause briefly
                            isPressKeyPromptVisible = true; // Show the prompt when the game starts
                            isBlackFadeActive = false;
                            blackFadeTimer = 0f;
                            blackFadeAlpha = 1.0f;

                            // Initialize gameplay
                            break;
                        case 1:
                            // Start Game
                            gameState = GameState.ControlsMenu;
                            isMainMenuVisible = false;
                            isControlsMenuVisible = true;

                            InitControlsMenu();  // Initialize gameplay
                            break;
                        case 2:
                            // Show Options Menu
                            gameState = GameState.OptionsMenu;

                            isMainMenuVisible = false;
                            isOptionsMenuVisible = true;
                            selectedOptionsMenuIndex = 0;

                            break;
                        case 3:
                            Exit();
                            break;
                    }
                }
            }



            // Handle OptionsMenu
            else if (gameState == GameState.ControlsMenu && isControlsMenuVisible)
            {
                if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up))
                {
                    selectedControlsMenuIndex = (selectedControlsMenuIndex - 1 + controlsMenuOptions.Length) % controlsMenuOptions.Length;
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down))
                {
                    selectedControlsMenuIndex = (selectedControlsMenuIndex + 1) % controlsMenuOptions.Length;
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    switch (selectedControlsMenuIndex)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            // Do nothing for these options
                            break;
                        case 5:
                            // Go back to Main Menu
                            isOptionsMenuVisible = false;
                            isMainMenuVisible = true;
                            SelectedMainMenuIndex = 0;
                            gameState = GameState.MainMenu;
                            break;
                    }
                }
            }


            //bool isOptionsMenuVisible = false;
            //string[] optionsMenuOptions = { "Color Picker Options", "Back" };
            //int selectedOptionsMenuIndex = 0;



            else if (gameState == GameState.OptionsMenu && isOptionsMenuVisible)
            {
                if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up))
                {
                    selectedOptionsMenuIndex = (selectedOptionsMenuIndex - 1 + optionsMenuOptions.Length) % optionsMenuOptions.Length;
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down))
                {
                    selectedOptionsMenuIndex = (selectedOptionsMenuIndex + 1) % optionsMenuOptions.Length;
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    switch (selectedOptionsMenuIndex)
                    {
                        case 0:
                            // Placeholder action for "Graphics"

                            InitBackgroundColorPicker();
                            break;
                        case 1:
                            // Go back to Main Menu
                            isOptionsMenuVisible = false;
                            isMainMenuVisible = true;
                            SelectedMainMenuIndex = 0;

                            gameState = GameState.MainMenu;
                            break;
                    }
                }
            }

            else if (isPressKeyPromptVisible && keyboardState.GetPressedKeys().Length > 0)
            {
                System.Threading.Thread.Sleep(20);  // Pause briefly

                isPressKeyPromptVisible = false; // Hide the prompt
            }

            else if (gameState == GameState.GamePlay)
            {
                // Handle gameplay input
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    snake.Vel.X = -1;
                    snake.Vel.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    snake.Vel.X = 1;
                    snake.Vel.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    snake.Vel.Y = -1;
                    snake.Vel.X = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    snake.Vel.Y = 1;
                    snake.Vel.X = 0;
                }

                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    // Optionally pause or exit gameplay
                    flashText = ""; // Clear any previous flash text
                    isFlashing = false; // Stop flashing
                    gameState = GameState.MainMenu;
                    isMainMenuVisible = true;
                }
            }
            // Handle ControlsMenu Menu
            //else if (gameState == GameState.ControlsMenu)
            //{
            //    if (keyboardState.IsKeyDown(Keys.Back))
            //    {
            //        System.Threading.Thread.Sleep(500);  // Pause briefly
            //        InitMainMenu();  // Go back to main menu
            //    }
            //    if (keyboardState.IsKeyDown(Keys.S))
            //    {
            //        InitSkinChooser();  // Initialize skin chooser
            //    }
            //    if (keyboardState.IsKeyDown(Keys.C))
            //    {
            //        InitColorPicker();  // Initialize color picker
            //    }
            //}
            // Handle ColorPicker
            else if (gameState == GameState.ColorPicker)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Back))

                {
                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    InitMainMenu();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.B))
                {
                    InitBackgroundColorPicker();
                }
            }
            else if (gameState == GameState.SkinChooser)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    InitControlsMenu();
                }
            }
            else if (gameState == GameState.ColorConfirm)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    InitMainMenu();  // Go back to main menu
                }
            }
            else if (gameState == GameState.BackgroundColorPicker)
            {

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    // Confirm and apply the background color
                    backgroundColor = background;

                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    gameState = GameState.ColorConfirm; // Transition to the desired state
                }
                else if (keyboardState.IsKeyDown(Keys.Back) && prevKeyboardState.IsKeyUp(Keys.Back))
                {
                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    selectedOptionsMenuIndex = 0;

                    InitOptionsMenu();
                }

                // Handle slider selection with clamping
                if (keyboardState.IsKeyDown(Keys.Up) && prevKeyboardState.IsKeyUp(Keys.Up))
                {
                    selectedSlider = (selectedSlider - 1 + 3) % 3; // Cycling through 0 to 2
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && prevKeyboardState.IsKeyUp(Keys.Down))
                {
                    selectedSlider = (selectedSlider + 1) % 3; // Cycling through 0 to 2
                }
                prevKeyboardState = keyboardState;

                // Update the previous keyboard state at the end of the update method                


                // Handle value adjustment for the selected slider
                if (selectedSlider == 0) // Red Slider
                {
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        xRed = Math.Max(xRed - xRedIncrement, minRed); // Clamp Red value
                        xPosition = Math.Max(xPosition - xPositionIncrement, minX); // Clamp Red slider position
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        xRed = Math.Min(xRed + xRedIncrement, maxRed); // Clamp Red value
                        xPosition = Math.Min(xPosition + xPositionIncrement, maxX); // Clamp Red slider position
                    }
                }
                else if (selectedSlider == 1) // Green Slider
                {
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        xGreen = Math.Max(xGreen - xGreenIncrement, minGreen); // Clamp Green value
                        yPosition = Math.Max(yPosition - YPositionIncrement, minY); // Clamp Green slider position
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        xGreen = Math.Min(xGreen + xGreenIncrement, maxGreen); // Clamp Green value
                        yPosition = Math.Min(yPosition + YPositionIncrement, maxY); // Clamp Green slider position
                    }
                }
                else if (selectedSlider == 2) // Blue Slider
                {
                    if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        xBlue = Math.Max(xBlue - xBlueIncrement, minBlue); // Clamp Blue value
                        zPosition = Math.Max(zPosition - ZPositionIncrement, minZ); // Clamp Blue slider position
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        xBlue = Math.Min(xBlue + xBlueIncrement, maxBlue); // Clamp Blue value
                        zPosition = Math.Min(zPosition + ZPositionIncrement, maxZ); // Clamp Blue slider position
                    }
                }

                // Update the background color based on the slider values
                background = new Color((int)xRed, (int)xGreen, (int)xBlue);
                redBoxPreview = new Color((int)xRed, 0, 0);
                greenBoxPreview = new Color(0, (int)xGreen, 0);
                blueBoxPreview = new Color(0, 0, (int)xBlue);

                // Handle other key presses for navigation or additional functionality
                if (keyboardState.IsKeyDown(Keys.Back) && prevKeyboardState.IsKeyUp(Keys.Back))
                {
                    InitMainMenu();
                }
                if (keyboardState.IsKeyDown(Keys.B) && prevKeyboardState.IsKeyUp(Keys.B))
                {
                    // Add functionality for 'B' key if needed
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                {
                    System.Threading.Thread.Sleep(250); // Pause briefly
                    InitMainMenu(); // Go back to main menu
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && !prevKeyboardState.IsKeyDown(Keys.Enter))
                {
                    // Apply the selected background color
                    backgroundColor = background;
                    gameState = GameState.GamePlay; // Transition to the desired state
                }

                if (keyboardState.IsKeyDown(Keys.Back) && !prevKeyboardState.IsKeyDown(Keys.Back))
                {
                    // Transition back without applying the background color
                    gameState = GameState.MainMenu; // Or appropriate action
                }

                if (keyboardState.IsKeyDown(Keys.Escape) && !prevKeyboardState.IsKeyDown(Keys.Escape))
                {
                    // Handle escape key action
                    gameState = GameState.MainMenu; // Or appropriate action
                }
            }
            else if (gameState == GameState.GameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Y))
                {
                    InitGamePlay();  // Start gameplay
                    flashText = ""; // Clear flash text for a new game
                }
                if (Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    System.Threading.Thread.Sleep(500);  // Pause briefly
                    InitMainMenu();  // Go back to main menu
                }
            }
            prevKeyboardState = Keyboard.GetState();
        }



        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            if (isPressKeyPromptVisible)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds; // Update elapsed time

                // Handle flashing logic
                if (elapsedTime >= flashDuration)
                {
                    isTextVisible = !isTextVisible; // Toggle visibility
                    elapsedTime = 0f; // Reset timer
                }
            }

            else if (gameState == GameState.MainMenu)
            {
                UpdateMainMenu(gameTime);
            }
            else if (gameState == GameState.GamePlay)
            {
                UpdateGamePlay(gameTime);
            }
            else if (gameState == GameState.ControlsMenu)
            {
                UpdateControlsMenu(gameTime);
            }
            else if (gameState == GameState.OptionsMenu)
            {
                UpdateOptionsMenu(gameTime);
            }
            else if (gameState == GameState.Controls2)
            {
                UpdateControls2(gameTime);
            }
            else if (gameState == GameState.ResolutionPicker)
            {
                UpdateResolutionPicker(gameTime);
            }
            else if (gameState == GameState.ColorPicker)
            {
                UpdateColorPicker(gameTime);
            }
            else if (gameState == GameState.SkinChooser)
            {
                UpdateSkinChooser(gameTime);
            }
            else if (gameState == GameState.BackgroundColorPicker)
            {
                UpdateBackgroundColorPicker(gameTime);
            }
            else if (gameState == GameState.ColorConfirm)
            {
                UpdateColorConfirm(gameTime);
            }
            else if (gameState == GameState.GameOver)
            {
                UpdateGameOver(gameTime);
            }

            base.Update(gameTime);
        }

        void UpdateMainMenu(GameTime gameTime)
        {
        }

        private float shuffleIntervalTime = 05f; // Shuffle every 0.1 seconds
        private float teleportDuration = 0.0f; // Duration to stay invisible
        private float teleportTimer = 0f; // Timer for teleporting
        private bool isFadingTail = false;
        private float fadeDuration = 5.0f; // Duration for fading out
        private float invisibleDuration = 5.0f; // Duration for invisibility
        private float fadeInDuration = 5.0f; // Duration for fading in
        private float fadeTimer = 0f; // General timer for handling fade effects
        private bool isTailVisible = true; // Track visibility of the tail

        private float blackFadeAlpha = 1.0f; // Current screen alpha for the black fade
        private float blackFadeDuration = 3.5f; // Duration for fading out (seconds)
        private float blackHoldDuration = 7.0f; // Duration to hold the fade (seconds)
        private float blackFadeInDuration = 3.5f; // Duration for fading back in (seconds)
        private float blackFadeTimer = 0f; // Timer for handling fade effects
        private bool isBlackFadeActive = false; // Flag to check if the fade effect is active


        void UpdateGamePlay(GameTime gameTime)
        {
            // Update food shuffling
            for (int i = 0; i < food.Length; ++i)
            {
                if (food[i].IsShuffling)
                {
                    // Increment the total elapsed time
                    food[i].totalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // If the elapsed time exceeds the shuffle interval, change the color
                    if (food[i].totalElapsedTime >= shuffleIntervalTime)
                    {
                        // Randomly select a new color from the available colors
                        food[i].Type = random.Next(0, foodColorList.Length);
                        food[i].totalElapsedTime = 0f; // Reset the elapsed time
                    }
                }
            }

            // Check if the snake is fading its tail
            if (isFadingTail)
            {
                fadeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Handle fade-out
                if (fadeTimer < fadeDuration)
                {
                    // During fade-out, reduce the alpha of the tail segments
                    float alpha = 1.0f - (fadeTimer / fadeDuration);
                    for (int i = 0; i < snake.TailLength; i++)
                    {
                        snake.TailAlpha[i] = alpha; // Set alpha based on fade duration
                    }
                }
                else if (fadeTimer < fadeDuration + invisibleDuration)
                {
                    // Tail is invisible, set alpha to 0 for all segments
                    for (int i = 0; i < snake.TailLength; i++)
                    {
                        snake.TailAlpha[i] = 0.0f; // Set to invisible
                    }
                }
                else if (fadeTimer < fadeDuration + invisibleDuration + fadeInDuration)
                {
                    // Handle fade-in
                    float alpha = (fadeTimer - (fadeDuration + invisibleDuration)) / fadeInDuration;
                    for (int i = 0; i < snake.TailLength; i++)
                    {
                        snake.TailAlpha[i] = alpha; // Restore fading in
                    }
                }
                else
                {
                    // Reset the fade effect
                    isFadingTail = false;
                    fadeTimer = 0f;

                    // Reset alpha values to fully visible
                    for (int i = 0; i < snake.TailLength; i++)
                    {
                        snake.TailAlpha[i] = 1.0f; // Ensure all segments are fully visible again
                    }
                }
            }

            // Update snake movement
            snake.StepTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (snake.StepTimer >= snake.Speed)
            {
                snake.StepTimer = 0;

                // Shift all tail pieces up one in the array so we can insert the new piece at the beginning
                for (int i = snake.TailLength; i > 0; --i)
                {
                    snake.Tail[i] = snake.Tail[i - 1];
                }
                snake.Tail[0] = snake.Pos; // Add new head position to the tail

                // Update snake position
                snake.Pos.X += snake.Vel.X;
                snake.Pos.Y += snake.Vel.Y;

                // Test for snake head colliding with snake tail
                for (int i = 0; i < snake.TailLength; ++i)
                {
                    if (snake.Pos.X == snake.Tail[i].X && snake.Pos.Y == snake.Tail[i].Y)
                    {
                        System.Threading.Thread.Sleep(2000);
                        InitGameOver();
                        return;
                    }
                }

                // For snake eating default food
                for (int i = 0; i < food.Length; ++i)
                {
                    if (snake.Pos.X == defaultFoodPosX && snake.Pos.Y == defaultFoodPosY)
                    {
                        SpawnDefaultFood();
                        snake.Speed *= 0.99;
                        snake.TailLength++;
                        score += 100;
                        targetCellSize = 8f;

                        foodMunch.Play();

                        if (score > highScore)
                        {
                            highScore = score;
                        }
                    }
                }

                // Test for snake head colliding with food
                for (int i = 0; i < food.Length; ++i)
                {
                    if (snake.Pos.X == food[i].Pos.X && snake.Pos.Y == food[i].Pos.Y)
                    {
                        int foodType = food[i].Type;

                        // Handle food consumption
                        SpawnFood(i); // Respawn the food at a new location

                        switch (foodType)
                        {
                            case 0: // Red Food
                                flashText = "Red Food Eaten!";
                                AddTailSegments(5); // Change from 3 to 5
                                snake.Speed *= 0.95f; // Increase speed
                                break;


                            // Existing cases for other food types...
                            case 1: // Orange
                                flashText = "Tail size decreased!";
                                snake.TailLength = Math.Max(0, snake.TailLength - 5); // Ensure TailLength doesn't go negative
                                snake.Speed *= 01.05f; // Increase speed

                                break;
                            case 2: // Yellow
                                flashText = "Increased speed!";
                                snake.Speed *= 0.8f; // Increase speed by 1.25x
                                snake.TailLength++;

                                AddTailSegments(5); // Change from 3 to 5


                                break;
                            case 3: // Green
                                flashText = "Decreased speed!";
                                snake.TailLength++;

                                snake.Speed *= 01.2f; // Decrease speed to 75% of current speed


                                break;
                            case 4: // Blue
                                flashText = "Bonus Points";
                                snake.TailLength++;

                                score += 1100;

                                break;



                            case 5: // Purple
                                flashText = "Purple Food Eaten!";
                                TeleportSnake();
                                // Add a brief invisibility period after teleportation
                                snake.IsVisible = false;
                                teleportTimer = teleportDuration;
                                break;


                            case 6: // Brown Food
                                flashText = "Brown Food Eaten!";
                                isFadingTail = true; // Start the fading process
                                break;

                            case 7: // Black
                                flashText = "Black Food Eaten!";
                                isBlackFadeActive = true;
                                blackFadeTimer = 0f; // Reset the timer
                                blackFadeAlpha = 1.0f; // Start from fully visible
                                break;
                            
                            default:
                                flashText = "Unknown Food Eaten!";
                                break;
                        }

                        // Play the munch sound effect
                        foodMunch.Play();

                        // Reset flashing timers for the new text
                        elapsedTime = 0f; // Reset timer for flashing cycle
                        flashTimeElapsed = 0f; // Reset total flashing time
                        isFlashing = true; // Start flashing effect
                    }
                }

                // Manage flashing text visibility duration
                if (isFlashing)
                {
                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsedTime >= flashDuration)
                    {
                        isFlashing = false; // Stop flashing after the duration
                        flashText = ""; // Clear the text after it's displayed
                    }
                }
            }

            if (isBlackFadeActive)
            {
                blackFadeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Fade to black over the specified duration
                if (blackFadeTimer < blackFadeDuration)
                {
                    blackFadeAlpha = MathHelper.Lerp(1.0f, 0.1f, blackFadeTimer / blackFadeDuration); // Fade to 0.1f
                }
                else if (blackFadeTimer < blackFadeDuration + blackHoldDuration)
                {
                    // Hold the fade at 0.1f for the specified duration
                    blackFadeAlpha = 0.0f; // Remain at 0.0f
                }
                else if (blackFadeTimer < blackFadeDuration + blackHoldDuration + blackFadeInDuration)
                {
                    // Fade back in
                    blackFadeAlpha = MathHelper.Lerp(0.1f, 1.0f, (blackFadeTimer - (blackFadeDuration + blackHoldDuration)) / blackFadeInDuration);
                }
                else
                {
                    // Reset the fade effect and timer
                    isBlackFadeActive = false; // Reset the fade effect
                    blackFadeTimer = 0f; // Reset the timer
                    blackFadeAlpha = 1.0f; // Reset to fully visible
                }
            }

            if (!snake.IsVisible)
            {
                teleportTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (teleportTimer <= 0)
                {
                    snake.IsVisible = true;
                }
            }

            void TeleportSnake()
            {
                // Generate new random coordinates for the snake's position
                int newX = random.Next(0, worldSize.X);
                int newY = random.Next(0, worldSize.Y);

                // Set the new position
                snake.Pos.X = newX;
                snake.Pos.Y = newY;

                // Preserve the current tail length
                int currentTailLength = snake.TailLength;

                // Reset the tail positions
                for (int i = 0; i < currentTailLength; i++)
                {
                    snake.Tail[i] = new Point(newX, newY);
                    snake.TailAlpha[i] = 1.0f; // Reset alpha to fully visible
                }

                // Ensure TailLength remains the same
                snake.TailLength = currentTailLength;

                // Make the snake temporarily invisible
                snake.IsVisible = false;
                teleportTimer = teleportDuration;
            }

            void RegrowTail()
            {
                // Keep the tail length the same, but reset the tail segments
                // We can set the tail segments based on the new position and the current length
                for (int i = 0; i < snake.TailLength; i++)
                {
                    // Regrow the tail segments from the new head position
                    snake.Tail[i] = new Point(snake.Pos.X, snake.Pos.Y);
                }

                // Make the snake visible again
                snake.IsVisible = true;
            }


            void AddTailSegments(int segments)
            {
                // Ensure the Tail array can accommodate the new segments
                if (snake.TailLength + segments > snake.Tail.Length)
                {
                    Array.Resize(ref snake.Tail, snake.TailLength + segments);
                    Array.Resize(ref snake.TailAlpha, snake.TailLength + segments); // Resize TailAlpha array
                }

                // Shift all tail pieces up in the array to make room for new segments at the front
                for (int i = snake.TailLength + segments - 1; i >= segments; --i)
                {
                    snake.Tail[i] = snake.Tail[i - segments];
                    snake.TailAlpha[i] = snake.TailAlpha[i - segments]; // Shift alpha values too
                }

                // Insert new segments at the beginning based on the current position
                for (int j = 0; j < segments; j++)
                {
                    snake.Tail[j] = new Point(snake.Pos.X, snake.Pos.Y); // Use current position
                    snake.TailAlpha[j] = 1.0f; // Set alpha to fully visible for new segments
                }

                // Update the TailLength to account for the newly added segments
                snake.TailLength += segments;
            }


            for (int i = 0; i < food.Length; ++i)
            {
                if (snake.Pos.X == food[i].Pos.X && snake.Pos.Y == food[i].Pos.Y)
                {
                    // Handle food consumption
                    SpawnFood(i);

                    snake.TailLength++;
                    // foodMunch.Play();


                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


                    // Calculate alpha using a sine wave function
                    float alpha = (float)(Math.Sin(elapsedTime * Math.PI * 2 / flashDuration) * 0.5 + 0.5);
                    textColor = new Color(1f, 1f, 1f, alpha); // White color with oscillating alpha

                    // Reset elapsedTime to ensure continuous flashing
                    if (elapsedTime > flashDuration)
                    {
                        isTextVisible = !isTextVisible;
                        elapsedTime -= flashDuration; // Keeps the flashing cycle going
                    }

                    if (flashTimeElapsed >= totalFlashTime)
                    {
                        isFlashing = false;
                        flashText = ""; // Optionally clear the text
                        textColor = Color.Transparent; // Ensure text is hidden
                    }

                    if (isFading)
                    {
                        if (fadingIn)
                        {
                            fadeAlpha -= fadeSpeed * elapsed;
                            if (fadeAlpha <= 0.0f)
                            {
                                fadeAlpha = 0.0f;
                                isFading = false; // Stop fading
                            }
                        }
                        else
                        {
                            fadeAlpha += fadeSpeed * elapsed;
                            if (fadeAlpha >= 1.0f)
                            {
                                fadeAlpha = 1.0f;
                                fadingIn = true; // Set to fade back in after fading out
                            }
                        }
                    }

                    if (isFlashing)
                    {
                        // Update elapsed time for current flash cycle
                        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        // Update total time for the flashing effect
                        flashTimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Toggle text visibility based on elapsed time
                        if (elapsedTime >= flashDuration)
                        {
                            isTextVisible = !isTextVisible;
                            elapsedTime = 0f; // Reset timer for the flashing cycle
                        }

                        textColor = isTextVisible ? Color.White : Color.Transparent; // Toggle between visible and transparent

                        // Stop flashing after totalFlashTime has elapsed
                        if (flashTimeElapsed >= totalFlashTime)
                        {
                            isFlashing = false;
                            flashText = ""; // Optionally clear the text
                            textColor = Color.Transparent; // Ensure text is hidden
                        }
                    }
                }
                // handle snake teleporting over edges
                if (snake.Pos.X < 0)
                {
                    snake.Pos.X = worldSize.X - 1;
                }
                if (snake.Pos.Y < 0)
                {
                    snake.Pos.Y = worldSize.Y - 1;
                }
                if (snake.Pos.X >= worldSize.X)
                {
                    snake.Pos.X = 0;
                }
                if (snake.Pos.Y >= worldSize.Y)
                {
                    snake.Pos.Y = 0;
                }
            }
        }

        void UpdateControlsMenu(GameTime gameTime)
        {
        }

        void UpdateOptionsMenu(GameTime gameTime)
        {
        }

        void UpdateControls2(GameTime gameTime)
        {
        }

        void UpdateResolutionPicker(GameTime gameTime)
        {
        }

        void UpdateColorPicker(GameTime gameTime)
        {
            backgroundColor = background;

            int red = (int)xPosition;

            // Convert the integer to a string
            string redString = red.ToString();

            xRed = MathHelper.Clamp(xRed, minRed, maxRed);
            base.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                backgroundColor = background;
            }
        }

        void UpdateSkinChooser(GameTime gameTime)
        {
        }

        void UpdateBackgroundColorPicker(GameTime gameTime)
        {
        }

        void UpdateColorConfirm(GameTime gameTime)
        {
        }

        void UpdateGameOver(GameTime gameTime)
        {
            if (gameState == GameState.GamePlay)

            {
                StopFlashing();
                // Additional game over logic here
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor); // Clear with the current backgroundColor

            switch (gameState)
            {
                case GameState.MainMenu:
                    DrawMainMenu(gameTime);
                    break;
                case GameState.GamePlay:
                    DrawGamePlay(gameTime);
                    break;
                case GameState.ControlsMenu:
                    DrawControlsMenu(gameTime);
                    break;
                case GameState.OptionsMenu:
                    DrawOptionsMenu(gameTime);
                    break;
                case GameState.Controls2:
                    DrawControls2(gameTime);
                    break;
                case GameState.ColorPicker:
                    DrawColorPicker(gameTime);
                    break;
                case GameState.SkinChooser:
                    DrawSkinChooser(gameTime);
                    break;
                case GameState.BackgroundColorPicker:
                    DrawBackgroundColorPicker(gameTime);
                    break;
                case GameState.ColorConfirm:
                    DrawColorConfirm(gameTime);
                    break;
                case GameState.GameOver:
                    DrawGameOver(gameTime);
                    break;
                default:
                    // Optionally handle unexpected game states
                    break;
            }

            base.Draw(gameTime);
        }


        private void DrawPressAnyKeyPrompt(GameTime gameTime)
        {
            if (!isPressKeyPromptVisible)
                return; // Exit the method if the prompt is not visible

            // Define box dimensions
            int boxWidth = GraphicsDevice.Viewport.Width; // Leave some margin
            int boxHeight = 50; // Height of the box
            int boxX = (GraphicsDevice.Viewport.Width - boxWidth) / 2; // Center the box horizontally
            int boxY = (GraphicsDevice.Viewport.Height - boxHeight) / 2 - 75; // Center the box vertically

            // Draw the semi-transparent box
            spriteBatch.Draw(texture, new Rectangle(boxX, boxY, boxWidth, boxHeight), Color.Black * 0.5f); // Adjust color and transparency as needed

            // Flashing text logic
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds; // Update elapsed time
            Color flashingColor = GetFlashingColor(gameTime, Color.White, Color.Gray); // Get the flashing color

            // Draw the text
            string text = "Press Any Key";
            Vector2 textSize = spriteFont.MeasureString(text);
            Vector2 textPosition = new Vector2(boxX + (boxWidth - textSize.X) / 2 - 20, boxY + (boxHeight - textSize.Y) / 2 - 5); // Center the text in the box

            // Set the scale and thickness for bold effect
            float textScale = 1.5f;
            float thickness = 1.5f; // Thickness for the bold effect
            Vector2 shadowOffset = new Vector2(3f, 3f); // Shadow offset

            // Draw the shadow
            spriteBatch.DrawString(spriteFont, text, textPosition + shadowOffset, Color.Black * 0.5f, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            // Draw the main text with thickness for bold effect
            for (float offset = -thickness; offset <= thickness; offset += thickness)
            {
                if (offset != 0) // Skip the center position
                {
                    spriteBatch.DrawString(spriteFont, text, textPosition + new Vector2(offset, 0), flashingColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, text, textPosition + new Vector2(0, offset), flashingColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }
            }

            // Finally, draw the main text over the shadow to complete the bold effect
            spriteBatch.DrawString(spriteFont, text, textPosition, flashingColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }

        void DrawMainMenu(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (isMainMenuVisible)
            {
                UpdateTextureFadeEffect(gameTime);

                // Draw menu background with a semi-transparent black rectangle
                spriteBatch.Draw(texture, new Rectangle(450, 355, 375, 335), Color.Black * 0.4f);

                // Calculate the flashing color for the selected menu option
                Color flashingColor = GetFlashingColor(gameTime, Color.Green, Color.DarkGreen);

                // Get the updated menu options with indicators
                string[] updatedMenuOptions = GetUpdatedMenuOptions();

                // Loop through each updated menu option to draw it on the screen
                for (int i = 0; i < updatedMenuOptions.Length; i++)
                {
                    // Determine the color of the menu option:
                    // If it's the selected option, use the flashing color, otherwise, use a darker green
                    Color color = i == SelectedMainMenuIndex ? flashingColor : Color.DarkOliveGreen;

                    // Set the position where the text will be drawn
                    Vector2 textPosition = new Vector2(490, 380 + i * 60);

                    // Set the scale to make the text larger; 1.5 times the original size
                    float textScale = 1.5f;

                    // Set the thickness for the bold effect by drawing multiple text layers slightly offset
                    float thickness = 1.5f;

                    // Set the offset for creating the 3D shadow effect
                    Vector2 shadowOffset = new Vector2(3f, 3f);

                    // Draw the shadow by drawing the text slightly offset to the bottom-right
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition + shadowOffset, Color.Black * 0.5f, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                    // Draw the text multiple times with slight offsets to create a thicker, bolder effect
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition + new Vector2(-thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition + new Vector2(thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition + new Vector2(0, -thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition + new Vector2(0, thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                    // Draw the main text in the center to complete the bold effect
                    spriteBatch.DrawString(mainMenuTextFont, updatedMenuOptions[i], textPosition, color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    // Draw additional textures



                    Color textureColor = Color.White * textureFadeAlpha; // Apply fade effect
                    spriteBatch.Draw(texPressEnter, new Vector2(515, 635), textureColor);


                    spriteBatch.Draw(texSnakeArt, new Vector2(450, -10), Color.White);
                    spriteBatch.Draw(texSnakeGameText, new Vector2(435, 225), Color.White);

                }

                // End the sprite batch
                spriteBatch.End();
            }
        }


        void DrawControlsMenu(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (isControlsMenuVisible)
            {
                UpdateTextureFadeEffect(gameTime);

                // Draw menu background with a semi-transparent black rectangle
                spriteBatch.Draw(texture, new Rectangle(450, 285, 375, 415), Color.Black * 0.4f);

                // Calculate the flashing color for the selected menu option
                Color flashingColor = GetFlashingColor(gameTime, Color.Green, Color.DarkGreen);

                // Get the updated menu options with indicators
                string[] updatedControlsMenuOptions = GetUpdatedControlsMenuOptions();

                // Loop through each updated menu option to draw it on the screen
                for (int i = 0; i < updatedControlsMenuOptions.Length; i++)
                {
                    // Determine the color of the menu option:
                    // If it's the selected option, use the flashing color, otherwise, use a darker green
                    Color color = i == selectedControlsMenuIndex ? flashingColor : Color.DarkOliveGreen;

                    // Set the position where the text will be drawn
                    Vector2 textPosition = new Vector2(490, 300 + i * 60);

                    // Set the scale to make the text larger; 1.5 times the original size
                    float textScale = 1.5f;

                    // Set the thickness for the bold effect by drawing multiple text layers slightly offset
                    float thickness = 1.5f;

                    // Set the offset for creating the 3D shadow effect
                    Vector2 shadowOffset = new Vector2(3f, 3f);

                    // Draw the shadow by drawing the text slightly offset to the bottom-right
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition + shadowOffset, Color.Black * 0.5f, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                    // Draw the text multiple times with slight offsets to create a thicker, bolder effect
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition + new Vector2(-thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition + new Vector2(thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition + new Vector2(0, -thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition + new Vector2(0, thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                    // Draw the main text in the center to complete the bold effect
                    spriteBatch.DrawString(mainMenuTextFont, updatedControlsMenuOptions[i], textPosition, color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }

                // Draw the "Press Enter" texture outside of the loop to ensure it's only drawn once
                Color textureColor = Color.White * textureFadeAlpha; // Apply fade effect
                spriteBatch.Draw(texPressEnter, new Vector2(515, 650), textureColor);

                // You can add additional textures here if needed
                // For example:
                // spriteBatch.Draw(texSomeOtherTexture, new Vector2(x, y), Color.White);

                spriteBatch.End();
            }
        }
        void DrawControls2(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(mainMenuTextFont, "Resolution Picker", new Vector2(375, 100), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, "Previous Page", new Vector2(375, 585), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, "Main Menu", new Vector2(375, 675), Color.White);

            spriteBatch.Draw(texControlsText, new Vector2(550, 0), Color.White);

            spriteBatch.Draw(texR, new Vector2(674, 80), Color.White);
            spriteBatch.Draw(texBack, new Vector2(665, 570), Color.White);
            spriteBatch.Draw(texEsc, new Vector2(674, 660), Color.White);

            spriteBatch.Draw(texture, new Rectangle(673, worldScreenTopY, 1, GraphicsDevice.Viewport.Height), Color.Black);
            spriteBatch.Draw(texture, new Rectangle(374, worldScreenTopY, 1, GraphicsDevice.Viewport.Height), Color.Black);

            spriteBatch.End();
        }

        void DrawOptionsMenu(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (isOptionsMenuVisible)
            {
                UpdateTextureFadeEffect(gameTime);

                // Draw menu background with a semi-transparent black rectangle
                spriteBatch.Draw(texture, new Rectangle(375, 400, 500, 250), Color.Black * 0.4f);

                // Get the updated menu options with indicators
                string[] updatedOptionsMenu = GetUpdatedOptionsMenuOptions();

                // Calculate the flashing color for the selected menu option
                Color flashingColor = GetFlashingColor(gameTime, Color.Green, Color.DarkGreen);

                // Loop through each menu option to draw it on the screen
                for (int i = 0; i < updatedOptionsMenu.Length; i++)
                {
                    // Determine the color of the menu option:
                    // If it's the selected option, use the flashing color, otherwise, use a darker green
                    Color color = i == selectedOptionsMenuIndex ? flashingColor : Color.DarkOliveGreen;

                    // Set the position where the text will be drawn
                    Vector2 textPosition = new Vector2(425, 425 + i * 60);

                    // Set the scale to make the text larger; 1.5 times the original size
                    float textScale = 1.5f;

                    // Set the thickness for the bold effect by drawing multiple text layers slightly offset
                    float thickness = 1.5f;

                    // Set the offset for creating the 3D shadow effect
                    Vector2 shadowOffset = new Vector2(3f, 3f);

                    Color textureColor = Color.White * textureFadeAlpha; // Apply fade effect
                    spriteBatch.Draw(texPressEnter, new Vector2(500, 600), textureColor);

                    // Draw the shadow by drawing the text slightly offset to the bottom-right
                    spriteBatch.DrawString(mainMenuTextFont, updatedOptionsMenu[i], textPosition + shadowOffset, Color.Black * 0.5f, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

                    // Draw the text multiple times with slight offsets to create a thicker, bolder effect
                    spriteBatch.DrawString(mainMenuTextFont, updatedOptionsMenu[i], textPosition + new Vector2(-thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedOptionsMenu[i], textPosition + new Vector2(thickness, 0), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedOptionsMenu[i], textPosition + new Vector2(0, -thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(mainMenuTextFont, updatedOptionsMenu[i], textPosition + new Vector2(0, thickness), color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
                }

                // End the sprite batch
                spriteBatch.End();
            }
        }









        void DrawColorPicker(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw the background with the current background color
            spriteBatch.Draw(texture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), backgroundColor);

            // Draw UI elements
            spriteBatch.DrawString(mainMenuTextFont, "Background color", new Vector2(450, 260), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, "Back", new Vector2(500, 660), Color.White);
            spriteBatch.Draw(texColorPickerText, new Vector2(405, 50), Color.White);
            spriteBatch.Draw(texBack, new Vector2(675, 650), Color.White);
            spriteBatch.Draw(texB, new Vector2(700, 240), Color.White);

            spriteBatch.End();
        }


        void DrawSkinChooser(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw skin selector background (assuming texSkinSelector is loaded)
            spriteBatch.Draw(texSkinSelector, new Vector2(300, 50), Color.White);

            // Draw the current texture based on currentTextureIndex
            if (textures.Length > 0 && currentTextureIndex >= 0 && currentTextureIndex < textures.Length)
            {
                spriteBatch.Draw(textures[currentTextureIndex], new Vector2(100, 200), Color.White);
            }

            // Handle key press to change currentTextureIndex
            KeyboardState keyboardState = Keyboard.GetState();

            // Move to the previous texture index (decrement)
            if (keyboardState.IsKeyDown(Keys.Left) && !prevKeyboardState.IsKeyDown(Keys.Left))
            {
                {
                    colorIndex++;
                    if (colorIndex >= textures.Length)
                    {
                        colorIndex = 0;
                    }
                }
                currentTextureIndex--;
                if (currentTextureIndex < 0)
                    currentTextureIndex = textures.Length - 1; // Wrap around to the last texture
            }

            // Move to the next texture index (increment)
            if (keyboardState.IsKeyDown(Keys.Right) && !prevKeyboardState.IsKeyDown(Keys.Right))
            {
                {
                    colorIndex++;
                    if (colorIndex >= textures.Length)
                    {
                        colorIndex = 0;
                    }
                }
                currentTextureIndex++;
                if (currentTextureIndex >= textures.Length)
                    currentTextureIndex = 0; // Wrap around to the first texture
            }

            prevKeyboardState = keyboardState; // Update previous keyboard state

            spriteBatch.DrawString(mainMenuTextFont, "Back", new Vector2(500, 660), Color.White);
            spriteBatch.Draw(texBack, new Vector2(675, 650), Color.White);
            spriteBatch.End();
        }

        void DrawBackgroundColorPicker(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(backgroundColor); // Use the updated backgroundColor

            spriteBatch.Draw(texture, new Rectangle(485, 125, 300, 150), background); // Use the current background color
            spriteBatch.DrawString(mainMenuTextFont, "Back", new Vector2(500, 660), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, "Apply Changes", new Vector2(435, 590), Color.White);

            spriteBatch.Draw(texBackgroundColorText, new Vector2(405, 20), Color.White);
            spriteBatch.Draw(texture, new Rectangle(485, 125, 300, 150), background);

            spriteBatch.Draw(texture, new Rectangle(943, 330, 50, 50), redBoxPreview);
            spriteBatch.Draw(texture, new Rectangle(943, 420, 50, 50), greenBoxPreview);
            spriteBatch.Draw(texture, new Rectangle(943, 510, 50, 50), blueBoxPreview);

            spriteBatch.Draw(texSliderBarLong, new Vector2(374, 335), Color.White);
            spriteBatch.Draw(texSliderBarLong, new Vector2(374, 425), Color.White);
            spriteBatch.Draw(texSliderBarLong, new Vector2(374, 515), Color.White);

            spriteBatch.Draw(texRedSliderBar, new Vector2(xPosition, 334), Color.White);
            spriteBatch.Draw(texGreenSliderBar, new Vector2(yPosition, 425), Color.White);
            spriteBatch.Draw(texBlueSliderBar, new Vector2(zPosition, 515), Color.White);

            Vector2 borderPosition = new Vector2(366, 329); // Default to Red Slider
            if (selectedSlider == 1) // Green Slider
            {
                borderPosition = new Vector2(374, 425);
            }
            else if (selectedSlider == 2) // Blue Slider
            {
                borderPosition = new Vector2(374, 515);
            }

            spriteBatch.Draw(texSlideBarBorder, borderPosition, Color.White);

            spriteBatch.DrawString(mainMenuTextFont, $"Red Value: {xRed}", new Vector2(175, 345), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainMenuTextFont, $"Green Value: {xGreen}", new Vector2(170, 435), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainMenuTextFont, $"Blue Value: {xBlue}", new Vector2(175, 525), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(texBack, new Vector2(625, 650), Color.White);
            spriteBatch.Draw(texEnter, new Vector2(635, 580), Color.White);
            spriteBatch.End();
        }

        void DrawColorConfirm(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texChangeConfirm, new Vector2((GraphicsDevice.Viewport.Width - texChangeConfirm.Width) / 2, (GraphicsDevice.Viewport.Height - texChangeConfirm.Height) / 2), Color.White);
            spriteBatch.End();
        }

        void DrawGameOver(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw the Game Over text
            spriteBatch.Draw(texGameOverText, new Vector2(375, 320), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, "Retry?", new Vector2(550, 450), Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainMenuTextFont, "Yes", new Vector2(350, 550), Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            spriteBatch.DrawString(mainMenuTextFont, "No", new Vector2(650, 550), Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);

            // Draw Y and N indicators
            spriteBatch.Draw(texY, new Vector2(450, 550), Color.White);
            spriteBatch.Draw(texN, new Vector2(750, 550), Color.White);

            // Draw flash text if it exists
            if (isFlashing && !string.IsNullOrEmpty(flashText))
            {
                // Position the flash text appropriately on the Game Over screen
                Vector2 flashTextPosition = new Vector2(400, 600); // Adjust the position as needed
                spriteBatch.DrawString(mainMenuTextFont, flashText, flashTextPosition, Color.White);
            }

            spriteBatch.End();
        }

        void DrawGamePlay(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor); // Clear with the current backgroundColor

            spriteBatch.Begin();

            DrawPressAnyKeyPrompt(gameTime);
            float fadeFactor = isBlackFadeActive ? blackFadeAlpha : 1f;


            // Draw score information
            spriteBatch.DrawString(mainMenuTextFont, $"Score: {score}", new Vector2(10, -2), Color.White);
            spriteBatch.DrawString(mainMenuTextFont, $"High Score: {highScore}", new Vector2(GraphicsDevice.Viewport.Width / 2 + 400, -2), Color.White);
            spriteBatch.Draw(texture, new Rectangle(0, worldScreenTopY, GraphicsDevice.Viewport.Width, 1), Color.White);

            // Draw the snake's head only if it's visible
            if (snake.IsVisible)
            {
                spriteBatch.Draw(texture, new Rectangle((int)(snake.Pos.X * cellSize), (int)(snake.Pos.Y * cellSize + worldScreenTopY), cellSize, cellSize), Color.Yellow);

                // Draw the snake's tail only if it's visible
                if (isTailVisible)
                {
                    for (int i = 0; i < snake.TailLength; i++)
                    {
                        Color tailColor = snakTailColorPresets[currentTextureIndex][i % snakTailColorPresets[currentTextureIndex].Length]; // Use color preset
                        spriteBatch.Draw(texture,
                            new Rectangle(snake.Tail[i].X * cellSize, snake.Tail[i].Y * cellSize + worldScreenTopY, cellSize, cellSize),
                            tailColor * snake.TailAlpha[i]); // Apply alpha value to the color
                    }
                }
            }

            // Draw food items
            for (int i = 0; i < food.Length; ++i)
            {
                Color foodColor = foodColorList[food[i].Type];
                spriteBatch.Draw(texture, new Rectangle(food[i].Pos.X * cellSize, food[i].Pos.Y * cellSize + worldScreenTopY, cellSize, cellSize), foodColor);
            }

            // Draw the default food
            spriteBatch.Draw(texture, new Rectangle(defaultFoodPosX * cellSize, defaultFoodPosY * cellSize + worldScreenTopY, cellSize, cellSize), Color.White);

            // Draw grid lines
            for (int x = 0; x < GraphicsDevice.Viewport.Width; x += cellSize)
            {
                spriteBatch.Draw(texture, new Rectangle(x, worldScreenTopY, 1, GraphicsDevice.Viewport.Height), Color.Black);
            }

            for (int y = worldScreenTopY; y < GraphicsDevice.Viewport.Height; y += cellSize)
            {
                spriteBatch.Draw(texture, new Rectangle(0, y, GraphicsDevice.Viewport.Width, 1), Color.Black);
            }


            Color fadeColor = Color.White;
            if (isBlackFadeActive)
            {
                fadeColor = Color.White * blackFadeAlpha;
            }

            if (isBlackFadeActive)
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                                 Color.Black * (1.0f - blackFadeAlpha));
            }

            // Draw fade effect if applicable
            if (isFading)
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black * fadeAlpha);
            }

            if (snake.IsVisible)
            {
                // Draw the snake's head
                Color headColor = Color.Yellow * fadeFactor;
                spriteBatch.Draw(texture,
                    new Rectangle((int)(snake.Pos.X * cellSize), (int)(snake.Pos.Y * cellSize + worldScreenTopY), cellSize, cellSize),
                    headColor);

                // Draw the snake's tail
                for (int i = 0; i < snake.TailLength; i++)
                {
                    Color tailColor = snakTailColorPresets[currentTextureIndex][i % snakTailColorPresets[currentTextureIndex].Length];
                    tailColor = new Color(
                        (int)(tailColor.R * snake.TailAlpha[i] * fadeFactor),
                        (int)(tailColor.G * snake.TailAlpha[i] * fadeFactor),
                        (int)(tailColor.B * snake.TailAlpha[i] * fadeFactor),
                        (int)(tailColor.A * snake.TailAlpha[i] * fadeFactor)
                    );
                    spriteBatch.Draw(texture,
                        new Rectangle(snake.Tail[i].X * cellSize, snake.Tail[i].Y * cellSize + worldScreenTopY, cellSize, cellSize),
                        tailColor);
                }
            }



            // Draw flashing text if applicable
            if (isFlashing && !string.IsNullOrEmpty(flashText))
            {
                spriteBatch.DrawString(spriteFont, flashText, textPosition, textColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
        }
    }
}