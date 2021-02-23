using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace PusherServer.Tests.Helpers
{
    /// <summary>
    /// Loads test configuration from a json file.
    /// </summary>
    public class JsonFileConfigLoader : IApplicationConfigLoader
    {
        private const string DefaultFileName = "AppConfig.test.json";

        /// <summary>
        /// Creates an instance of a <see cref="JsonFileConfigLoader"/>.
        /// </summary>
        public JsonFileConfigLoader()
            : this(Path.Combine(Assembly.GetExecutingAssembly().Location, $"../../../../../{DefaultFileName}"))
        {
        }

        /// <summary>
        /// Creates an instance of a <see cref="JsonFileConfigLoader"/>.
        /// </summary>
        /// <param name="fileName">The location of the file that contains the settings.</param>
        public JsonFileConfigLoader(string fileName)
        {
            this.FileName = fileName;
        }

        public static IApplicationConfigLoader Default { get; } = new JsonFileConfigLoader();

        /// <summary>
        /// Gets or sets the location of the file that contains the settings.</param>
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Loads test configuration from a json file.
        /// </summary>
        /// <returns>An <see cref="IApplicationConfig"/> instance.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the json settings file does not exist.</exception>
        public IApplicationConfig Load()
        {
            FileInfo fileInfo = new FileInfo(this.FileName);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"The JSON test application configuration file was not found at location {fileInfo.FullName}.", fileName: this.FileName);
            }

            string content = File.ReadAllText(fileInfo.FullName);
            ApplicationConfig result = JsonConvert.DeserializeObject<ApplicationConfig>(content);
            return result;
        }
    }
}
