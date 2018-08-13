using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Breeze.Services.InputService
{
    public partial class InputService
    {
        public class VirtualMouse
        {
            public TouchLocation? CurrentTouchLocation;
            internal Vector2 MousePosition;
            public bool IsTwoFinger;
            public bool mouseActive;
            public int ScrollPos;
            public bool PrimaryDown;
            public bool SecondaryDown;
            public bool MiddleDown;
            public int PrimaryDownCount = 0;
            public Vector2 PrimaryHoldStartPos;
            public void GetState()
            {
                SecondaryDown = false;
                PrimaryDown = false;
                MiddleDown = false;
                var currentState = InputState.GetState();

                TouchCollection touchCollection = TouchPanel.GetState();
                if (touchCollection.Count == 1)
                {
                    CurrentTouchLocation = touchCollection[0];
                    switch (touchCollection[0].State)
                    {
                        case TouchLocationState.Moved:
                        case TouchLocationState.Pressed:
                            {
                                
                                var xp = touchCollection[0].Position;
                                if (xp.X > 0 && xp.Y > 0)
                                {
                                    MousePosition = xp;

                                    if (PrimaryDownCount == 0)
                                    {
                                        PrimaryHoldStartPos = xp;
                                    }

                                    PrimaryDown = true;

                                    if (Vector2.Distance(xp, PrimaryHoldStartPos) > Solids.Bounds.Width / 16f && PrimaryDownCount<100)
                                    {
                                        PrimaryDownCount = 0;
                                    }
                                    else
                                    {
                                        PrimaryDownCount++;
                                        if (PrimaryDownCount > 100)
                                        {
                                            if (PrimaryDownCount > 102)
                                            {
                                                PrimaryDownCount = 0;
                                            }

                                            SecondaryDown = true;
                                        }
                                        else
                                        {
                                            SecondaryDown = false;
                                        }
                                    }
                                }
                               
                                break;
                            }
                    }
                }

                if (touchCollection.Count > 2)
                {
                    Vector2 p1 = touchCollection[0].Position;
                    Vector2 p2 = touchCollection[1].Position;

                    Vector2 p3 = Vector2.Lerp(p1, p2, 0.5f);
                    MousePosition = p3;
                    IsTwoFinger = true;
                }
                else
                {
                    IsTwoFinger = false;
                }

                if (touchCollection.Count == 0)
                {
                    PrimaryDown = false;
                    PrimaryDownCount = 0;
                    CurrentTouchLocation = null;
                    var xp = currentState.mouseState.Position.ToVector2();

                    if (xp.X > 0 && xp.Y > 0)
                    {
                        MousePosition = xp;
                    }
                }

                if (Math.Abs(MousePosition.X - LastGoodXmouse) > 10 || Math.Abs(MousePosition.Y - LastGoodYmouse) > 10)
                {
                    LastGoodXmouse = (int)MousePosition.X;
                    LastGoodYmouse = (int)MousePosition.Y;
                    mouseActive = true;
                }

                ScrollPos = currentState.ScrollPos;

                if (!PrimaryDown) PrimaryDown = (currentState.mouseState.LeftButton == ButtonState.Pressed);
                if (!SecondaryDown) SecondaryDown = (currentState.mouseState.RightButton == ButtonState.Pressed);
                if (!MiddleDown) MiddleDown = (currentState.mouseState.MiddleButton == ButtonState.Pressed);


            }

        }
        internal class BreezeInputState
        {
            public VirtualMouse VirtualMouse = new VirtualMouse();
            public TouchLocation? CurrentTouchLocation;
            internal Vector2 MousePosition;
            public bool IsTwoFinger;
            
            private Dictionary<ActionKeys, bool> states = new Dictionary<ActionKeys, bool>();
            private Dictionary<ShiftKeys, bool> shiftstates = new Dictionary<ShiftKeys, bool>();
            

            private InputState currentState;

            public List<Keys> PressedKeys = new List<Keys>();
            public int ScrollPos = 0;

            private bool mouseActive = false;
            internal void UpdateState()
            {
                VirtualMouse.GetState();
                states = new Dictionary<ActionKeys, bool>();
                shiftstates = new Dictionary<ShiftKeys, bool>();
                currentState = InputState.GetState();

                foreach (ActionKeys actionKey in Helpers.EnumExtensions.GetValues<ActionKeys>())
                {
                    states.Add(actionKey, currentState.CheckPressed(actionKey));
                }


                foreach (ShiftKeys actionKey in SingularShiftKeys)
                {
                    shiftstates.Add(actionKey, currentState.CheckShiftPressed(actionKey));
                }

                
                TouchCollection touchCollection = TouchPanel.GetState();
                if (touchCollection.Count == 1)
                {
                    CurrentTouchLocation = touchCollection[0];
                    switch (touchCollection[0].State)
                    {
                        case TouchLocationState.Moved:
                        {
                            var xp = touchCollection[0].Position;
                            if (xp.X > 0 && xp.Y > 0)
                            {
                                MousePosition = xp;
                            }
                            break;
                        }

                        case TouchLocationState.Pressed:
                        {
                            var xp = touchCollection[0].Position;
                            if (xp.X > 0 && xp.Y > 0)
                            {
                                MousePosition = xp;
                            }

                            break;
                        }
                    }
                }

                if (touchCollection.Count > 2)
                {
                    Vector2 p1 = touchCollection[0].Position;
                    Vector2 p2 = touchCollection[1].Position;

                    Vector2 p3 = Vector2.Lerp(p1, p2, 0.5f);
                    MousePosition = p3;
                    IsTwoFinger = true;
                }
                else
                {
                    IsTwoFinger = false;
                }

                if (touchCollection.Count == 0)
                {
                    CurrentTouchLocation = null;
                    var xp = currentState.mouseState.Position.ToVector2();

                    if (xp.X > 0 && xp.Y > 0)
                    {
                        MousePosition = xp;
                    }
                }

                if (Math.Abs(MousePosition.X - LastGoodXmouse) > 10 || Math.Abs(MousePosition.Y - LastGoodYmouse) > 10)
                {
                    LastGoodXmouse = (int)MousePosition.X;
                    LastGoodYmouse = (int)MousePosition.Y;
                    mouseActive = true;
                }

                ScrollPos = currentState.ScrollPos;
                PressedKeys = currentState.PressedKeys;
            }

            internal bool CheckPressed(ActionKeys key) => states.ContainsKey(key) && states[key];

            internal bool CheckShiftPressed(ShiftKeys key) => shiftstates.ContainsKey(key) && shiftstates[key];

            internal bool ShiftPressed(ShiftKeys shift)
            {
                if (shift == ShiftKeys.DontCare)
                {
                    return true;
                }

                if (shift == ShiftKeys.NoShift) return
                    !CheckShiftPressed(ShiftKeys.LeftShift1) && !CheckShiftPressed(ShiftKeys.RightShift1) &&
                    !CheckShiftPressed(ShiftKeys.LeftShift2) && !CheckShiftPressed(ShiftKeys.RightShift2) &&
                    !CheckShiftPressed(ShiftKeys.LeftShift3) && !CheckShiftPressed(ShiftKeys.RightShift3);

                if (shift == ShiftKeys.LeftShift1) return CheckShiftPressed(ShiftKeys.LeftShift1);
                if (shift == ShiftKeys.LeftShift2) return CheckShiftPressed(ShiftKeys.LeftShift2);
                if (shift == ShiftKeys.LeftShift3) return CheckShiftPressed(ShiftKeys.LeftShift3);


                if (shift == ShiftKeys.RightShift1) return CheckShiftPressed(ShiftKeys.RightShift1);
                if (shift == ShiftKeys.RightShift2) return CheckShiftPressed(ShiftKeys.RightShift2);
                if (shift == ShiftKeys.RightShift3) return CheckShiftPressed(ShiftKeys.RightShift3);

                if (shift == ShiftKeys.Shift1) return CheckShiftPressed(ShiftKeys.LeftShift1) || CheckShiftPressed(ShiftKeys.RightShift1);
                if (shift == ShiftKeys.Shift2) return CheckShiftPressed(ShiftKeys.LeftShift2) || CheckShiftPressed(ShiftKeys.RightShift2);
                if (shift == ShiftKeys.Shift3) return CheckShiftPressed(ShiftKeys.LeftShift3) || CheckShiftPressed(ShiftKeys.RightShift3);

                if (shift == ShiftKeys.AnyShift) return
                    CheckShiftPressed(ShiftKeys.LeftShift1) || CheckShiftPressed(ShiftKeys.RightShift1) ||
                        CheckShiftPressed(ShiftKeys.LeftShift2) || CheckShiftPressed(ShiftKeys.RightShift2) ||
                        CheckShiftPressed(ShiftKeys.LeftShift3) || CheckShiftPressed(ShiftKeys.RightShift3);

                return false;
            }

            public bool? ShouldMouseBeActive()
            {
                var res = currentState.ShouldMouseBeActive();

                if (res.HasValue && !res.Value)
                {
                    mouseActive = false;
                }

                if (mouseActive)
                {
                    return true;
                }

                return res;

            }

            public BreezeInputState CloneMe()
            {
                var result = new BreezeInputState();
                foreach (var t in states)
                {
                    result.states.Add(t.Key, t.Value);
                }

                result.MousePosition = new Vector2(MousePosition.X, MousePosition.Y);

                result.PressedKeys = PressedKeys.ToList();
                return result;
            }
        }
    }
}
