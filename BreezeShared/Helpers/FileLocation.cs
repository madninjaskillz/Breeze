using System;
#if WINDOWS_UAP
using Windows.Storage;
#endif

namespace Breeze.Helpers
{
    public static class FileLocation
    {
#if WINDOWS_UAP
     //   public static string LocalFolder => ApplicationData.Current.LocalFolder.Path;
        public static string InstalledLocation => Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
#endif

#if WINDOWS
   //     public static string LocalFolder => "\\userdata\\";
        public static string InstalledLocation => AppDomain.CurrentDomain.BaseDirectory;
#endif

#if LINUX || ANDROID
     //   public static string LocalFolder => "\\userdata\\";
        public static string InstalledLocation => "";
#endif
    }
}