using System.IO;
using System.Reflection;

namespace MarioMaker2Overlay.Utility
{
    public static class EmbeddedResourceUtility
    {
        public static string GetEmbeddedResourceContentAsString(string fileName)
        {
            string commandText = string.Empty;

            Assembly assembly = Assembly.Load("MarioMaker2Overlay");
            Stream resourceStream = assembly.GetManifestResourceStream(fileName);
            using (StreamReader reader = new(resourceStream))
            {
                commandText = reader.ReadToEnd();
            }

            return commandText;
        }
    }
}
