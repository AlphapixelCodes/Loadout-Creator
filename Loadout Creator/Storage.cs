using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Loadout_Creator
{
    public static class Storage
    {
        public static StorageFolder LocalStorage = ApplicationData.Current.LocalFolder;
        internal static StorageFolder LocalCache = ApplicationData.Current.LocalCacheFolder;
        internal static StorageFolder TempFolder = ApplicationData.Current.TemporaryFolder;
        internal async static Task SaveSettingsFile(string fileName, string data)
        {
            try
            {
                var file = await LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Storage.SaveSettingsFile: Failed to save file: " + ex.Message);
            }
        }



        public async static Task<StorageFile> GetFile(string FileName, StorageFolder Folder = null)
        {
            if(Folder==null)
                return await LocalStorage.GetFileAsync(FileName);
            return await Folder.GetFileAsync(FileName);
        }

        public async static Task<bool> FileExists(string Filename, StorageFolder Folder=null)
        {
            try
            {
                if (Folder == null)
                    await LocalStorage.GetFileAsync(Filename);
                else
                    await Folder.GetFileAsync(Filename);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async static Task<bool> FolderExists(string FolderName, StorageFolder Folder = null)
        {
            try
            {
                if (Folder == null)
                    await LocalStorage.GetFolderAsync(FolderName);
                else
                    await Folder.GetFolderAsync(FolderName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async static Task<StorageFolder> GetFolder(string FolderName,StorageFolder Folder=null)
        {
            if (Folder == null)
                return await LocalStorage.GetFolderAsync(FolderName);
            else
                return await Folder.GetFolderAsync(FolderName);
        }
        public async static Task<bool> DeleteFile(string FileName, StorageFolder Folder = null)
        {
            if(await FileExists(FileName, Folder))
            {
                await (await GetFile(FileName,Folder))?.DeleteAsync();
                return true;
            }
            return false;
        }
        public async static Task<bool> DeleteFolder(string FolderName, StorageFolder Folder = null)
        {
            if (await FolderExists(FolderName, Folder))
            {
                await (await GetFolder(FolderName, Folder))?.DeleteAsync();
                return true;
            }
            return false;
        }
    }

    public class File
    {
        public string StartNameNoExtension { get; set; }
        public string DateModified { get; set; }
        public StorageFile ActualFile { get; set; }
        public static async Task<File> Get(StorageFile sf)
        {
            var r = new File();
            r.ActualFile = sf;
            r.StartNameNoExtension = Path.GetFileNameWithoutExtension(sf.Name);
            r.DateModified = (await sf.GetBasicPropertiesAsync()).DateModified.ToString().Split(" ")[0].ToString();
            return r;
        }
        public override string ToString()
        {
            return StartNameNoExtension;
        }
    }
}
