using Microsoft.Xna.Framework.Input;

namespace Breeze.Services.InputService
{
    public static partial class InputBindings
    {
        public static class UserInterface
        {
            public static InputService.InputBinding Up => new InputService.InputBinding
       (
           new InputService.InputStack(new InputService.KeyboardControl(Keys.Up, InputService.PressType.Press)),
           new InputService.InputStack(new InputService.KeyboardControl(Keys.Up, InputService.PressType.LongHoldingRepeat)),
           new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadUp, InputService.PressType.Press)),
           new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadUp, InputService.PressType.LongHoldingRepeat))
       );


            public static InputService.InputBinding Down => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Down, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Down, InputService.PressType.LongHoldingRepeat)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadDown, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadDown, InputService.PressType.LongHoldingRepeat))
            );

            public static InputService.InputBinding Right => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Right, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Right, InputService.PressType.LongHoldingRepeat)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadRight, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadRight, InputService.PressType.LongHoldingRepeat))
            );

            public static InputService.InputBinding Left => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Left, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Left, InputService.PressType.LongHoldingRepeat)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadLeft, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.DPadLeft, InputService.PressType.LongHoldingRepeat))
            );

            public static InputService.InputBinding ShowMenu => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Escape, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.Back, InputService.PressType.Released))
            );

            public static InputService.InputBinding ClickButton => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Tap)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Tap)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Tap))
            );


            public static InputService.InputBinding ButtonDown => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Press))
            );

            public static InputService.InputBinding ButtonHeld => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Held))
            );

            public static InputService.InputBinding ButtonUp=> new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Released))
            );
        }
    }
}
