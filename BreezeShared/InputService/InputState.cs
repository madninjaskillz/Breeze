using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Breeze.Services.InputService
{
    public partial class InputService
    {
        internal class InputState
        {
            internal GamePadState[] gamepadState = new GamePadState[4];
            internal MouseState mouseState;
            internal int ScrollPos = 0;
            internal int HistoricScrollPos = 0;
            internal KeyboardState keyboardState;

            public List<Keys> PressedKeys;

            public static InputState GetState()
            {
                InputState result = new InputState();
                for (int i = 0; i < 4; i++)
                {
                    result.gamepadState[i] = GamePad.GetState(i);
                }

                result.mouseState = Mouse.GetState();

                result.ScrollPos = result.mouseState.ScrollWheelValue;
                result.keyboardState = Keyboard.GetState();

                result.PressedKeys = result.keyboardState.GetPressedKeys().ToList();

                var dummy = result.keyboardState.GetPressedKeys();

                return result;
            }

            internal InputState CloneMe()
            {
                InputState rv = new InputState();

                for (int i = 0; i < 4; i++)
                {
                    rv.gamepadState[i] = gamepadState[i];
                }

                rv.mouseState = mouseState;
                rv.ScrollPos = ScrollPos;
                rv.keyboardState = keyboardState;

                return rv;
            }

            internal bool CheckPressed(ActionKeys key)
            {

                if (GamePadMappings.Any(x => x.Key == key))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (gamepadState[i].IsConnected)
                        {
                            KeyValuePair<ActionKeys, Buttons> found = GamePadMappings.FirstOrDefault(x => x.Key == key);

                            Buttons mapping = found.Value;
                            if (gamepadState[i].IsButtonDown(mapping))
                            {
                                return true;
                            }
                        }
                    }
                }

                if (KeyboardMappings.ContainsKey(key))
                {
                    var keyToCheck = KeyboardMappings[key];
                    if (keyboardState.IsKeyDown(keyToCheck))
                    {
                        return true;
                    }
                }

                if (MouseButtonsMappings.ContainsKey(key))
                {
                    MouseButtons mouseButtonToCheck = MouseButtonsMappings[key];

                    if (mouseButtonToCheck == MouseButtons.LeftClick &&
                        mouseState.LeftButton == ButtonState.Pressed) return true;


                    if (mouseButtonToCheck == MouseButtons.MiddleClick &&
                        mouseState.MiddleButton == ButtonState.Pressed) return true;


                    if (mouseButtonToCheck == MouseButtons.RightClick &&
                        mouseState.RightButton == ButtonState.Pressed) return true;

                    if (mouseButtonToCheck == MouseButtons.MouseWheelUp && ScrollPos > LastScrollPos) return true;
                    if (mouseButtonToCheck == MouseButtons.MouseWheelDown && ScrollPos < LastScrollPos) return true;
                }

                return false;
            }

            internal bool CheckShiftPressed(ShiftKeys key)
            {

                if (GamePadShiftMappings.Any(x => x.Key == key))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (gamepadState[i].IsConnected)
                        {
                            KeyValuePair<ShiftKeys, Buttons> found = GamePadShiftMappings.FirstOrDefault(x => x.Key == key);

                            Buttons mapping = found.Value;
                            if (gamepadState[i].IsButtonDown(mapping))
                            {
                                return true;
                            }
                        }
                    }
                }

                if (KeyboardShiftMappings.ContainsKey(key))
                {
                    var keyToCheck = KeyboardShiftMappings[key];
                    if (keyboardState.IsKeyDown(keyToCheck))
                    {
                        return true;
                    }
                }

                return false;
            }

            internal bool? ShouldMouseBeActive()
            {
                foreach (KeyValuePair<ActionKeys, Buttons> gamePadMapping in GamePadMappings)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (gamepadState[i].IsButtonDown(gamePadMapping.Value))
                        {
                            return false;
                        }
                    }
                }

                if (KeyboardMappings.Any(keyboardMapping => keyboardState.IsKeyDown(keyboardMapping.Value)))
                {
                    return false;
                }

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    return true;
                }

                return null;
            }
        }
    }
}
