using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Operational_Risk_Management.Models.Common
{
    // Helper class to load unmanaged libraries from a specific path.
    // This is often needed for DinkToPdf to find the wkhtmltopdf native library.
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base(isCollectible: false) // Typically non-collectible for persistent libraries
        {
        }

        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            // This method is called by the CLR when an unmanaged DLL needs to be loaded.
            // 'unmanagedDllName' can be just the DLL name (e.g., "libwkhtmltox.dll")
            // or a full path if LoadUnmanagedLibrary was called with a full path.

            // If 'unmanagedDllName' is already an absolute path (from our LoadUnmanagedLibrary call),
            // then we try to load it directly.
            if (Path.IsPathRooted(unmanagedDllName))
            {
                if (File.Exists(unmanagedDllName))
                {
                    try
                    {
                        return LoadUnmanagedDllFromPath(unmanagedDllName);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception if loading fails
                        Console.WriteLine($"Error loading unmanaged DLL from path '{unmanagedDllName}': {ex.Message}");
                        // Fall through to default loading or return IntPtr.Zero
                    }
                }
                else
                {
                    Console.WriteLine($"Unmanaged DLL not found at absolute path: '{unmanagedDllName}'");
                }
            }

            // Fallback to default loading mechanism if not already an absolute path
            // or if loading from absolute path failed.
            // This allows other unmanaged DLLs (not libwkhtmltox) to be loaded normally.
            try
            {
                return base.LoadUnmanagedDll(unmanagedDllName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading unmanaged DLL '{unmanagedDllName}' with base loader: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        // Optional: Override Load method if you were loading managed assemblies too,
        // but for DinkToPdf, LoadUnmanagedDll is the key.
        protected override Assembly Load(AssemblyName assemblyName)
        {
            // For managed assemblies, can implement custom logic or fall back to default.
            // Not strictly necessary for DinkToPdf's unmanaged library loading.
            return null; // Fallback to other contexts or default.
        }
    }
}