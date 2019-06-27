using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace DiveLog.API.Helpers
{
    public class HashGenerator
    {
        public static string GenerateKey(object sourceObject)
        {
            string hashString;

            //Catch unuseful parameter values
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Null as parameter is not allowed");
            }
            else
            {
                //We determine if the passed object is really serializable.
                try
                {
                    //Now we begin to do the real work.
                    hashString = ComputeHash(ObjectToByteArray(sourceObject));
                    return hashString;
                }
                catch (AmbiguousMatchException ame)
                {
                    throw new ApplicationException("Could not definitely decide if object is serializable.Message:" + ame.Message);
                }
            }
        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch 
            {
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

        private static readonly Object locker = new Object();

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                lock (locker)
                {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occurred during serialization. Message: " + se.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}