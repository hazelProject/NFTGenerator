using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public class TraitImage
    {
        public static string FileFormat;
        public string FileName { get; set; }
        public Image Image { get; set; }
        public TraitImage() { }

        public TraitImage(string fileName, Image image)
        {
            FileName = fileName;
            Image = image;
        }

        public string GetStrippedFileName()
        {
            if(TraitImage.FileFormat == null)
            {
                var jsonPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Constants.Configuration.ConfigFile);
                var config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
                TraitImage.FileFormat = config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>();
            }
            
            return System.IO.Path.GetFileName(this.FileName).Replace(FileFormat, string.Empty);
        }
    }
}
