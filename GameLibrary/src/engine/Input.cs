using Raylib_cs;

namespace GameLibrary;

public enum InputAction
{
    Up,
    Down,
    Left,
    Right,
    Primary,
    Secondary
}

public static class Input
{
    private static Dictionary<InputAction, Binding> _bindings = new Dictionary<InputAction, Binding>()
    {
         { InputAction.Up,        new Binding([KeyboardKey.Up, KeyboardKey.W], [GamepadButton.LeftFaceUp])},
         { InputAction.Down,      new Binding([KeyboardKey.Down, KeyboardKey.S], [GamepadButton.LeftFaceDown])},
         { InputAction.Left,      new Binding([KeyboardKey.Left, KeyboardKey.A], [GamepadButton.LeftFaceLeft])},
         { InputAction.Right,     new Binding([KeyboardKey.Right, KeyboardKey.D], [GamepadButton.LeftFaceRight])},
         { InputAction.Primary,   new Binding([KeyboardKey.Z, KeyboardKey.Space], [GamepadButton.RightFaceDown])},
         { InputAction.Secondary, new Binding([KeyboardKey.X, KeyboardKey.LeftShift], [GamepadButton.RightFaceRight])}
    };

    public static bool Held(InputAction action) => _bindings[action].Held();
    public static bool Pressed(InputAction action) => _bindings[action].Pressed();
    public static bool Released(InputAction action) => _bindings[action].Released();
    
    public class Binding
    {
        public KeyboardKey[] Keys;
        public GamepadButton[] Buttons;

        public Binding(KeyboardKey[] keys, GamepadButton[] buttons)
        {
            Keys = keys;
            Buttons = buttons;
        }

        public bool Held() =>
            Keys.Any(b => Raylib.IsKeyDown(b)) || 
            Buttons.Any(b => Raylib.IsGamepadButtonDown(0, b));
        
        public bool Pressed() =>
            Keys.Any(b => Raylib.IsKeyPressed(b)) || 
            Buttons.Any(b => Raylib.IsGamepadButtonPressed(0, b));
        
        public bool Released() =>
            Keys.Any(b => Raylib.IsKeyReleased(b)) || 
            Buttons.Any(b => Raylib.IsGamepadButtonReleased(0, b));
    }
}