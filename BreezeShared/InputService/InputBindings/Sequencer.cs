using Microsoft.Xna.Framework.Input;

namespace Breeze.Services.InputService
{
    public static partial class InputBindings
    {
        public static class Console
        {
            public static InputService.InputBinding Execute => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Tap)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.Start, InputService.PressType.Tap))
            );
        }
        public static class Sequencer
        {
            public static InputService.InputBinding TimeStretch => new InputService.InputBinding
           (
               new InputService.InputStack
               (
                   new InputService.KeyboardControl(Keys.LeftShift, InputService.PressType.Holding),
                   new InputService.KeyboardControl(Keys.LeftControl, InputService.PressType.Holding),
                   new InputService.KeyboardControl(Keys.LeftAlt, InputService.PressType.NotPressed),
                   new InputService.KeyboardControl(Keys.RightAlt, InputService.PressType.NotPressed),
                   new InputService.KeyboardControl(Keys.RightShift, InputService.PressType.NotPressed),
                   new InputService.KeyboardControl(Keys.RightControl, InputService.PressType.NotPressed)
               ),
               new InputService.InputStack
               (
                   new InputService.GamepadControl(Buttons.LeftTrigger, InputService.PressType.Holding),
                   new InputService.GamepadControl(Buttons.LeftShoulder, InputService.PressType.Holding),
                   new InputService.GamepadControl(Buttons.RightTrigger, InputService.PressType.NotPressed),
                   new InputService.GamepadControl(Buttons.RightShoulder, InputService.PressType.NotPressed)
               )
           );

            public static InputService.InputBinding Slip => new InputService.InputBinding
            (
                new InputService.InputStack
                (
            new InputService.KeyboardControl(Keys.LeftShift, InputService.PressType.NotPressed),
            new InputService.KeyboardControl(Keys.LeftControl, InputService.PressType.NotPressed),
            new InputService.KeyboardControl(Keys.LeftAlt, InputService.PressType.NotPressed),
            new InputService.KeyboardControl(Keys.RightAlt, InputService.PressType.NotPressed),
            new InputService.KeyboardControl(Keys.RightShift, InputService.PressType.NotPressed),
            new InputService.KeyboardControl(Keys.RightControl, InputService.PressType.NotPressed)
                ),
                new InputService.InputStack
                (
                    new InputService.GamepadControl(Buttons.LeftTrigger, InputService.PressType.NotPressed),
                    new InputService.GamepadControl(Buttons.LeftShoulder, InputService.PressType.NotPressed),
                    new InputService.GamepadControl(Buttons.RightTrigger, InputService.PressType.NotPressed),
                    new InputService.GamepadControl(Buttons.RightShoulder, InputService.PressType.NotPressed)
                )
            );

            public static InputService.InputBinding DisableLock => new InputService.InputBinding
            (
                new InputService.InputStack
                (
                    new InputService.KeyboardControl(Keys.LeftShift, InputService.PressType.Holding),
                    new InputService.KeyboardControl(Keys.LeftControl, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.LeftAlt, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightAlt, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightShift, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightControl, InputService.PressType.NotPressed)
                ),
                new InputService.InputStack
                (
                    new InputService.GamepadControl(Buttons.LeftTrigger, InputService.PressType.NotPressed),
                    new InputService.GamepadControl(Buttons.LeftShoulder, InputService.PressType.Holding),
                    new InputService.GamepadControl(Buttons.RightTrigger, InputService.PressType.NotPressed),
                    new InputService.GamepadControl(Buttons.RightShoulder, InputService.PressType.NotPressed)
                )
            );

            public static InputService.InputBinding PlayPause => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Space, InputService.PressType.Tap)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.Start, InputService.PressType.Tap))
             );

            public static InputService.InputBinding Drag => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Held)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Held)),
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Held))

            );

            public static InputService.InputBinding DragStart => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Press)),
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Press))
            );

            public static InputService.InputBinding DragEnd => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Enter, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.LeftClick, InputService.PressType.Released))
            );

            public static InputService.InputBinding ContextMenu => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.RightClick, InputService.PressType.Tap)),
                new InputService.InputStack(new InputService.KeyboardControl(Keys.C, InputService.PressType.Tap),
                    new InputService.KeyboardControl(Keys.LeftShift, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.LeftControl, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.LeftAlt, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightAlt, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightShift, InputService.PressType.NotPressed),
                    new InputService.KeyboardControl(Keys.RightControl, InputService.PressType.NotPressed)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.A, InputService.PressType.HoldingTriggered))
            );

            public static InputService.InputBinding PanLeft => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.A, InputService.PressType.Holding)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.RightThumbstickLeft, InputService.PressType.Holding))
            );

            public static InputService.InputBinding PanRight => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.D, InputService.PressType.Holding)),
                new InputService.InputStack(new InputService.GamepadControl(Buttons.RightThumbstickRight, InputService.PressType.Holding))
            );

       
        }
    }
}
