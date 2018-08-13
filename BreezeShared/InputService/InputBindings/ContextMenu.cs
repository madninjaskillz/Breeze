using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Breeze.Services.InputService
{
    public static partial class InputBindings
    {
        public static class ContextMenu
        {
            public static InputService.InputBinding CloseWindow => new InputService.InputBinding
            (
                new InputService.InputStack(new InputService.KeyboardControl(Keys.Escape, InputService.PressType.Released)),
                new InputService.InputStack(new InputService.MouseControl(InputService.MouseButtons.RightClick, InputService.PressType.Released))

            );
        }
    }
}
