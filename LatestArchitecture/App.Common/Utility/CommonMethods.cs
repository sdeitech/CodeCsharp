using App.Common.Constant;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static App.Common.Enums.CommonEnums;

namespace App.Common.Utility
{
    public static class CommonMethods
    {



        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12)); // Salt factor 12
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }


        public static DateTime TruncateToMinute(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes?.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static string? GetClientIp(HttpContext? context)
        {
            if (context == null) return null;

            var ipFromHeader = context.Request.Headers["X-Client-IP"].FirstOrDefault();
            return !string.IsNullOrEmpty(ipFromHeader)
                ? ipFromHeader
                : context.Connection.RemoteIpAddress?.ToString();
        }

        public static string GenerateOtp(int length = 6)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => random.Next(0, 10).ToString()[0])
                .ToArray());
        }

        /// <summary>
        /// A dictionary that maps file extensions to their corresponding MIME types.
        /// </summary>
        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        #region list of mime types
        // combination of values from Windows 7 Registry and 
        // from C:\Windows\System32\inetsrv\config\applicationHost.config
        // some added, including .7z and .dat
        {".aaf", "application/octet-stream"},
        {".AddIn", "text/xml"},
        {".adobebridge", "application/x-bridge-url"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aiff", "audio/aiff"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".bmp", "image/bmp"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".doc", "application/msword"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".exe", "application/octet-stream"},
        {".gif", "image/gif"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".java", "application/octet-stream"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mpeg", "video/mpeg"},
        {".pdf", "application/pdf"},
        {".png", "image/png"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".psd", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".rar", "application/octet-stream"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".txt", "text/plain"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xml", "text/xml"},
        {".zip", "application/x-zip-compressed"},
        #endregion

        };

        /// <summary>
        /// Gets the MIME type for a given file extension.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetExtenstion(string mimeType)
        {
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimetype");
            }
            return _mappings.FirstOrDefault(x => x.Value == mimeType).Key;
        }

        /// <summary>
        /// Saves the image to the specified path with the given dimensions.
        /// </summary>
        /// <param name="imageModel"></param>
        public static void SaveImages(dynamic imageModel)
        {
            int height = 32; int width = 180;  //default logo
            if (imageModel.Type == ImagesFolder.Favicon.ToString()) {/*Favicon size*/ height = 16; width = 16; }
            MemoryStream ms = new MemoryStream(imageModel.Bytes);
            using (var image = Image.FromStream(ms))
            {
                var newWidth = width;
                var newHeight = height;
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                string newPath = Path.GetFullPath(imageModel.Url);
                thumbnailImg.Save(newPath, image.RawFormat);
            }

        }

        /// <summary>
        /// Creates an image URL based on the request context and the specified directory and file name.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="directoy"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateImageUrl(HttpContext request, string directoy, string fileName)
        {
            try
            {
                string webRootPath = Directory.GetCurrentDirectory();
                if (File.Exists(Path.Combine(webRootPath, directoy.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), fileName)))
                {
                    return request.Request.Scheme + "://" + request.Request.Host + request.Request.PathBase + directoy + fileName;
                }
                else
                {
                    return string.Empty;
                    //return request.Request.Scheme + "://" + request.Request.Host + request.Request.PathBase + "/Images/DefaultPhotos/" + "default-no-image.JPG";
                }
            }
            catch (Exception)
            { return string.Empty; }
        }

        public static string DecryptUserPassword(string cipherText)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionConfig.Key);
                    aes.IV = Encoding.UTF8.GetBytes(EncryptionConfig.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T ConvertDateTimesToTimeZone<T>(T obj, TimeZoneInfo timeZoneInfo)
        {
            if (obj == null) return obj;

            void ConvertProperties(object item)
            {
                if (item == null) return;

                var properties = item.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item);
                    if (value == null) continue;

                    var propType = prop.PropertyType;

                    if (propType == typeof(DateTime))
                    {
                        prop.SetValue(item, TimeZoneInfo.ConvertTimeFromUtc((DateTime)value, timeZoneInfo));
                    }
                    else if (propType == typeof(DateTime?))
                    {
                        var dt = (DateTime?)value;
                        if (dt.HasValue)
                            prop.SetValue(item, (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(dt.Value, timeZoneInfo));
                    }
                    else if (propType == typeof(DateTimeOffset))
                    {
                        prop.SetValue(item, TimeZoneInfo.ConvertTime((DateTimeOffset)value, timeZoneInfo));
                    }
                    else if (propType == typeof(DateTimeOffset?))
                    {
                        var dto = (DateTimeOffset?)value;
                        if (dto.HasValue)
                            prop.SetValue(item, (DateTimeOffset?)TimeZoneInfo.ConvertTime(dto.Value, timeZoneInfo));
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(propType) && propType != typeof(string))
                    {
                        foreach (var child in (IEnumerable)value)
                            ConvertProperties(child);
                    }
                    else if (!propType.IsPrimitive && !propType.IsEnum && !propType.IsValueType && propType != typeof(string))
                    {
                        ConvertProperties(value);
                    }
                }
            }

            if (obj is IEnumerable enumerable && !(obj is string))
            {
                foreach (var item in enumerable)
                    ConvertProperties(item);
            }
            else
            {
                ConvertProperties(obj);
            }

            return obj;
        }



    }
}
