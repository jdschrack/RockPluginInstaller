using Rock;
using Rock.Data;
using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace TestRockInstall
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Properties.Settings.Default.PackagePath;

            ProcessInstall(path);

            Console.ReadKey();
        }

        static void ProcessInstall(string packagePath)
        {
            var appRoot = Properties.Settings.Default.RockPath;

            try
            {
                using (var packageZip = ZipFile.OpenRead(packagePath))
                {                    
                    foreach (var entry in packageZip.Entries.Where(x => x.FullName.StartsWith("content/", StringComparison.OrdinalIgnoreCase)))
                    {
                        string fullPath = Path.Combine(appRoot, entry.FullName.Replace("content/", "")).Replace("/", "\\");
                        var directory = Path.GetDirectoryName(fullPath).Replace("content/", "");
                        Console.WriteLine(directory);
                        if (entry.Length != 0)
                        {
                            createDirectory(directory); 
                            entry.ExtractToFile(fullPath, true);
                        }                        
                    }

                    processSql(packageZip);

                    try
                    {
                        var deleteListEntry = packageZip.Entries.Where(e => e.FullName == "install/deletefile.lst").FirstOrDefault();
                        if (deleteListEntry != null)
                        {

                            string deleteList = System.Text.Encoding.Default.GetString(deleteListEntry.Open().ReadBytesToEnd());

                            string[] itemsToDelete = deleteList.Replace("\r\n", "\n").Split(new string[] { "\n" }, StringSplitOptions.None);

                            foreach (string deleteItem in itemsToDelete)
                            {
                                if (!string.IsNullOrWhiteSpace(deleteItem))
                                {
                                    string deleteItemFullPath = appRoot + deleteItem;
                                    Console.WriteLine($"Deleting {deleteItemFullPath}");

                                    if (Directory.Exists(deleteItemFullPath))
                                    {
                                        Directory.Delete(deleteItemFullPath, true);
                                    }

                                    if (File.Exists(deleteItemFullPath))
                                    {
                                        File.Delete(deleteItemFullPath);
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Modifying Files: An error occurred while modifying files. \nError: {ex.Message}\nSource: {ex.Source}\nTrace: {ex.StackTrace}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void processDeleteFile(ZipArchiveEntry deleteListEntry)
        {

        }

        private static void processSql(ZipArchive packageZip)
        {
            try
            {
                var sqlInstallEntry = packageZip.Entries.Where(e => e.FullName == "install/run.sql").FirstOrDefault();
                if (sqlInstallEntry != null)
                {
                    string sqlScript = System.Text.Encoding.Default.GetString(sqlInstallEntry.Open().ReadBytesToEnd());

                    if (!string.IsNullOrWhiteSpace(sqlScript))
                    {
                        using (var context = new RockContext())
                        {
                            context.Database.Log = s => Console.WriteLine(s);
                            context.Database.ExecuteSqlCommand(sqlScript);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Updating Database. An error occurred while updating the database. \nError: {ex.Message}");
                return;
            }
        }

        private static void createDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string buildRockShopDir(string appRoot)
        {
            var baseDirectory = Path.Combine(appRoot, "RockShop");
            createDirectory(baseDirectory);

            return baseDirectory;
        }

        static string baseDirectoryPath()
        {
            var baseDirectory = @"c:\files";
            createDirectory(baseDirectory);

            return baseDirectory;
        }


    }
}
