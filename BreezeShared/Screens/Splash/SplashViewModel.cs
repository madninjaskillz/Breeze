using System;
using System.Collections.Generic;
using System.Text;

namespace Breeze.Screens.Splash
{
    public class SplashViewModel : VirtualizedDataContext
    {
        private string testText = "ooh hello";
        [Databound]
        public string TestText
        {
            get => testText;
            set => Set(ref testText, value);
        }
    }
}
