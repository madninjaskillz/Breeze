using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Breeze.Services.InputService
{
    public partial class InputService
    {
        public enum ActionKeys
        {
            UIUp,
            UIDown,
            UIRight,
            UILeft,
            MainAction,
            PlayPause,
            ZoomOut,
            ZoomIn,
            PanRight,
            PanLeft,
            ToggleDebug,
            ToggleViz,
            Menu,
            Delete,
            //LeftShift1,
            //LeftShift2,
            //LeftShift3,
            //RightShift1,
            //RightShift2,
            //RightShift3,
            SelectArea,
            CloseModal,
            Split,
            ToggleFullScreen
        }

        public class InputBinding
        {
            public List<InputStack> Stacks { get; set; }

            public bool IsPressed(InputStack input)
            {
                foreach (InputStack inputStack in Stacks)
                {
                    //if (inputStack.Inputs.Count == input.Inputs.Count)
                    {
                        bool anyNegative = false;
                        foreach (var inputItem in inputStack.Inputs)
                        {
                            if (!input.Inputs.Any(x =>
                                x.PressType == inputItem.PressType &&
                                x.GetType() == inputItem.GetType() &&
                                (
                                    (x.GetType() == typeof(GamepadControl) && ((GamepadControl) x).Button == ((GamepadControl) inputItem).Button) ||
                                    (x.GetType() == typeof(MouseControl) && ((MouseControl) x).MouseButton == ((MouseControl) inputItem).MouseButton) ||
                                    (x.GetType() == typeof(KeyboardControl) && ((KeyboardControl) x).Key == ((KeyboardControl) inputItem).Key)
                                )))
                            {
                                anyNegative = true;
                                break;
                            }
                        }

                        if (!anyNegative)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            
            public InputBinding(params InputStack[] stacks)
            {
                this.Stacks = stacks.ToList();
            }
            
        }

        public class InputStack
        {
            public List<InputControl> Inputs { get; set; }

            public InputStack(params InputControl[] inputs)
            {
                Inputs = inputs.ToList();
            }
        }

        public class InputControl
        {
            public PressType PressType { get; set; }
        }

        public class GamepadControl : InputControl
        {
            public Buttons Button { get; set; }

            public GamepadControl(Buttons button, PressType pressType)
            {
                base.PressType = pressType;
                Button = button;
            }
        }

        public class KeyboardControl : InputControl
        {
            public Keys Key { get; set; }

            public KeyboardControl(Keys key, PressType pressType)
            {
                base.PressType = pressType;
                this.Key = key;
            }
        }

        public class MouseControl : InputControl
        {
            public MouseButtons MouseButton { get; set; }
            public bool RightMouseButton { get; set; }

            public MouseControl(MouseButtons button, PressType pressType)
            {
                base.PressType = pressType;
                MouseButton = button;
            }
        }

        public enum PressType
        {
            Press,
            Released,
            Holding,
            LongHolding,
            LongHoldingRepeat,
            HoldingTriggered,
            Tap,
            LongTapped,
            NotPressed,
            Held

        }


    }
}
