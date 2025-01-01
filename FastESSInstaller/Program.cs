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

namespace FastESSInstaller
{
    class Program
    {
        static string instanceTag = "243";
        static string pathToArhievedServises = @"\\irnpo\public\Ess\Platform\2.4\2.4.3\Components";
        static string pathToTools = pathToArhievedServises.Replace(@"\Components", @"\Tools");
        static string EssServicesFolder = @"D:\TestEss";
        static List<string> serviceFolders = new List<string> {
            "DocumentService",
            "EssCLI",
            "EssService",
            "EssSite",
            "FileStorage",
            "IdCLI",
            "IdentityService",
            "MessageBroker/Sheduler",
            "MessageBroker/WebApiService",
            "SignService",
            "StorageService"
        };
        
        static void ClearOldFilesIfExists()
        {
            try
            {
                Console.Write("Удаление старых папок и файлов... ");
                foreach (var folder in serviceFolders)
                {
                    DirectoryInfo serviceFolder = new DirectoryInfo(Path.Combine(EssServicesFolder, folder));
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
                Console.WriteLine("Успешно \n");
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
                foreach (var folder in serviceFolders)
                    Directory.CreateDirectory(Path.Combine(EssServicesFolder, folder));
                Console.WriteLine("Успешно \n");
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
                Console.Write($"Извлечение архива {zipFilePath.Substring(zipFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)} ...");
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
                Console.WriteLine("Успешно");
            }
            catch(Exception ex)
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
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"MessageBroker\WebApiService-win-x64.zip"),
                    Path.Combine(EssServicesFolder, @"MessageBroker/WebApiService"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"MessageBroker\Scheduler-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"MessageBroker\SmscTransport-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"MessageBroker\MobilGroupTransport-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"MessageBroker\Sheduler"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"MessageBroker\MtsMarketologTransport-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"MessageBroker\Sheduler"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"IdentityService\IdentityService-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"IdentityService"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"IdentityService\EssPlatformIdentityProvider2-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"IdentityService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"StorageService\StorageApi.Service-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"StorageService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"DocumentService\WebApiService-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"DocumentService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"SignService\Directum.Core.SignService.App-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"SignService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"Ess\OfficeService-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"EssService"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToArhievedServises, @"Ess\Site-win-x64.zip"), 
                    Path.Combine(EssServicesFolder, @"EssSite"), false);

                CheckIfExistAndExtractFiles(Path.Combine(pathToTools, @"Ess-Cli-win-x64.zip"),
                    Path.Combine(EssServicesFolder, @"EssCLI"), false);
                CheckIfExistAndExtractFiles(Path.Combine(pathToTools, @"Id-Cli-win-x64.zip"),
                    Path.Combine(EssServicesFolder, @"IdCLI"), false);

                Console.WriteLine("Успешно \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при распаковке архивов: {ex.Message}");
            }
        }

        static void AddSiteToIIS(string siteName, string protocol, string binding, string path)
        {
            try
            {
                ServerManager iisManager = new ServerManager();
                Site site = iisManager.Sites.Add(siteName, protocol, $"*:{binding}:", EssServicesFolder + @"\" + path);
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
                    //check site exists or not
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
            DeleteSiteFromIIS("DocumentService" + instanceTag);
            AddSiteToIIS("DocumentService" + instanceTag, "http", instanceTag + 0, "DocumentService");

            DeleteSiteFromIIS("EssService" + instanceTag);
            AddSiteToIIS("EssService" + instanceTag, "https", instanceTag + 1, "EssService");

            DeleteSiteFromIIS("EssSite" + instanceTag);
            AddSiteToIIS("EssSite" + instanceTag, "https", instanceTag + 2, "EssSite");
            string webConfigPath = EssServicesFolder + @"\EssSite\web.config"; // Укажите путь к вашему web.config
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
                    AddRedirectRule(xmlDoc, rulesNode, "storage", "^storage/(.*)", $"http://localhost:{instanceTag + 2}/" + "{R:1}");

                    // Создание второго правила перенаправления
                    AddRedirectRule(xmlDoc, rulesNode, "api", "^api/(.*)", $"http://localhost:{instanceTag + 1}/api/" + "{R:1}");
                }
                xmlDoc.Save(webConfigPath);

                Console.WriteLine($"Настройки перенаправления для сайта {"EssSite" + instanceTag} успешно сохранены в web.config.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.ResetColor();
            }

            DeleteSiteFromIIS("Storage" + instanceTag);
            AddSiteToIIS("Storage" + instanceTag, "http", instanceTag + 3, "Storage");

            DeleteSiteFromIIS("IdentityService" + instanceTag);
            AddSiteToIIS("IdentityService" + instanceTag, "https", instanceTag + 4, "IdentityService");

            DeleteSiteFromIIS("Sheduler" + instanceTag);
            AddSiteToIIS("Sheduler" + instanceTag, "http", instanceTag + 5, @"MessageBroker\Sheduler");

            DeleteSiteFromIIS("CoreMessageBroker" + instanceTag);
            AddSiteToIIS("CoreMessageBroker" + instanceTag, "http", instanceTag + 6, @"MessageBroker\WebApiService");
        }
        
        static void Main(string[] args)
        {
            /*ClearOldFilesIfExists();
            CreateFolders();
            ExtractArhives();*/
            ConfigureIIS();
        }
    }
}
