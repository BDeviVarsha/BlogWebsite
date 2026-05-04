using BlogWebsite.Configurations;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace BlogWebsite.Services
{
    public class SftpService
    {
        private readonly SftpSettings _settings;
        public SftpService(IOptions<SftpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
            client.Connect();

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var remotePath = "/uploads/" + fileName;

            using var stream = file.OpenReadStream();
            if (!client.IsConnected)
                throw new Exception("SFTP connection failed");

            if (!client.Exists("/uploads"))
                client.CreateDirectory("/uploads");
            client.UploadFile(stream, remotePath);

            client.Disconnect();

            //  Public URL (depends on your server setup)
            return _settings.BaseUrl.TrimEnd('/') + "/" + fileName;
        }

        public async Task<Stream?> DownloadFileAsync(string fileName)
        {
            var remotePath = "/uploads/" + fileName;
            using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
            try
            {
                client.Connect();
                if (!client.IsConnected)
                    throw new Exception($"SFTP connection to {_settings.Host} failed.");

                if (!client.Exists(remotePath))
                    return null;

                var ms = new MemoryStream();
                client.DownloadFile(remotePath, ms);
                ms.Position = 0;
                return ms;
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
                client.Dispose();
            }
        }
    }
}
