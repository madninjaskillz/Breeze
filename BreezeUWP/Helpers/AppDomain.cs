using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Breeze.Helpers;


namespace Breeze.Helpers
{
    public sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            return GetAssemblyList().ToArray();
        }

        public List<string> AssembliesToNotLoad = new List<string>
        {
            "clrcompression.dll","clrjit.dll"
        };

        private IEnumerable<Assembly> GetAssemblyList()
        {
            var folder = FileLocation.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in Directory.GetFiles(folder))
            {
                if (!AssembliesToNotLoad.Contains(file.Split('\\').Last()))
                {
                    if (file.Split('.').Last() == "dll" || file.Split('.').Last() == "exe")
                    {
                        AssemblyName name = new AssemblyName()
                            {Name = Path.GetFileNameWithoutExtension(file.Split('\\').Last())};
                        try
                        {
                            Assembly asm = Assembly.Load(name);
                            assemblies.Add(asm);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Couldnt load {name} - {e.Message}");
                        }
                    }
                }
            }

            return assemblies;
        }
    }

}
