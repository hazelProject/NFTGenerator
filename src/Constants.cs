using System;
using System.Collections.Generic;
using System.Text;

namespace HazelProject.NFTGenerator
{
    public static class Constants
    {
        public static class Configuration
        {
            public const string ConfigFile = "config.json";
            public const string AssetsPath = "assets";
            public const string OutputPath = "output";
            public const string ImagesPath = "images";
            public const string MetadataPath = "metadata";
            public const string MetadataFile = "metadata";
            public const string MetadataFileExtension = ".json";
            
            public const string NftsToCreateKey = "NftsToCreate";
            public const string TraitsKey = "Traits";
            public const string BaseDirectoryKey = "BaseDirectory";
            public const string ImageFormatKey = "ImageFormat";
            public const string ImageWidthKey = "ImageWidth";
            public const string ImageHeightKey = "ImageHeight";
            public const string NftNameKey = "NftName";
            public const string DescriptionKey = "Description";
            public const string ImageNameKey = "imageName";

            public const int Precentage = 1;
        }
    }
}
