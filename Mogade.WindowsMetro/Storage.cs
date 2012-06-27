using System;
using System.Collections.Generic;
using Windows.Storage;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using Microsoft.Phone.Info;

namespace Mogade.WindowsMetro
{
   public class Storage : IStorage
   {
      private const string _mogadeDataFile = "mogade.dat";
      private const string _userNamesDataFile = "usernames.dat";

      private Configuration _configuration;
      private IList<string> _userNames;

      public Storage()
      {
          Initialise();
      }

       public async void Initialise()
       {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            
            StorageFile file  = await storageFolder.CreateFileAsync(_userNamesDataFile, CreationCollisionOption.OpenIfExists);
            _userNames = await FileIO.ReadLinesAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                        //StorageFile file2  = await storageFolder.CreateFileAsync(_userNamesDataFile, CreationCollisionOption.OpenIfExists);
            _configuration = await Read<Configuration>(_mogadeDataFile);
            
           if (_configuration == null)
            {
                _configuration = new Configuration { UniqueIdentifier = Guid.NewGuid().ToString() };
                WriteToFile(_configuration, _mogadeDataFile);
            }
        }

      public string GetUniqueIdentifier()
      {
         //if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.DeviceId)
         //{
         //   object raw;  
         //   if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out raw) && raw != null)
         //   {
         //      var bytes = (byte[]) raw;
         //      var sb = new StringBuilder(bytes.Length * 2);
         //      for(var i = 0; i < bytes.Length; ++i)
         //      {
         //         sb.Append(bytes[i].ToString("X2"));
         //      }
         //      return sb.ToString();
         //   }
         //}
         //else if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.LegacyUserId)
         //{
         //   object anid;
         //   if (UserExtendedProperties.TryGetValue("ANID", out anid) && anid != null)
         //   {
         //      return anid.ToString();
         //   }
         //}
         //else if (MogadeConfiguration.Data.UniqueIdStrategy == UniqueIdStrategy.UserId)
         //{
         //   object anid;
         //   if (UserExtendedProperties.TryGetValue("ANID", out anid) && anid != null)
         //   {
         //      return anid.ToString().Substring(2, 32);
         //   } 
         //}
          if(_configuration == null)
          {
              _configuration = new Configuration { UniqueIdentifier = Guid.NewGuid().ToString() };
              WriteToFile(_configuration, _mogadeDataFile);
          }

         return _configuration.UniqueIdentifier;
      }

      public ICollection<string> GetUserNames()
      {
         return _userNames;
      }

      public void SaveUserName(string userName)
      {
         if (string.IsNullOrEmpty(userName)) { return; }
         if ( _userNames.Contains(userName)) { return; }
         _userNames.Add(userName);
         WriteToFile(_userNames, _userNamesDataFile);
      }

      public void RemoveUserName(string userName)
      {
         if (string.IsNullOrEmpty(userName) || !_userNames.Remove(userName)) { return; }
         WriteToFile(_userNames, _userNamesDataFile);
      }
      private static async Task<T> Read<T>(string dataFile)
      {
          StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

          StorageFile file  = await storageFolder.CreateFileAsync(dataFile, CreationCollisionOption.OpenIfExists);
          string data = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
          return JsonConvert.DeserializeObject<T>(data);
          

         //using (var store = Windows.Storage..GetUserStoreForApplication())
         //using (var stream = new IsolatedStorageFileStream(dataFile, System.IO.FileMode.OpenOrCreate, store))
         //{
         //   if (stream.Length <= 0) { return default(T); }
         //   var buffer = new byte[stream.Length];
         //   stream.Read(buffer, 0, buffer.Length);
         //   return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
         //}        
      }
      private static async void WriteToFile(object objectToWrite, string dataFile)
      {
          StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

          StorageFile file = await storageFolder.CreateFileAsync(dataFile, CreationCollisionOption.OpenIfExists);
          await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(objectToWrite), Windows.Storage.Streams.UnicodeEncoding.Utf8);

         //using (var store = IsolatedStorageFile.GetUserStoreForApplication())
         //using (var stream = new IsolatedStorageFileStream(dataFile, System.IO.FileMode.Create, store))
         //{
         //   var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToWrite));
         //   stream.Write(buffer, 0, buffer.Length);
         //}
      }
   }
}