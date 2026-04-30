using BlogWebsite.Configurations;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace BlogWebsite.Services
{
    public class SftpService
    {
        private readonly SftpSettings _settings;
        public string BasePath => _settings.BasePath;
        public SftpService(IOptions<SftpSettings> options)
        {
            _settings = options.Value;
        }
        public void UploadFile(Stream fileStream, string fileName)
        {
            using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
            client.Connect();

            if (!client.Exists(_settings.BasePath))
            {
                client.CreateDirectory(_settings.BasePath);
            }

            var remoteFilePath = $"{_settings.BasePath}/{Path.GetFileName(fileName)}";
            fileStream.Position = 0;
            client.UploadFile(fileStream, remoteFilePath);
            client.Disconnect();
        }

        public Stream DownloadFile(string fileName)
        {
            using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
            client.Connect();

            var remotePath = $"{_settings.BasePath}/{fileName}";
            if (!client.Exists(remotePath))
            {
                client.Disconnect();
                return null;
            }

            var ms = new MemoryStream();
            client.DownloadFile(remotePath, ms);
            ms.Position = 0; // reset stream
            client.Disconnect();

            return ms;
        }
    }
}
