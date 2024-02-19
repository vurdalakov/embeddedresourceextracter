namespace Vurdalakov.EmbeddedResourceExtracter
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Resources;

    internal class Program
    {
        static void Main(String[] args)
        {
            var assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"{(assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute).Title} {assembly.GetName().Version} | {(assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute).Copyright} | {(assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute).Description}");

            if (args.Length < 2)
            {
                Help();
            }

            var assemblyFilePath = args[1];
            var fileNameMask = args.Length < 3 ? null : args[2];

            try
            {
                switch (args[0].ToLower())
                {
                    case "list":
                    case "l":
                        List(assemblyFilePath);
                        break;
                    case "extract":
                    case "x":
                        Extract(assemblyFilePath, fileNameMask);
                        break;
                    default:
                        Help();
                        break;
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ({ex.GetType().Name}): {ex.Message}");
                Environment.Exit(2);
            }
        }

        private static void List(String assemblyFilePath)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFilePath);

            var resourceNames = assembly.GetManifestResourceNames();
            Console.WriteLine($"Listing {resourceNames.Length} resources:");

            foreach (var resourceName in resourceNames)
            {
                Console.WriteLine(resourceName);
            }
        }

        private static void Extract(String assemblyFilePath, String fileNameMask)
        {
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFilePath);

            var resourceNames = assembly.GetManifestResourceNames();
            Console.WriteLine("Extracting resources:");

            if (0 == resourceNames.Length)
            {
                Console.WriteLine("No resources found");
                return;
            }

            var directoryName = Path.GetFileNameWithoutExtension(assemblyFilePath) + ".EmbeddedResources";
            var directoryPath = Path.Combine(Environment.CurrentDirectory, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var count = 0;

            foreach (var resourceName in resourceNames)
            {
                if (!String.IsNullOrEmpty(fileNameMask) && (resourceName.IndexOf(fileNameMask, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    continue;
                }

                Console.WriteLine(resourceName);
                count++;

                var filePath = Path.Combine(directoryPath, resourceName);

                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                    {
                        var resourcesFilePath = Path.ChangeExtension(filePath, ".resx");

                        try
                        {
                            using (var reader = new ResourceReader(resourceStream))
                            {
                                using (var writer = new ResXResourceWriter(resourcesFilePath))
                                {
                                    foreach (DictionaryEntry entry in reader)
                                    {
                                        writer.AddResource(entry.Key.ToString(), entry.Value);
                                    }
                                }
                            }

                            continue;
                        }
                        catch
                        {
                            if (File.Exists(resourcesFilePath))
                            {
                                File.Delete(resourcesFilePath);
                            }
                        }
                    }

                    using (var writer = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new Byte[65536];
                        while (true)
                        {
                            var bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
                            if (0 == bytesRead)
                            {
                                break;
                            }
                            writer.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }

            Console.WriteLine($"{count} resources extracted");
        }

        private static void Help()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("* List embedded resources:");
            Console.WriteLine("EmbeddedResourceExtracter list <dll-name>");
            Console.WriteLine("* Extract all embedded resources:");
            Console.WriteLine("EmbeddedResourceExtracter extract <dll-name>");
            Console.WriteLine("* Extract individual embedded resources:");
            Console.WriteLine("EmbeddedResourceExtracter extract <dll-name> <file-name-mask>");

            Environment.Exit(1);
        }
    }
}