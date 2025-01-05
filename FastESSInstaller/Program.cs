using Microsoft.Web.Administration;
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

namespace FastESSInstaller
{
    class Program
    {
        static string configPath = @"\FastESSInstaller\FastESSInstaller\Config.json";
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
                Console.WriteLine("ОК \n");
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
        static void FillDocumentServiceConfig()
        {
            try
            {
                Console.Write("Заполнение конфига DocumentService... ");
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[0]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<DocumentServiceConfiguration>(file);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServiceHost};User ID=DocServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[1]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<EssCLIConfiguration>(file);
                json.DataBase = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.EssDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[2]), "appsettings.json");
                var file = File.ReadAllText(configPath, Encoding.UTF8);
                var json = JsonSerializer.Deserialize<EssServiceConfiguration>(file);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.EssDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.ConnectionStrings.RabbitMQ = generalConfiguration.RabbitMQ;
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServiceHost};User ID=DocServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
                json.ConnectionStrings.SignService = $"Name=Directum.Core.SignService;Host={generalConfiguration.SignServiceHost};UseSsl=true;Port={generalConfiguration.SignServicePort};";
                json.ConnectionStrings.MessagingService = $"Name=Directum.Core.MessageBroker;Host={generalConfiguration.MessagingServiceHost};UseSsl=false;Port={generalConfiguration.MessagingServicePort};";
                json.ConnectionStrings.DocumentService = $"Name=Directum.Core.DocumentService;Host={generalConfiguration.DocumentServiceHost};UseSsl=false;Port={generalConfiguration.DocumentServicePort};";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[3]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<EssSiteConfiguration>(file);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServiceHost};User ID=DocServiceUser;Password=11111;";
                json.ConnectionStrings.OfficeService = $"Name=Directum.Core.EssService;Host={generalConfiguration.EssServiceHost};UseSsl=false;Port={generalConfiguration.EssServicePort};";
                json.Authentication.ReturnUrl = $"https://{generalConfiguration.EssSiteHost}:{generalConfiguration.EssSitePort}";
                json.Authentication.TokenIssuer = generalConfiguration.TokenIssuer;
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[5]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<IdCLIConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.IdDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[6]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<IdentityServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.IdDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.ConnectionStrings.MessagingService = $"Name=Directum.Core.MessageBroker;Host={generalConfiguration.MessagingServiceHost};UseSsl=false;Port={generalConfiguration.MessagingServicePort};";
                json.General.ServiceEndpoint = $"https://localhost:{generalConfiguration.IdentityServicePort}";
                json.UserDevices.DeviceFingerprintSalt = "1234567890";
                json.TokenIssuer.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                json.AccountEnrichment.Providers[0].Configuration.IntegrationServiceEndpoint = generalConfiguration.IntegrationServiceEndpoint;
                json.AccountEnrichment.Providers[0].Configuration.ServiceUsername = generalConfiguration.IntegrationServiceUser;
                json.AccountEnrichment.Providers[0].Configuration.ServiceUserPassword = generalConfiguration.IntegrationServicePassword;
                json.AccountEnrichment.Providers[0].Configuration.EssPlatformVersion = generalConfiguration.EssPlatformVersion;
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[7]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<ShedulerServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.MessagesDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.Transport.SmsDeliveryProxy = "Sms";
                json.Transport.Proxies = new Proxy[] { json.Transport.Proxies[0] };
                json.Transport.Proxies[0].Configuration.Username = "DIDSMS";
                json.Transport.Proxies[0].Configuration.Password = "7863400";
                json.Transport.Proxies[0].Configuration.Sender = "DIDSMS";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[8]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<WebApiServiceConfiguration>(file, options);
                json.ConnectionStrings.Database = $"ProviderName=Npgsql;Host={generalConfiguration.DataBase.DBHost};Port={generalConfiguration.DataBase.Port};database={generalConfiguration.DataBase.MessagesDatabaseName};Username={generalConfiguration.DataBase.Username};password={generalConfiguration.DataBase.Password};";
                json.Authentication.SigningCertificateThumbprint = generalConfiguration.SigningCertificateThumbprint;
                json.Scheduler.HealthCheckUrl = $"http://{generalConfiguration.MessagingServiceHost}:{generalConfiguration.MessagingServicePort}/health";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[9]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<SignServiceConfiguration>(file, options);
                json.ConnectionStrings.IdentityService = $"Name=Directum.Core.IdentityService;Host={generalConfiguration.IdentityServiceHost};UseSsl=true;Port={generalConfiguration.IdentityServicePort};User ID=DocServiceUser;Password=11111;";
                json.ConnectionStrings.StorageService = $"Name=Directum.Core.BlobStorageService;Host={generalConfiguration.StorageServiceHost};UseSsl=false;Port={generalConfiguration.StorageServicePort};";
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
                Console.WriteLine("OK \n");
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
                var configPath = Path.Combine(Path.Combine(generalConfiguration.InstanceFolder, generalConfiguration.ServiceFolders[10]), "appsettings.json");
                var file = File.ReadAllText(configPath);
                var json = JsonSerializer.Deserialize<StorageServiceConfiguration>(file, options);
                json.General.FileStoragePath = @".\\storage";
                file = JsonSerializer.Serialize(json, options);
                File.WriteAllText(configPath, file);
                Console.WriteLine("OK \n");
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
        static void AddSiteToIIS(string siteName, string protocol, string binding, string path)
        {
            try
            {
                ServerManager iisManager = new ServerManager();
                Site site = iisManager.Sites.Add(siteName, protocol, $"*:{binding}:", generalConfiguration.InstanceFolder + @"\" + path);
                site.ApplicationDefaults.ApplicationPoolName = siteName;
                var newPool = iisManager.ApplicationPools.Add(siteName);
                newPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                newPool.ManagedRuntimeVersion = "Без управляемого кода";
                iisManager.CommitChanges();
                Console.WriteLine($"Сайт и пул {siteName} добавлены");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка при добавлении сайта или пула: {ex.Message}");
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
                    //Если сайт существует, то удалить
                    if (site != null)
                    {
                        iisManager.Sites.Remove(site);
                    }
                    var newPool = iisManager.ApplicationPools[siteName];
                    if (newPool != null)
                    {
                        iisManager.ApplicationPools.Remove(newPool);
                    }
                    iisManager.CommitChanges();
                    Console.WriteLine($"Сайт и пул {siteName} удалены");
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
        static void ConfigureIIS()
        {
            DeleteSiteFromIIS("DocumentService" + generalConfiguration.InstanceTag);
            AddSiteToIIS("DocumentService" + generalConfiguration.InstanceTag, "http", generalConfiguration.InstanceTag + 0, "DocumentService");

            DeleteSiteFromIIS("EssService" + generalConfiguration.InstanceTag);
            AddSiteToIIS("EssService" + generalConfiguration.InstanceTag, "https", generalConfiguration.InstanceTag + 1, "EssService");

            DeleteSiteFromIIS("EssSite" + generalConfiguration.InstanceTag);
            AddSiteToIIS("EssSite" + generalConfiguration.InstanceTag, "https", generalConfiguration.InstanceTag + 2, "EssSite");
            string webConfigPath = generalConfiguration.InstanceFolder + @"\EssSite\web.config";
            try
            {
                // Загрузка web.config как XML-документ
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
                    AddRedirectRule(xmlDoc, rulesNode, "storage", "^storage/(.*)", $"http://localhost:{generalConfiguration.InstanceTag + 2}/" + "{R:1}");

                    // Создание второго правила перенаправления
                    AddRedirectRule(xmlDoc, rulesNode, "api", "^api/(.*)", $"http://localhost:{generalConfiguration.InstanceTag + 1}/api/" + "{R:1}");
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

            DeleteSiteFromIIS("Identity" + generalConfiguration.InstanceTag);
            AddSiteToIIS("Identity" + generalConfiguration.InstanceTag, "https", generalConfiguration.InstanceTag + 3, "IdentityService");

            DeleteSiteFromIIS("Sheduler" + generalConfiguration.InstanceTag);
            AddSiteToIIS("Sheduler" + generalConfiguration.InstanceTag, "http", generalConfiguration.InstanceTag + 4, @"MessageBroker\Sheduler");

            DeleteSiteFromIIS("CoreMessageBroker" + generalConfiguration.InstanceTag);
            AddSiteToIIS("CoreMessageBroker" + generalConfiguration.InstanceTag, "http", generalConfiguration.InstanceTag + 5, @"MessageBroker\WebApiService");

            DeleteSiteFromIIS("SignService" + generalConfiguration.InstanceTag);
            AddSiteToIIS("SignService" + generalConfiguration.InstanceTag, "https", generalConfiguration.InstanceTag + 6, "SignService");

            DeleteSiteFromIIS("Storage" + generalConfiguration.InstanceTag);
            AddSiteToIIS("Storage" + generalConfiguration.InstanceTag, "http", generalConfiguration.InstanceTag + 7, "Storage");
        }


        static void Main(string[] args)
        {
            if (File.Exists(configPath))
            {
                try
                {
                    Console.Write($"Чтение Config.json... ");
                    string data = File.ReadAllText(configPath);
                    generalConfiguration = JsonSerializer.Deserialize<GeneralConfiguration>(data);
                    Console.WriteLine($"OK");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Произошла ошибка при чтении Config.json: {ex.Message}");
                    Console.ResetColor();
                }
                Console.WriteLine();
                /*ClearOldFilesIfExists();
                CreateFolders();
                ExtractArhives();*/
                CreateDatabasesIfNotExist();
                //FillConfigs();
                //ConfigureIIS();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Не удалось найти Config.json в папке проекта");
                Console.ResetColor();
            }
        }
    }
}
