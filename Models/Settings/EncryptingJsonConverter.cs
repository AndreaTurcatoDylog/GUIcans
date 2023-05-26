using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EncryptingJsonConverter : JsonConverter
    {
        #region Methods

        public EncryptingJsonConverter(string encryptionKey)
        {
            if (encryptionKey == null)
            {
                throw new ArgumentNullException(nameof(encryptionKey));
            }
        }

        public EncryptingJsonConverter(byte[] encryptionKey)
        {
            if (encryptionKey == null)
            {
                throw new ArgumentNullException(nameof(encryptionKey));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
            {
                writer.WriteNull();
                return;
            }

            //if (!AESCryptography._isDirty)
            //{
            //    AESCryptography._isDirty = true;

            //    var length = AESCryptography.Information.Length;
            //    var first = AESCryptography.Information.Take(length / 2).ToArray();
            //    var second = AESCryptography.Information.Skip(length / 2).ToArray();

            //    var inf = second.Concat(first).ToArray();

            //    AESCryptography.Information = inf;
            //}

            var encryptedValue = AESCryptography.EncryptStringToBytes_Aes(stringValue, AESCryptography.Information, AESCryptography.IV);
            writer.WriteValue(encryptedValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader?.Value?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Convert.FromBase64String(value);

                try
                {
                    var stringValue = AESCryptography.DecryptStringFromBytes_Aes(bytes, AESCryptography.Information, AESCryptography.IV);
                    return stringValue;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //{
        //    var pippo = reader?.Value?.ToString();
        //    return reader?.Value?.ToString();
        //}

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        #endregion
    }
}
