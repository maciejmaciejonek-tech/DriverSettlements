using System.Reflection;

namespace DriverSettlements.Common.Helpers
{
    public class ApplicationLocationProvider
    {
        private static string overriddenLocation = string.Empty;

        private ApplicationLocationProvider()
        {
        }

        /// <summary>
        /// Gets or sets the path to the application folder
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(overriddenLocation))
                {
                    var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    if (appPath == null)
                        throw new Exception("");
                    return appPath;
                }
                else
                    return overriddenLocation;

            }
            set { overriddenLocation = value; }
        }

        /// <summary>
        /// Gets the path to the element below the application folder.
        /// </summary>
        /// <param name="pathFragments">Path fragments.</param>
        /// <returns>Path.</returns>
        public static string GetSubDirectoryPath(params string[] pathFragments)
        {
            string path = ApplicationPath;
            for (int i = 0; i < pathFragments.Length; i++)
                path = Path.Combine(path, pathFragments[i]);
            return path;
        }
    }
}
