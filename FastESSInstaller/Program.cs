﻿using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FastESSInstaller
{
    class Program
    {
        
        static JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, //без этого русские буквы будут записаны неправильно
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //запысывать null в конфиги не нужно
            ReadCommentHandling = JsonCommentHandling.Skip //чтобы не ругался на комментарии в конфигах
        };
        static GeneralConfiguration generalConfiguration = new GeneralConfiguration();

        static void ClearOldFilesIfExists()
        {
            try
            {
                Console.Write("Удаление старых папок и файлов... ");
                foreach (var folder in generalConfiguration.ServiceFolders)
                {
                    DirectoryInfo serviceFolder = new DirectoryInfo(Path.Combine(generalConfiguration.InstanceFolder, folder));
                    if (serviceFolder.Exists)//если папка существует, то удаляем
                    {
                        foreach (FileInfo file in serviceFolder.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in serviceFolder.GetDirectories())
                        {
                            dir.Delete(true);
                        }
                        serviceFolder.Delete(true);
                    }
                }
                Console.WriteLine("ОК \n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при удалении старых папок и файлов: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void CreateFolders()
        {
            try
            {
                Console.Write("Создание папок для сервисов... ");
                foreach (var folder in generalConfiguration.ServiceFolders)
                    Directory.CreateDirectory(Path.Combine(generalConfiguration.InstanceFolder, folder));
                Console.WriteLine("ОК \n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при создании папок: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void CheckIfExistAndExtractFiles(string zipFilePath, string extractPath, bool overwrite)
        {
            try
            {
                Console.Write($"Извлечение архива {zipFilePath.Substring(zipFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)}... ");
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(extractPath, entry.FullName);
                        // Проверяем, является ли текущий элемент каталогом
                        if (entry.FullName.EndsWith("/"))
                        {
                            // Создаем папку, если она не существует
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            //Для избежания ошибки "The file ... already exist"
                            if (!File.Exists(destinationPath))
                            {
                                // Создаем папку, если она не существует
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                                entry.ExtractToFile(destinationPath, overwrite);
                            }
                        }
                    }
                }
                Console.WriteLine("ОК");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при распаковке архива: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void ExtractArhives()
        {
            try
            {
                //много похожих строк, как иначе - хз, потому что нужно сохранять порядок извлечения
                //извлечение архивов без замены файлов (с флагом false)
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"MessageBroker\WebApiService-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker/WebApiService"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"MessageBroker\Scheduler-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"MessageBroker\SmscTransport-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"MessageBroker\MobilGroupTransport-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"MessageBroker\MtsMarketologTransport-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\Sheduler"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"IdentityService\IdentityService-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"IdentityService"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"IdentityService\EssPlatformIdentityProvider2-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"IdentityService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"StorageService\StorageApi.Service-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"StorageService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"DocumentService\WebApiService-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"DocumentService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"SignService\Directum.Core.SignService.App-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"SignService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"Ess\OfficeService-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"EssService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices, @"Ess\Site-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"EssSite"), false);

                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices.Replace(@"\Components", @"\Tools"), @"Ess-Cli-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"EssCLI"), false);
                CheckIfExistAndExtractFiles(Path.Combine(generalConfiguration.PathToArhievedServices.Replace(@"\Components", @"\Tools"), @"Id-Cli-win-x64.zip"),
                    Path.Combine(generalConfiguration.InstanceFolder, @"IdCLI"), false);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при распаковке архивов: {ex.Message}");
            }
        }
        static void CreateDatabasesIfNotExist()
        {
            try
            {
                string connStr = $"Server={generalConfiguration.IdentityServiceHost};Port={generalConfiguration.DataBase.Port};User Id={generalConfiguration.DataBase.Username};Password={generalConfiguration.DataBase.Password};";
                var connection = new NpgsqlConnection(connStr);
                Console.WriteLine("Проверка наличия БД... ");
                connection.Open();
                var databaseNames = new List<string> {
                    generalConfiguration.DataBase.EssDatabaseName,
                    generalConfiguration.DataBase.IdDatabaseName,
                    generalConfiguration.DataBase.MessagesDatabaseName
                };
                string checkDbQuery = $"SELECT datname FROM pg_database WHERE datname IN ('{string.Join("', '", databaseNames)}')";
                using (var command = new NpgsqlCommand(checkDbQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string existingDbName = reader.GetString(0);
                            databaseNames.Remove(databaseNames.Single(name => name.Equals(existingDbName))); //из списка БД убираются уже существующие
                            Console.WriteLine($"База данных '{existingDbName}' уже существует");
                        }
                    }
                }
                foreach (var dbName in databaseNames)
                {
                    try
                    {
                        string createDbQuery = $"CREATE DATABASE \"{dbName}\" WITH OWNER = postgres ENCODING = 'UTF8' CONNECTION LIMIT = -1;";
                        using (var createCommand = new NpgsqlCommand(createDbQuery, connection))
                        {
                            createCommand.ExecuteNonQuery();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"База данных '{dbName}' создана успешно");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Произошла ошибка при создании БД '{dbName}': {ex.Message}");
                        Console.ResetColor();
                    }
                }
                connection.Close();

                Console.WriteLine("ОК \n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при создании БД: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void ConfigureDB()
        {
            try
            {
                string connectionToEssString = $"Server={generalConfiguration.IdentityServiceHost};Port={generalConfiguration.DataBase.Port};Database={generalConfiguration.DataBase.EssDatabaseName};User Id={generalConfiguration.DataBase.Username};Password={generalConfiguration.DataBase.Password};";
                string connectionToIdentityString = $"Server={generalConfiguration.IdentityServiceHost};Port={generalConfiguration.DataBase.Port};Database={generalConfiguration.DataBase.IdDatabaseName};User Id={generalConfiguration.DataBase.Username};Password={generalConfiguration.DataBase.Password};";
                string connectionToMessagesString = $"Server={generalConfiguration.MessagingServiceHost};Port={generalConfiguration.DataBase.Port};Database={generalConfiguration.DataBase.MessagesDatabaseName};User Id={generalConfiguration.DataBase.Username};Password={generalConfiguration.DataBase.Password};";
                Console.Write("Выполнение скриптов настройки БД... ");
                var EssScriptPath = $@"{generalConfiguration.PathToArhievedServices}\Ess\DatabaseScripts\PostgreSql\InitializeDatabase.sql";
                var IdentityScriptPath = $@"{generalConfiguration.PathToArhievedServices}\IdentityService\DatabaseScripts\PostgreSql\InitializeDatabase.sql";
                var MessagesScriptPath = $@"{generalConfiguration.PathToArhievedServices}\MessageBroker\DatabaseScripts\PostgreSql\InitializeDatabase.sql";
                string script = File.ReadAllText(EssScriptPath);
                using (var connection = new NpgsqlConnection(connectionToEssString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                script = File.ReadAllText(IdentityScriptPath);
                using (var connection = new NpgsqlConnection(connectionToIdentityString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                script = File.ReadAllText(MessagesScriptPath);
                using (var connection = new NpgsqlConnection(connectionToMessagesString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("ОК \n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при настройке БД: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillDocumentServiceConfig()
        {
            try
            {
                Console.Write("Заполнение конфига DocumentService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "DocumentService", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<DocumentServiceConfiguration>(file);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServicePort};User ID=EssServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига DocumentService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillEssCLIConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига EssCLI... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "EssCLI", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<EssCLIConfiguration>(file);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.EssDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига DocumentService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillEssServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига EssService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "EssService", "appsettings.json");
                var file = File.ReadAllText(configPath, Encoding.UTF8);
                var json = JsonSerializer.Deserialize<EssServiceConfiguration>(file);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.EssDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.ConnectionStrings.RabbitMQ = generalConfiguration.RabbitMQ;
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServicePort};User ID=EssServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
                json.ConnectionStrings.SignService = $"Name=Directum.Core.SignService;Host={generalConfiguration.SignServiceHost};UseSsl=true;Port={generalConfiguration.SignServicePort};";
                json.ConnectionStrings.MessagingService = $"Name=Directum.Core.MessageBroker;Host={generalConfiguration.MessagingServiceHost};UseSsl=false;Port={generalConfiguration.MessagingServicePort};";
                json.ConnectionStrings.DocumentService = $"Name=Directum.Core.DocumentService;Host={generalConfiguration.DocumentServiceHost};UseSsl=false;Port={generalConfiguration.DocumentServicePort};";
                json.ConnectionStrings.PreviewService = "";
                json.ConnectionStrings.PreviewStorage = "";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига EssService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillEssSiteConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига EssSite... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "EssSite", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<EssSiteConfiguration>(file);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServicePort};User ID=EssServiceUser;Password=11111;";
                json.ConnectionStrings.OfficeService = $"Name=Directum.Core.EssService;Host={generalConfiguration.EssServiceHost};UseSsl=false;Port={generalConfiguration.EssServicePort};User ID=EssServiceUser;Password=11111;";
                json.Authentication.ReturnUrl = $"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                json.ClientConfiguration.ServiceEndpoint = "/office/";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига EssSite: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillIdCLIConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига IdCLI... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "IdCLI", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<IdCLIConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.IdDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига IdCLI: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillIdentityServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига IdentityService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "IdentityService", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<IdentityServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.IdDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.ConnectionStrings.MessagingService = $"Name=Directum.Core.MessageBroker;Host={generalConfiguration.MessagingServiceHost};UseSsl=false;Port={generalConfiguration.MessagingServicePort};";
                json.General.ServiceEndpoint = $"https://{generalConfiguration.IdentityServiceHost}:{generalConfiguration.IdentityServicePort}";
                json.UserDevices.DeviceFingerprintSalt = "1234567890";
                json.TokenIssuer.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                json.AccountEnrichment.Providers[0].Configuration.IntegrationServiceEndpoint = generalConfiguration.IntegrationServiceEndpoint;
                json.AccountEnrichment.Providers[0].Configuration.ServiceUsername = generalConfiguration.IntegrationServiceUser;
                json.AccountEnrichment.Providers[0].Configuration.ServiceUserPassword = generalConfiguration.IntegrationServicePassword;
                json.AccountEnrichment.Providers[0].Configuration.EssPlatformVersion = generalConfiguration.EssPlatformVersion;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига IdentityService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillShedulerServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига ShedulerService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\Sheduler", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<ShedulerServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.MessagesDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.Transport.SmsDeliveryProxy = "Sms";
                json.Transport.Proxies = new Proxy[] { json.Transport.Proxies[0] };
                json.Transport.Proxies[0].Name = "Sms";
                json.Transport.Proxies[0].Configuration.Username = "DIDSMS";
                json.Transport.Proxies[0].Configuration.Password = "7863400";
                json.Transport.Proxies[0].Configuration.Sender = "DIDSMS";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига ShedulerService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillWebApiServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига WebApiService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, @"MessageBroker\WebApiService", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<WebApiServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.MessagesDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.Authentication.TrustedIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.EncryptionKey = "";
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                json.Scheduler.HealthCheckUrl = $"http://{generalConfiguration.ShedulingServiceHost}:{generalConfiguration.ShedulingServicePort}/health";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига WebApiService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillSignServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига SignService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "SignService", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<SignServiceConfiguration>(file, options);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServicePort};User ID=DocServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
                json.Authentication.Audience = "Directum.Core.SignService";
                json.Authentication.TrustedIssuers[0].SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                var provider = new CloudSignProvider();
                provider.CloudSignService = "Name=CloudSigningService;Host=ca.foxtrot.comp.npo;Port=7002;UseSsl=false;";
                provider.CloudAuthService = "Name=CloudSigningService;Host=ca.foxtrot.comp.npo;Port=7002;UseSsl=false;";
                provider.Name = "Directum СОП";
                provider.ProviderId = "directum-css";
                provider.ProviderType = "CSS";
                provider.PluginType = "CloudSigningService";
                provider.IdentificationTypes.Add("Personal");
                provider.ClientId = "directum";
                provider.SignServiceName = "directumss";
                provider.OperatorLogin = "SignServiceOperator";
                provider.OperatorPassword = "11111";
                json.Providers.Add(provider);
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига SignService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillStorageServiceConfig()
        {
            try
            {
                Console.Write($"Заполнение конфига StorageService... ");
                var configPath = Path.Combine(generalConfiguration.InstanceFolder, "StorageService", "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<StorageServiceConfiguration>(file, options);
                json.General.FileStoragePath = @".\storage";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.Audience = "Directum.Core.BlobStorageService";
                json.Authentication.EncryptionKey = "";
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при заполнении конфига StorageService: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void FillConfigs()
        {
            FillDocumentServiceConfig();
            FillEssCLIConfig();
            FillEssServiceConfig();
            FillEssSiteConfig();
            FillIdCLIConfig();
            FillIdentityServiceConfig();
            FillShedulerServiceConfig();
            FillWebApiServiceConfig();
            FillSignServiceConfig();
            FillStorageServiceConfig();
            Console.WriteLine();
        }
        static void AddSiteToIIS(string siteName, string protocol, string port, string path)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    if (iisManager.Sites[siteName] != null)
                    {
                        Console.WriteLine($"Сайт с именем '{siteName}' уже существует.");
                        return;
                    }
                    var site = iisManager.Sites.Add(siteName, protocol, $"*:{port}:", (generalConfiguration.InstanceFolder + @"\" + path));
                    var newPool = iisManager.ApplicationPools.Add(siteName);
                    newPool.ManagedRuntimeVersion = ""; //Версия среды CLR .NET: Без управляемого кода
                    newPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                    site.ApplicationDefaults.ApplicationPoolName = newPool.Name;
                    iisManager.CommitChanges();
                    Console.WriteLine($"Сайт и пул {siteName} успешно добавлены.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при добавлении сайта или пула {siteName}: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void DeleteSiteFromIIS(string siteName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = iisManager.Sites[siteName];
                    var pool = iisManager.ApplicationPools[siteName];
                    if (site == null && pool == null)
                        return;
                    //Если сайт существует, то удалить, чтобы создать новый
                    if (site != null)
                    {
                        if (!site.State.Equals(ObjectState.Stopped)) //обход ошибки "Служба не запущена."
                            site.Stop();
                        iisManager.Sites.Remove(site);
                    }

                    if (pool != null)
                    {
                        if (!pool.State.Equals(ObjectState.Stopped))
                            pool.Stop();
                        iisManager.ApplicationPools.Remove(pool);
                    }
                    iisManager.CommitChanges();
                    Console.WriteLine($"{siteName} удален из IIS");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при удалении сайта или пула: {ex.Message}");
                Console.ResetColor();
            }
        }
        static void AddRedirectRule(XmlDocument xmlDoc, XmlNode rulesNode, string ruleName, string matchUrl, string redirectUrl)
        {
            // Создание нового правила перенаправления
            XmlElement ruleElement = xmlDoc.CreateElement("rule");
            ruleElement.SetAttribute("name", ruleName);
            ruleElement.SetAttribute("stopProcessing", "true");

            // Создание узла <match>
            XmlElement matchElement = xmlDoc.CreateElement("match");
            matchElement.SetAttribute("url", matchUrl);
            ruleElement.AppendChild(matchElement);

            // Создание узла <action>
            XmlElement actionElement = xmlDoc.CreateElement("action");
            actionElement.SetAttribute("type", "Rewrite");
            actionElement.SetAttribute("url", redirectUrl);
            ruleElement.AppendChild(actionElement);

            // Добавление нового правила в узел <rules>
            rulesNode.AppendChild(ruleElement);
        }
        static void StopAndDeleteOldSitesAndPools()
        {
            DeleteSiteFromIIS("DocumentService" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("EssService" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("EssSite" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("Identity" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("Sheduler" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("CoreMessageBroker" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("SignService" + generalConfiguration.InstanceTag);
            DeleteSiteFromIIS("Storage" + generalConfiguration.InstanceTag);

        }
        static void ConfigureIIS()
        {
            AddSiteToIIS("DocumentService" + generalConfiguration.InstanceTag, "http", generalConfiguration.DocumentServicePort, "DocumentService");

            AddSiteToIIS("EssService" + generalConfiguration.InstanceTag, "https", generalConfiguration.EssServicePort, "EssService");

            AddSiteToIIS("EssSite" + generalConfiguration.InstanceTag, "https", generalConfiguration.EssSitePort, "EssSite");
            string webConfigPath = generalConfiguration.InstanceFolder + @"\EssSite\web.config";
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(webConfigPath);

                // Получение узла <system.webServer>
                XmlNode systemWebServerNode = xmlDoc.SelectSingleNode("/configuration/system.webServer");
                if (systemWebServerNode == null)
                {
                    // Если узел не существует, создаем его
                    systemWebServerNode = xmlDoc.CreateElement("system.webServer");
                    xmlDoc.DocumentElement.AppendChild(systemWebServerNode);
                }

                // Создание узла <rewrite>
                XmlNode rewriteNode = xmlDoc.SelectSingleNode("/configuration/system.webServer/rewrite");
                if (rewriteNode == null)
                {
                    rewriteNode = xmlDoc.CreateElement("rewrite");
                    systemWebServerNode.AppendChild(rewriteNode);
                }

                // Создание узла <rules>
                XmlNode rulesNode = xmlDoc.SelectSingleNode("/configuration/system.webServer/rewrite/rules");
                if (rulesNode == null)
                {
                    rulesNode = xmlDoc.CreateElement("rules");
                    rewriteNode.AppendChild(rulesNode);
                    // Создание первого правила перенаправления
                    AddRedirectRule(xmlDoc, rulesNode, "storage", "^storage/(.*)", $"http://localhost:{generalConfiguration.StorageServicePort}/" + "{R:1}");

                    // Создание второго правила перенаправления
                    AddRedirectRule(xmlDoc, rulesNode, "api", "^api/(.*)", $"https://localhost:{generalConfiguration.EssServicePort}/api/" + "{R:1}");
                }
                xmlDoc.Save(webConfigPath);

                Console.WriteLine($"Настройки перенаправления для сайта {"EssSite" + generalConfiguration.InstanceTag} успешно сохранены в web.config.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.ResetColor();
            }

            AddSiteToIIS("Identity" + generalConfiguration.InstanceTag, "https", generalConfiguration.IdentityServicePort, "IdentityService");

            AddSiteToIIS("Sheduler" + generalConfiguration.InstanceTag, "http", generalConfiguration.ShedulingServicePort, @"MessageBroker\Sheduler");

            AddSiteToIIS("CoreMessageBroker" + generalConfiguration.InstanceTag, "http", generalConfiguration.MessagingServicePort, @"MessageBroker\WebApiService");

            AddSiteToIIS("SignService" + generalConfiguration.InstanceTag, "https", generalConfiguration.SignServicePort, "SignService");

            AddSiteToIIS("Storage" + generalConfiguration.InstanceTag, "http", generalConfiguration.StorageServicePort, "StorageService");
            Console.WriteLine();
        }
        static async Task ExecuteCommandAsync(string workingDirectory, string command)
        {
            try
            {
                Console.WriteLine(command);
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/c {command}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.WorkingDirectory = workingDirectory;
                    process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            if (e.Data.Contains("is not exists"))
                                Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(e.Data);
                            Console.ResetColor();
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ошибка: " + e.Data);
                            Console.ResetColor();
                        }
                    };
                    process.Start();

                    // Начинаем асинхронное чтение вывода и ошибок
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // Создаем задачу для ожидания завершения процесса
                    var waitTask = Task.Run(() => process.WaitForExit());
                    // Ожидаем ввода от пользователя
                    while (!process.HasExited)
                    {
                        if (!Console.IsInputRedirected && Console.KeyAvailable) // Проверяем, доступен ли ввод
                        {
                            string userInput = await Console.In.ReadLineAsync();
                            if (process.StandardInput.BaseStream.CanWrite)
                            {
                                await process.StandardInput.WriteLineAsync(userInput);
                            }
                        }
                    }
                    await waitTask;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Произошла ошибка при выполнении консольных команд: " + ex.Message);
                Console.ResetColor();
            }
        }
        static async Task ConnectToRX()
        {
            try
            {
                Console.WriteLine("Выполнение консольных команд... ");
                var hrpro_AdapterConfigPath = Path.Combine(generalConfiguration.HRProRepositoryPath, "data\\AdapterConfig\\HRPro_AdapterConfig.json");
                var pathToEssCLI = Path.Combine(generalConfiguration.InstanceFolder, "EssCLI");
                var pathToIdCLI = Path.Combine(generalConfiguration.InstanceFolder, "IdCLI");
                await ExecuteCommandAsync(pathToEssCLI, $"ess connect \"{generalConfiguration.EssBaseEndpointPath}\" -p UserIdentity=\"DirectumRX\" -p " +
                    $"Configuration:AppServerConnection:Endpoint=\"{generalConfiguration.IntegrationServiceEndpoint}\" -p " +
                    $"Configuration:AppServerConnection:UserName=\"{generalConfiguration.IntegrationServiceUser}\" -p " +
                    $"Configuration:AppServerConnection:Password=\"{generalConfiguration.IntegrationServicePassword}\" -p " +
                    $"Configuration:ServerVersion=\"{generalConfiguration.RxVersion}\"");
                await ExecuteCommandAsync(pathToEssCLI, $"ess connect \"{hrpro_AdapterConfigPath}\" -p UserIdentity=\"DirectumRX\" -p " +
                    $"Configuration:AppServerConnection:Endpoint=\"{generalConfiguration.IntegrationServiceEndpoint}\" -p " +
                    $"Configuration:AppServerConnection:UserName=\"{generalConfiguration.IntegrationServiceUser}\" -p " +
                    $"Configuration:AppServerConnection:Password=\"{generalConfiguration.IntegrationServicePassword}\" -p " +
                    $"Configuration:ServerVersion=\"{generalConfiguration.RxVersion}\"");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.EssBasePath}\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.ESSRepositoryPath}\\data\\EssConfig\\Roles.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.ESSRepositoryPath}\\data\\EssConfig\\SignPlatform.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.HRProRepositoryPath}\\data\\EssConfig\\HRDocFlow\\HrProStatements.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.HRProRepositoryPath}\\data\\EssConfig\\Vacations\\Vacations.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.HRProRepositoryPath}\\data\\EssConfig\\Hiring\\Hiring.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.HRProRepositoryPath}\\data\\EssConfig\\Hiring\\Roles.xml\" -a");
                await ExecuteCommandAsync(pathToEssCLI, $"ess install \"{generalConfiguration.ESSRepositoryPath}\\data\\EssConfig\\UsageAgreements\" -a");

                await ExecuteCommandAsync(pathToIdCLI, $"id add role \"service\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add user \"DocServiceUser\" -p password=\"11111\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id assign -u \"DocServiceUser\" -r \"service\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.DocumentService\" -c {generalConfiguration.PathToArhievedServices}\\DocumentService\\DocumentServiceAudience.json\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add user \"EssServiceUser\" -p password=\"11111\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id assign -u \"EssServiceUser\" -r \"service\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.EssService\" -c {generalConfiguration.PathToArhievedServices}\\Ess\\EssServiceAudience.json\" -p icon=\"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}/logo_32.png\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.EssSite\" -c {generalConfiguration.PathToArhievedServices}\\Ess\\EssSiteAudience.json\" -p returnUrl=\"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}\" -p originUrl=\"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}\" -p icon=\"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}/logo_32.png\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.MessageBroker\" -c {generalConfiguration.PathToArhievedServices}\\MessageBroker\\MessageBrokerAudience.json\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add user \"SignServiceUser\" -p password=\"11111\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add user \"SignServiceOperator\" -p password=\"11111\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add role \"Admins\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add role \"Users\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id assign -u \"SignServiceUser\" -r \"service\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id assign -u \"SignServiceOperator\" -r \"Admins\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.SignService\" -c {generalConfiguration.PathToArhievedServices}\\SignService\\SignServiceAudience.json\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Directum.Core.BlobStorageService\" -c {generalConfiguration.PathToArhievedServices}\\StorageService\\BlobStorageServiceAudience.json\"");
                await ExecuteCommandAsync(pathToIdCLI, $"id add resource \"Integration service\" -c  {generalConfiguration.PathToArhievedServices}\\Ess\\RXIntegrationServiceAudience.json\"");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при выполнении консольных команд: {ex.Message}");
                Console.ResetColor();
            }
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            string configPath = @"\FastESSInstaller\FastESSInstaller\Config.json";
            string releasePath = Path.Combine(new FileInfo(Process.GetCurrentProcess().MainModule.FileName).DirectoryName, @"Config.json");
            if (File.Exists(releasePath))
                configPath = releasePath;
            Console.WriteLine("Используется конфиг по пути: " + configPath);
            if (File.Exists(configPath))
            {
                try
                {
                    Console.Write($"Чтение Config.json... ");
                    string data = File.ReadAllText(configPath);
                    generalConfiguration = JsonSerializer.Deserialize<GeneralConfiguration>(data, options);
                    Console.WriteLine($"OK");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Произошла ошибка при чтении Config.json: {ex.Message}");
                    Console.ResetColor();
                }
                Process[] processes = Process.GetProcessesByName("ess");
                foreach (Process process in processes)
                {
                    Console.WriteLine(process.ProcessName + " " + process.MainModule.FileName);
                    if (process.MainModule.FileName.Contains(generalConfiguration.InstanceFolder))
                    {
                        //process.Kill();
                        Console.WriteLine($"Process {process.ProcessName} has been killed.");
                    }
                }
                Console.WriteLine();
                StopAndDeleteOldSitesAndPools();
                ClearOldFilesIfExists();
                CreateFolders();
                ExtractArhives();
                CreateDatabasesIfNotExist();
                ConfigureDB();
                FillConfigs();
                ConfigureIIS();
                await ConnectToRX();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Не удалось найти Config.json в папке проекта");
                Console.ResetColor();
            }
            Console.ReadKey();
        }
    }
}
