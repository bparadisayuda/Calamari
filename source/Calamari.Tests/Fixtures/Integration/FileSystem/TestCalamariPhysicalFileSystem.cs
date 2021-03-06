using System;
using System.IO;
using System.Reflection;
using Calamari.Integration.FileSystem;

namespace Calamari.Tests.Fixtures.Integration.FileSystem
{
    public class TestCalamariPhysicalFileSystem : CalamariPhysicalFileSystem
    {
        public new static ICalamariFileSystem GetPhysicalFileSystem()
        {
            if (CalamariEnvironment.IsRunningOnNix || CalamariEnvironment.IsRunningOnMac)
            {
                return new TestNixCalamariPhysicalFileSystem();
            }

            return new TestWindowsPhysicalFileSystem();
        }

        protected override bool GetDiskFreeSpace(string directoryPath, out ulong totalNumberOfFreeBytes)
        {
            throw new NotImplementedException("*testing* this is in TestCalamariPhysicalFileSystem");
        }

        private class TestNixCalamariPhysicalFileSystem : NixCalamariPhysicalFileSystem, ICalamariFileSystem
        {
            public new string CreateTemporaryDirectory()
            {
                var path = Path.Combine("/tmp", Guid.NewGuid().ToString());

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }
        private class TestWindowsPhysicalFileSystem : WindowsPhysicalFileSystem, ICalamariFileSystem
        {
            public new string CreateTemporaryDirectory()
            {
#if NET40
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#else
                var path = Environment.GetEnvironmentVariable("LOCALAPPDATA")
                            ?? Environment.GetEnvironmentVariable("TMPDIR")
                            ?? Environment.GetEnvironmentVariable("TEMP")
                            ?? @"C:\temp";
#endif
                path = Path.Combine(path, Assembly.GetEntryAssembly()?.GetName().Name ?? Guid.NewGuid().ToString());

                path = Path.Combine(path, Guid.NewGuid().ToString());

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }
    }
}