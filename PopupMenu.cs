using System;

public class PopupMenu
{
    private bool isVisible;
    private string[] options;
    private int selectedIndex;

    public PopupMenu()
    {
        // Initialize menu options and selected index
        options = new string[] { "Option 1", "Option 2", "Option 3" };
        selectedIndex = 0;
        isVisible = false;
    }

    public void Show()
    {
        isVisible = true;
    }

    public void Hide()
    {
        isVisible = false;
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
    }

    public void Update(KeyboardState keyboardState)
    {
        if (!isVisible) return;

        if (keyboardState.IsKeyDown(Keys.Up))
        {
            selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
        }
        else if (keyboardState.IsKeyDown(Keys.Down))
        {
            selectedIndex = (selectedIndex + 1) % options.Length;
        }

        // Handle selection (e.g., Enter key)
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            // Implement action based on selected option
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!isVisible) return;

        // Draw menu background (optional)
        spriteBatch.DrawRectangle(new Rectangle(100, 100, 200, 150), Color.Black * 0.5f);

        // Draw menu options
        for (int i = 0; i < options.Length; i++)
        {
            Color color = i == selectedIndex ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, options[i], new Vector2(110, 110 + i * 30), color);
        }
    }
}