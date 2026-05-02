using BlogWebsite.Configurations;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace BlogWebsite.Services
{
    public class SftpService
    {

        private readonly string _host;
        //private readonly string _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _baseUrl;

        public SftpService(IConfiguration config)
        {
            _host = config["SftpConfig:Host"];
            //_port = config["Sftp:Port"];
            _username = config["SftpConfig:Username"];
            _password = config["SftpConfig:Password"];
            _baseUrl = config["SftpConfig:BaseUrl"];
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            using var client = new SftpClient(_host, _username, _password);
            client.Connect();

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var remotePath = "/uploads/" + fileName;

            using var stream = file.OpenReadStream();
            if (!client.IsConnected)
                throw new Exception("SFTP connection failed");

            if (!client.Exists("/uploads"))
                client.CreateDirectory("/uploads");
            await Task.Run(() => client.UploadFile(stream, remotePath));

            client.Disconnect();

            // 👉 Public URL (depends on your server setup)
            return _baseUrl.TrimEnd('/') + "/" + fileName;
        }

        public async Task<Stream?> DownloadFileAsync(string fileName)
        {
            var remotePath = "/uploads/" + fileName;
            using var client = new SftpClient(_host, _username, _password);
            try
            {
                client.Connect();
                if (!client.IsConnected)
                    throw new Exception($"SFTP connection to {_host} failed.");

                if (!client.Exists(remotePath))
                    return null;

                var ms = new MemoryStream();
                await Task.Run(() => client.DownloadFile(remotePath, ms));
                ms.Position = 0;
                return ms;
            }
            finally
            {
                // Note: client disposed AFTER stream is returned
                // MemoryStream is already fully buffered so safe to disconnect here
                if (client.IsConnected)
                    client.Disconnect();
                client.Dispose();
            }
        }


        //private readonly SftpSettings _settings;
        //public string BasePath => _settings.BasePath;
        //public SftpService(IOptions<SftpSettings> options)
        //{
        //    _settings = options.Value;
        //}
        //public void UploadFile(Stream fileStream, string fileName)
        //{
        //    using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
        //    client.Connect();

        //    if (!client.Exists(_settings.BasePath))
        //    {
        //        client.CreateDirectory(_settings.BasePath);
        //    }

        //    var remoteFilePath = $"{_settings.BasePath}/{Path.GetFileName(fileName)}";
        //    fileStream.Position = 0;
        //    client.UploadFile(fileStream, remoteFilePath);
        //    client.Disconnect();
        //}

        //public Stream DownloadFile(string fileName)
        //{
        //    using var client = new SftpClient(_settings.Host, _settings.Username, _settings.Password);
        //    client.Connect();

        //    var remotePath = $"{_settings.BasePath}/{fileName}";
        //    if (!client.Exists(remotePath))
        //    {
        //        client.Disconnect();
        //        return null;
        //    }

        //    var ms = new MemoryStream();
        //    client.DownloadFile(remotePath, ms);
        //    ms.Position = 0; // reset stream
        //    client.Disconnect();

        //    return ms;
        //}
    }
}
