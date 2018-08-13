using Breeze.AssetTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Breeze.Screens
{
    public class XuiDialog
    {
        public static async Task<int> ShowDialog(SmartSpriteBatch spriteBatch, string title, string body, 
            List<string> buttons)
        {
            TaskCompletionSource<int> completionSource = new TaskCompletionSource<int>();
            XuiDialog dialog = new XuiDialog();
            DialogScreen screen = new DialogScreen
            {
                Buttons = new List<StaticButtonAsset>()
            };

            List<StaticButtonAsset> buttonStack = ButtonHelpers.CreateButtonHorizontalStack(new Vector2(0.2f, 0.65f), 0.3f,
                0.1f, 0.025f,
                buttons.Select(t => new ButtonBasics(t, (args) =>
                {
                    dialog.HandleClick(t, buttons.IndexOf(t), completionSource, screen);
                })).ToList());

            screen.Buttons.AddRange(buttonStack);
            screen.Title = title;
            screen.Body = body;
            await screen.Initialise();

            Solids.Instance.ScreenManager.Add(screen);

            return await completionSource.Task;
        }

        public static async Task<DialogButton> ShowDialog(SmartSpriteBatch spriteBatch, string title, string body, List<DialogButton> buttons)
        {
            TaskCompletionSource<DialogButton> completionSource = new TaskCompletionSource<DialogButton>();
            XuiDialog dialog = new XuiDialog();
            DialogScreen screen = new DialogScreen
            {
                Buttons = new List<StaticButtonAsset>()
            };

            float margin = 0.005f;

            List<StaticButtonAsset> buttonStack = ButtonHelpers.CreateButtonHorizontalStack(new FloatRectangle(0.15f, 0.75f - margin - margin, 0.75f, 0.1f),
                0.1f, margin,
                buttons.Select(t => new ButtonBasics(t.Text, (args) =>
                {
                    dialog.HandleClick(t, completionSource, screen);
                })).ToList());

            screen.Buttons.AddRange(buttonStack);
            screen.Title = title;
            screen.Body = body;
            await screen.Initialise();

            Solids.Instance.ScreenManager.Add(screen);
            Solids.Instance.ScreenManager.BringToFront(screen);

            return await completionSource.Task;
        }

        //public static async Task<DialogButton> ShowTextDialog(SmartSpriteBatch spriteBatch, string title, string body, bool numberOnly, List<DialogButton> buttons)
        //{
    
        //    TaskCompletionSource<DialogButton> completionSource = new TaskCompletionSource<DialogButton>();
        //    XuiDialog dialog = new XuiDialog();
        //    TextEntryScreen screen = new TextEntryScreen
        //    {
        //        Buttons = new List<ButtonAsset>(),
        //        NumberOnly = numberOnly
        //    };

        //    float margin = 0.005f;



        //    List<ButtonAsset> buttonStack = ButtonAsset.CreateButtonHorizontalStack(new FloatRectangle(0.15f, 0.75f - margin - margin, 0.75f, 0.1f),
        //        0.1f, margin,
        //        buttons.Select(t => new ButtonBasics(t.Text, (args) =>
        //        {
        //            dialog.HandleTextEntryClick(t, completionSource, screen, screen.EnteredText);
        //        })).ToList());

        //    screen.Buttons.AddRange(buttonStack);
        //    screen.Title = title;
        //    screen.Body = body;
        //    await screen.Initialise();

        //    Solids.Screens.Add(screen);
        //    Solids.Screens.BringToFront(screen);

        //    return await completionSource.Task;
        //}

        public static void FireDialog(SmartSpriteBatch spriteBatch, string title, string body, List<DialogButton> buttons)
        {
            TaskCompletionSource<DialogButton> completionSource = new TaskCompletionSource<DialogButton>();
            XuiDialog dialog = new XuiDialog();
            DialogScreen screen = new DialogScreen
            {
                Buttons = new List<StaticButtonAsset>()
            };

            float margin = 0.02f;
            float fontMargin = 0.03f;

            List<StaticButtonAsset> buttonStack = ButtonHelpers.CreateButtonHorizontalStack(new FloatRectangle(0.15f, 0.75f - margin, 0.75f, 0.1f),
                0.1f, margin,
                buttons.Select(t => new ButtonBasics(t.Text, (args) =>
                {
                    dialog.HandleClick(t, completionSource, screen);
                })).ToList(), fontMargin);

            screen.Buttons.AddRange(buttonStack);
            screen.Title = title;
            screen.Body = body;
            screen.Initialise();

            Solids.Instance.ScreenManager.Add(screen);
            Solids.Instance.ScreenManager.BringToFront(screen);

        }



        public class DialogButton
        {
            public string Text { get; set; }
            public Action Action { get; set; }
            public string EnteredText { get; set; }
            public DialogButton(string text, Action action)
            {
                this.Text = text;
                this.Action = action;
                
            }
        }

        public void HandleClick(string clickText, int indexOf, TaskCompletionSource<int> completionSource, DialogScreen screen)
        {
            Solids.Instance.ScreenManager.Remove(screen);
            completionSource.SetResult(indexOf);
        }

        public void HandleClick(DialogButton button, TaskCompletionSource<DialogButton> completionSource, DialogScreen screen)
        {
            Solids.Instance.ScreenManager.Remove(screen);
            completionSource.SetResult(button);
            button.Action?.Invoke();
        }

        //public void HandleTextEntryClick(DialogButton button, TaskCompletionSource<DialogButton> completionSource, TextEntryScreen screen,string enteredText)
        //{
        //    Solids.Screens.Remove(screen);
        //    completionSource.SetResult(new DialogButton(button.Text,button.Action){EnteredText = enteredText});
        //    button.Action?.Invoke();
        //}

    }
}
