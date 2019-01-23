using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#if WINDOWS_UAP
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Xaml;
#endif
using Breeze.Helpers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Breeze.Services.InputService
{
    public partial class InputService
    {
        private const int TAPLENGTH = 30;
        private const int SHORTHOLD = 50;
        private const int LONGHOLD = 70;
        private const int LONGHOLDREPEAT = 10;
        public static int LastGoodXmouse = 0;
        public static int LastGoodYmouse = 0;
        public static int LastScrollPos;

        public bool MouseActive;
        public InputStack CurrentStack = new InputStack();

        private BreezeInputState oldState = new BreezeInputState();
        private BreezeInputState newState = new BreezeInputState();
        public Vector2 MousePosition { get; set; } = Vector2.Zero;
        public Vector2 PreviousMousePosition { get; set; } = Vector2.Zero;
        public Vector2 MouseScreenPosition { get; set; } = Vector2.Zero;
        public List<char> PressedChars { get; set; }

        readonly Buttons[] PossibleGamePadButtons = (Buttons[])Enum.GetValues(typeof(Buttons));
        readonly MouseButtons[] PossibleMouseButtons = (MouseButtons[])Enum.GetValues(typeof(MouseButtons));
        readonly Keys[] PossibleKeyboardKeys = (Keys[])Enum.GetValues(typeof(Keys));


        public async Task Init()
        {
#if WINDOWS_UAP
            await EnumerateMidiInputDevices();
#endif
        }
#if WINDOWS_UAP
        private async Task EnumerateMidiInputDevices()
        {
            // Find all input MIDI devices
            string midiInputQueryString = MidiInPort.GetDeviceSelector();
            DeviceInformationCollection midiInputDevices = await DeviceInformation.FindAllAsync(midiInputQueryString);

            DeviceInformation firstMidiDevice = midiInputDevices.FirstOrDefault();

            if (firstMidiDevice == null)
            {
                return;
            }

            var midiInPort = await MidiInPort.FromIdAsync(firstMidiDevice.Id);

            if (midiInPort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }

//todo move me            midiInPort.MessageReceived += MidiInPort_MessageReceived;

        }


        //TODO Move ME
        //private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        //{
        //    IMidiMessage receivedMidiMessage = args.Message;

        //    //System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

        //    if (receivedMidiMessage.Type == MidiMessageType.NoteOn || receivedMidiMessage.Type == MidiMessageType.NoteOff)
        //    {
        //        int velocity = -1;
        //        byte note = receivedMidiMessage.Type == MidiMessageType.NoteOn ? ((MidiNoteOnMessage)receivedMidiMessage).Note : ((MidiNoteOffMessage)receivedMidiMessage).Note;
        //        if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
        //        {
        //            velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
        //        }
        //        MidiNote midiNote = new MidiNote(note, velocity);

        //        if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
        //        {
        //            Solids.Mixer.NoteOn(midiNote);
        //        }

        //        if (receivedMidiMessage.Type == MidiMessageType.NoteOff)
        //        {
        //            Solids.Mixer.NoteOff(midiNote);
        //        }

        //        //  System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Velocity);
        //    }
        //}
#endif

        public void Update(GameTime gameTime)
        {
            
            if (newState != null)
            {
                LastScrollPos = newState.ScrollPos;
            
                oldState = newState.CloneMe();
            }
            else
            {
                newState = new BreezeInputState();
            }

           // newState = new BreezeInputState();
            newState.UpdateState();
            MousePosition = newState.MousePosition;
            if (MousePosition != PreviousMousePosition)
            {
                MouseActive = true;
            }

            PreviousMousePosition = MousePosition;

            MouseScreenPosition = new Vector2(newState.MousePosition.X / Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds.Width, newState.MousePosition.Y / Solids.Instance.SpriteBatch.GraphicsDevice.Viewport.Bounds.Height);

            bool? mba = newState.ShouldMouseBeActive();
            if (mba != null)
            {
                MouseActive = mba.Value;
            }

            //---------------------------------

            CurrentStack = new InputStack();

            var currentKeyState = Keyboard.GetState();
            GamePadState[] currentGamePadState = new GamePadState[GamePad.MaximumGamePadCount];

            List<Buttons> currentButtons = new List<Buttons>();
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                currentGamePadState[i] = GamePad.GetState(i);

                foreach (Buttons b in Enum.GetValues(typeof(Buttons)))
                {
                    if (currentGamePadState[i].IsButtonDown(b))
                    {
                        currentButtons.Add(b);
                        MouseActive = false;
                    }
                }

            }

            var justPressedButtons = currentButtons.Where(x => !previousButtons.Contains(x));
            var justReleasedButtons = previousButtons.Where(x => !currentButtons.Contains(x));
            var heldButtons = previousButtons.Where(x => currentButtons.Contains(x));
            var notPressedButtons = PossibleGamePadButtons.Where(x => !currentButtons.Contains(x));

            foreach (Buttons justPressedKey in justPressedButtons)
            {
                holdCounter.Add("GPB_" + justPressedKey.ToString(), 0);
                CurrentStack.Inputs.Add(new GamepadControl(justPressedKey, PressType.Press));
                CurrentStack.Inputs.Add(new GamepadControl(justPressedKey, PressType.PressThenHolding));
            }

            foreach (Buttons heldKey in heldButtons)
            {
                holdCounter["GPB_" + heldKey.ToString()]++;

                if (holdCounter["GPB_" + heldKey.ToString()] == SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new GamepadControl(heldKey, PressType.HoldingTriggered));
                }

                if (holdCounter["GPB_" + heldKey.ToString()] > LONGHOLD + LONGHOLDREPEAT)
                {
                    CurrentStack.Inputs.Add(new GamepadControl(heldKey, PressType.LongHoldingRepeat));
                    holdCounter["GPB_" + heldKey.ToString()] = LONGHOLD + 1;
                }


                if (holdCounter["GPB_" + heldKey.ToString()] > LONGHOLD)
                {
                    CurrentStack.Inputs.Add(new GamepadControl(heldKey, PressType.LongHolding));
                }

                if (holdCounter["GPB_" + heldKey.ToString()] > SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new GamepadControl(heldKey, PressType.Holding));
                    CurrentStack.Inputs.Add(new GamepadControl(heldKey, PressType.PressThenHolding));
                }

            }

            foreach (Buttons justReleasedKey in justReleasedButtons)
            {
                if (holdCounter["GPB_" + justReleasedKey.ToString()] < SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new GamepadControl(justReleasedKey, PressType.Tap));
                }
                else
                {
                    CurrentStack.Inputs.Add(new GamepadControl(justReleasedKey, PressType.LongTapped));
                }

                CurrentStack.Inputs.Add(new GamepadControl(justReleasedKey, PressType.Released));

                holdCounter.Remove("GPB_" + justReleasedKey.ToString());

            }

            foreach (Buttons notPressed in notPressedButtons)
            {
                CurrentStack.Inputs.Add(new GamepadControl(notPressed, PressType.NotPressed));
            }

            previousButtons = currentButtons.ToArray().ToList();

            List<MouseButtons> currentMouseButtons = new List<MouseButtons>();
            
                if (newState.VirtualMouse.PrimaryDown) currentMouseButtons.Add(MouseButtons.LeftClick);
                if (newState.VirtualMouse.SecondaryDown) currentMouseButtons.Add(MouseButtons.RightClick);
                if (newState.VirtualMouse.MiddleDown) currentMouseButtons.Add(MouseButtons.MiddleClick);


#if WINDOWS_UAP
                //if (state.HorizontalScrollWheelValue < previousMouseState.HorizontalScrollWheelValue)
                //{
                //    currentMouseButtons.Add(MouseButtons.MouseWheelUp);
                //}

                //if (state.HorizontalScrollWheelValue > previousMouseState.HorizontalScrollWheelValue)
                //{
                //    currentMouseButtons.Add(MouseButtons.MouseWheelDown);
                //}
#endif
          
            var justPressedMouseButtons = currentMouseButtons.Where(x => previousMouseButtons.Contains(x) == false);
            var justReleasedMouseButtons = previousMouseButtons.Where(x => currentMouseButtons.Contains(x) == false);
            var heldMouseButtons = currentMouseButtons.Where(x => previousMouseButtons.Contains(x));
            var notPressedMouseButtons = PossibleMouseButtons.Where(x => !currentMouseButtons.Contains(x));

            foreach (MouseButtons notPressedMouseButton in notPressedMouseButtons)
            {
                CurrentStack.Inputs.Add(new MouseControl(notPressedMouseButton, PressType.NotPressed));
            }

            foreach (MouseButtons justPressedKey in justPressedMouseButtons)
            {
                holdCounter.Add("MB_" + justPressedKey.ToString(), 0);
                CurrentStack.Inputs.Add(new MouseControl(justPressedKey, PressType.Press));
                CurrentStack.Inputs.Add(new MouseControl(justPressedKey, PressType.PressThenHolding));
                MouseActive = true;
            }

            foreach (MouseButtons heldKey in heldMouseButtons)
            {
                holdCounter["MB_" + heldKey.ToString()]++;

                CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.Held));

                if (holdCounter["MB_" + heldKey.ToString()] == SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.HoldingTriggered));
                }

                if (holdCounter["MB_" + heldKey.ToString()] > LONGHOLD + LONGHOLDREPEAT)
                {
                    CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.LongHoldingRepeat));
                    holdCounter["MB_" + heldKey.ToString()] = LONGHOLD + 1;
                }


                if (holdCounter["MB_" + heldKey.ToString()] > LONGHOLD)
                {
                    CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.LongHolding));
                }
                // else
                {
                    if (holdCounter["MB_" + heldKey.ToString()] > SHORTHOLD)
                    {
                        CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.Holding));
                        CurrentStack.Inputs.Add(new MouseControl(heldKey, PressType.PressThenHolding));
                    }
                }

            }

            foreach (MouseButtons justReleasedKey in justReleasedMouseButtons)
            {
                Console.WriteLine(holdCounter["MB_" + justReleasedKey.ToString()]);
                if (holdCounter["MB_" + justReleasedKey.ToString()] < SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new MouseControl(justReleasedKey, PressType.Tap));
                }
                else
                {
                    CurrentStack.Inputs.Add(new MouseControl(justReleasedKey, PressType.LongTapped));
                }

                CurrentStack.Inputs.Add(new MouseControl(justReleasedKey, PressType.Released));

                holdCounter.Remove("MB_" + justReleasedKey.ToString());

            }

            previousMouseButtons = currentMouseButtons.ToArray().ToList();


            var currentKeys = currentKeyState.GetPressedKeys();
            var previousKeys = previousKeyState.GetPressedKeys();
            var justPressedKeys = currentKeys.Where(x => previousKeys.Contains(x) == false);
            var justReleasedKeys = previousKeys.Where(x => currentKeys.Contains(x) == false);
            var heldKeys = currentKeys.Where(x => previousKeys.Contains(x));
            var notPressedKeys = PossibleKeyboardKeys.Where(x => !currentKeys.Contains(x));

            foreach (var notPressedKey in notPressedKeys)
            {
                CurrentStack.Inputs.Add(new KeyboardControl(notPressedKey, PressType.NotPressed));
            }

            foreach (Keys justPressedKey in justPressedKeys)
            {
                holdCounter.Add("Key_" + justPressedKey.ToString(), 0);
                CurrentStack.Inputs.Add(new KeyboardControl(justPressedKey, PressType.Press));
            }

            foreach (Keys heldKey in heldKeys)
            {
                holdCounter["Key_" + heldKey.ToString()]++;

                if (holdCounter["Key_" + heldKey.ToString()] == SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new KeyboardControl(heldKey, PressType.HoldingTriggered));
                }

                if (holdCounter["Key_" + heldKey.ToString()] > LONGHOLD + LONGHOLDREPEAT)
                {
                    CurrentStack.Inputs.Add(new KeyboardControl(heldKey, PressType.LongHoldingRepeat));
                    holdCounter["Key_" + heldKey.ToString()] = LONGHOLD + 1;
                }


                if (holdCounter["Key_" + heldKey.ToString()] > LONGHOLD)
                {
                    CurrentStack.Inputs.Add(new KeyboardControl(heldKey, PressType.LongHolding));
                }
                //   else
                {
                    if (holdCounter["Key_" + heldKey.ToString()] > SHORTHOLD)
                    {
                        CurrentStack.Inputs.Add(new KeyboardControl(heldKey, PressType.Holding));
                    }
                }
            }

            foreach (Keys justReleasedKey in justReleasedKeys)
            {
                if (holdCounter["Key_" + justReleasedKey.ToString()] < SHORTHOLD)
                {
                    CurrentStack.Inputs.Add(new KeyboardControl(justReleasedKey, PressType.Tap));
                }
                else
                {
                    CurrentStack.Inputs.Add(new KeyboardControl(justReleasedKey, PressType.LongTapped));
                }

                CurrentStack.Inputs.Add(new KeyboardControl(justReleasedKey, PressType.Released));

                holdCounter.Remove("Key_" + justReleasedKey.ToString());

            }

            previousKeyState = currentKeyState;

        }
        private List<MouseButtons> previousMouseButtons = new List<MouseButtons>();
        private KeyboardState previousKeyState = new KeyboardState();
        private List<Buttons> previousButtons = new List<Buttons>();
        private MouseState previousMouseState = new MouseState();
        public Dictionary<string, int> holdCounter = new Dictionary<string, int>();

        //todo move me?
       // public SequencerScreen.ActivePatternActionMode MousePointer = SequencerScreen.ActivePatternActionMode.None;
        public void Draw(GameTime gameTime)
        {
            float mouseSize = 96f;
            var mp = newState.MousePosition;

            string mousePointer = "pointer";

            //todo move me
            //switch (MousePointer)
            //{
            //    case SequencerScreen.ActivePatternActionMode.Move: mousePointer = "move"; break;
            //    case SequencerScreen.ActivePatternActionMode.SlipLeft: mousePointer = "slipLeft"; break;
            //    case SequencerScreen.ActivePatternActionMode.SlipRight: mousePointer = "slipRight"; break;
            //    case SequencerScreen.ActivePatternActionMode.None: mousePointer = "pointer"; break;
            //    case SequencerScreen.ActivePatternActionMode.TimestretchLeft: mousePointer = "timestretchLeft"; break;
            //    case SequencerScreen.ActivePatternActionMode.TimestretchRight: mousePointer = "timestretchRight"; break;
            //}

            string icon = "MousePointers\\" + mousePointer + ".png";

            Solids.Instance.SpriteBatch.Draw(Solids.Instance.AssetLibrary.GetTexture(icon), new Rectangle((int)(mp.X - (mouseSize / 2f)), (int)(mp.Y - (mouseSize / 2f)), (int)mouseSize, (int)mouseSize), Color.White);
        }

        public static List<ShiftKeys> SingularShiftKeys = new List<ShiftKeys>
        {
            ShiftKeys.LeftShift1,
            ShiftKeys.LeftShift2,
            ShiftKeys.LeftShift3,
            ShiftKeys.RightShift1,
            ShiftKeys.RightShift2,
            ShiftKeys.RightShift3
        };

        public static Dictionary<ActionKeys, Buttons> GamePadMappings = new Dictionary<ActionKeys, Buttons>
        {
            {ActionKeys.UIUp, Buttons.DPadUp},
            {ActionKeys.UIRight, Buttons.DPadRight},
            {ActionKeys.UIDown, Buttons.DPadDown},
            {ActionKeys.UILeft, Buttons.DPadLeft},
            {ActionKeys.MainAction, Buttons.A },
            {ActionKeys.PlayPause, Buttons.Start },
            {ActionKeys.ZoomIn, Buttons.RightThumbstickUp },
            {ActionKeys.ZoomOut, Buttons.RightThumbstickDown },
            {ActionKeys.Menu, Buttons.Y },
            {ActionKeys.PanRight, Buttons.RightTrigger },
            {ActionKeys.PanLeft, Buttons.LeftTrigger },
            {ActionKeys.Delete, Buttons.B },
            {ActionKeys.SelectArea, Buttons.X },
            {ActionKeys.CloseModal, Buttons.B },
        };

        public static Dictionary<ShiftKeys, Buttons> GamePadShiftMappings = new Dictionary<ShiftKeys, Buttons>
        {
            {ShiftKeys.LeftShift1, Buttons.LeftTrigger},
                {ShiftKeys.LeftShift2, Buttons.LeftShoulder         },
                {ShiftKeys.LeftShift3, Buttons.LeftStick     },
                {ShiftKeys.RightShift1,  Buttons.RightTrigger   },
                {ShiftKeys.RightShift2,  Buttons.RightShoulder   },
                {ShiftKeys.RightShift3,  Buttons.RightStick   },
        };

        public static Dictionary<ShiftKeys, Keys> KeyboardShiftMappings = new Dictionary<ShiftKeys, Keys>
        {
            {ShiftKeys.LeftShift1, Keys.LeftShift},
            {ShiftKeys.LeftShift2, Keys.LeftControl},
            {ShiftKeys.LeftShift3, Keys.LeftAlt},
            {ShiftKeys.RightShift1, Keys.RightShift},
            {ShiftKeys.RightShift2, Keys.RightControl},
            {ShiftKeys.RightShift3, Keys.RightAlt},
        };

        public static Dictionary<ActionKeys, Keys> KeyboardMappings = new Dictionary<ActionKeys, Keys>
        {
            {ActionKeys.UIUp, Keys.Up},
            {ActionKeys.UIRight, Keys.Right},
            {ActionKeys.UIDown, Keys.Down},
            {ActionKeys.UILeft, Keys.Left},
            {ActionKeys.MainAction, Keys.Enter },
            {ActionKeys.PlayPause, Keys.Space },
            {ActionKeys.ZoomIn, Keys.PageUp },
            {ActionKeys.ZoomOut, Keys.PageDown },
            {ActionKeys.PanRight, Keys.D},
            {ActionKeys.PanLeft, Keys.A },
            {ActionKeys.ToggleDebug, Keys.F12 },
            {ActionKeys.ToggleViz, Keys.F1 },
            {ActionKeys.Menu, Keys.I },
            {ActionKeys.Delete, Keys.Delete },

            {ActionKeys.SelectArea, Keys.LeftShift },
            {ActionKeys.CloseModal, Keys.Escape },
            {ActionKeys.Split, Keys.S },
            
        };

        public static Dictionary<ActionKeys, MouseButtons> MouseButtonsMappings = new Dictionary<ActionKeys, MouseButtons>
        {
            {ActionKeys.MainAction, MouseButtons.LeftClick },
            {ActionKeys.Menu, MouseButtons.RightClick },
            {ActionKeys.ZoomIn, MouseButtons.MouseWheelUp },
            {ActionKeys.ZoomOut, MouseButtons.MouseWheelDown },
        };

        public bool JustPressed(ActionKeys key)
        {
            return (!oldState.CheckPressed(key) && newState.CheckPressed(key));
        }

        public bool JustReleased(ActionKeys key)
        {
            return (oldState.CheckPressed(key) && !newState.CheckPressed(key));
        }

        public bool IsPressed(ActionKeys key)
        {
            return newState.CheckPressed(key);
        }

        public bool IsPressed(ShiftKeys shift)
        {
            return newState.ShiftPressed(shift);
        }

        public bool JustPressed(ActionKeys key, ShiftKeys shift)
        {
            return newState.ShiftPressed(shift) && ((!oldState.CheckPressed(key) && newState.CheckPressed(key)));
        }

        public bool JustReleased(ActionKeys key, ShiftKeys shift)
        {
            return newState.ShiftPressed(shift) && ((oldState.CheckPressed(key) && !newState.CheckPressed(key)));
        }

        public bool IsPressed(ActionKeys key, ShiftKeys shift)
        {
            return newState.ShiftPressed(shift) && (newState.CheckPressed(key));
        }

        public List<Keys> JustPressedKeys()
        {
            return newState.PressedKeys.Where(t => !oldState.PressedKeys.Contains(t)).ToList();
        }

        public static Dictionary<Keys, Symbol> SymbolDefinitions = new Dictionary<Keys, Symbol>
        {
            // Digits.
            { Keys.D1, new Symbol("1", "!") },
            { Keys.D2, new Symbol("2", "@") },
            { Keys.D3, new Symbol("3", "#") },
            { Keys.D4, new Symbol("4", "$") },
            { Keys.D5, new Symbol("5", "%") },
            { Keys.D6, new Symbol("6", "^") },
            { Keys.D7, new Symbol("7", "&") },
            { Keys.D8, new Symbol("8", "*") },
            { Keys.D9, new Symbol("9", "(") },
            { Keys.D0, new Symbol("0", ")") },
            { Keys.NumPad1, new Symbol("1") },
            { Keys.NumPad2, new Symbol("2") },
            { Keys.NumPad3, new Symbol("3") },
            { Keys.NumPad4, new Symbol("4") },
            { Keys.NumPad5, new Symbol("5") },
            { Keys.NumPad6, new Symbol("6") },
            { Keys.NumPad7, new Symbol("7") },
            { Keys.NumPad8, new Symbol("8") },
            { Keys.NumPad9, new Symbol("9") },
            { Keys.NumPad0, new Symbol("0") },

            // Letters.
            { Keys.Q, new Symbol("q", "Q") },
            { Keys.W, new Symbol("w", "W") },
            { Keys.E, new Symbol("e", "E") },
            { Keys.R, new Symbol("r", "R") },
            { Keys.T, new Symbol("t", "T") },
            { Keys.Y, new Symbol("y", "Y") },
            { Keys.U, new Symbol("u", "U") },
            { Keys.I, new Symbol("i", "I") },
            { Keys.O, new Symbol("o", "O") },
            { Keys.P, new Symbol("p", "P") },
            { Keys.OemOpenBrackets, new Symbol("[", "{") },
            { Keys.OemCloseBrackets, new Symbol("]", "}") },

            { Keys.A, new Symbol("a", "A") },
            { Keys.S, new Symbol("s", "S") },
            { Keys.D, new Symbol("d", "D") },
            { Keys.F, new Symbol("f", "F") },
            { Keys.G, new Symbol("g", "G") },
            { Keys.H, new Symbol("h", "H") },
            { Keys.J, new Symbol("j", "J") },
            { Keys.K, new Symbol("k", "K") },
            { Keys.L, new Symbol("l", "L") },
            { Keys.OemSemicolon, new Symbol(";", ":") },
            { Keys.OemQuotes, new Symbol("'", "\"") },
            { Keys.OemPipe, new Symbol("\\", "|") },
            { Keys.OemBackslash, new Symbol("\\", "|") },
            { Keys.Z, new Symbol("z", "Z") },
            { Keys.X, new Symbol("x", "X") },
            { Keys.C, new Symbol("c", "C") },
            { Keys.V, new Symbol("v", "V") },
            { Keys.B, new Symbol("b", "B") },
            { Keys.N, new Symbol("n", "N") },
            { Keys.M, new Symbol("m", "M") },
            { Keys.OemComma, new Symbol(",", "<") },
            { Keys.OemPeriod, new Symbol(".", ">") },
            { Keys.OemQuestion, new Symbol("/", "?") },
            // Special.
            { Keys.Space, new Symbol(" ", " ") },
            { Keys.OemMinus, new Symbol("-", "_") },
            { Keys.OemPlus, new Symbol("=", "+") },
            { Keys.Decimal, new Symbol(".") },
            { Keys.Add, new Symbol("+") },
            { Keys.Subtract, new Symbol("-") },
            { Keys.Multiply, new Symbol("*") },
            { Keys.Divide, new Symbol("/") }
            //{ Keys.Tab, new SymbolPair("\t", "\t") } // Tab char is not supported in many fonts.
            //{ Keys.Tab, new SymbolPair("    ", "    ") } // Use 4 spaces instead.
        };
    }
}
