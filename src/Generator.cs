using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HazelProject.NFTGenerator
{
    public class Generator
    {
        public Random RandomGenerator;
        public IConfiguration Config;
        private List<Trait> traitList;
        private string baseDirectory;
        private string collectionName;
        public void Generate(string collectionName)
        {
            RandomGenerator = new Random();
            this.collectionName = collectionName;
            var jsonPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), Constants.Configuration.ConfigFile);
            if(!File.Exists(jsonPath))
                throw new Exception($"Cannot find {jsonPath}");

            try
            {
                Config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();
            }
            catch (Exception e)
            {
                throw new Exception("Invalid contents of config.json");
            }

            baseDirectory = Config.GetSection(Constants.Configuration.BaseDirectoryKey).Get<string>();
            if (!Directory.Exists(baseDirectory))
                throw new Exception($"Cannot find {baseDirectory}");

            try
            {
                traitList = Config.GetSection(Constants.Configuration.TraitsKey).Get<List<Trait>>().OrderBy(x => x.Id).ToList();
            }
            catch(Exception e)
            {
                throw new Exception("Invalid trait list");
            }

            int nftsToCreate;
            try
            {
                nftsToCreate = Config.GetSection(Constants.Configuration.NftsToCreateKey).Get<int>();
            }
            catch (Exception e)
            {
                throw new Exception("Invalid amount of nfts to create");
            }

            if(!Directory.Exists(System.IO.Path.Combine(baseDirectory, Constants.Configuration.AssetsPath)))
                throw new Exception("Invalid Assets path"); 

            var traitImageList = GenerateImageList(traitList, System.IO.Path.Combine(baseDirectory, Constants.Configuration.AssetsPath));

            var traitSetsToCreate = new List<Set>();

            if (nftsToCreate > TotalCombinatons(traitList))
                throw new Exception("Not enough total combinations! Try a lower number of nfts to create");
            else
                traitSetsToCreate = GenerateTraitSets(traitList, nftsToCreate);

            if(!Directory.Exists(System.IO.Path.Combine(baseDirectory, Constants.Configuration.OutputPath)))
                Directory.CreateDirectory(Path.Combine(baseDirectory, Constants.Configuration.OutputPath));

            var collectionPath = System.IO.Path.Combine(baseDirectory, Constants.Configuration.OutputPath, collectionName);
            if (Directory.Exists(collectionPath))
                throw new Exception("Collection name already used!");
           
            Directory.CreateDirectory(collectionPath);
            Directory.CreateDirectory(Path.Combine(collectionPath, Constants.Configuration.ImagesPath));
            GenerateImages(traitSetsToCreate, traitImageList, collectionPath);
        }
        
        private List<TraitImages> GenerateImageList(List<Trait> traitList, string baseDirectory)
        {
            var traitImageList = new List<TraitImages>();

            foreach (var trait in traitList)
            {
                var traitPath = System.IO.Path.Combine(baseDirectory, trait.Directory);
                var traitImages = new TraitImages { TraitName = trait.Name, Images = new List<TraitImage>() };
                var imageFormat = Config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>();

                foreach (var imagePath in Directory.GetFiles(traitPath, $"*{imageFormat}"))
                    traitImages.Images.Add(new TraitImage{ FileName = imagePath, Image = Image.Load(imagePath)});

                trait.NumOfElements = traitImages.Images.Count();

                if (trait.RarityWeights != null && trait.NumOfElements != trait.RarityWeights.Length)
                    throw new Exception($"Invalid rarity weight array for trait {trait.Name}");     

                traitImageList.Add(traitImages);
            }

            return traitImageList;
        }


#nullable enable
        public int GetRandomTraitIndex(int numberOfElements, int[]? traitArray = null)
        {
            if (traitArray == null)
                return RandomGenerator.Next(0, numberOfElements);

            var weightedIndexList = new List<int>();

            for (var i = 0; i < traitArray.Length; i++)
                for (var j = 0; j < traitArray[i]; j++)
                    weightedIndexList.Add(i);

            return weightedIndexList.Shuffle()[RandomGenerator.Next(0, weightedIndexList.Count)];
        }
#nullable disable    

        private List<Set> GenerateTraitSets(List<Trait> traitList, int nftsToCreate)
        {
            var consoleCursorTop = Console.CursorTop;
            List<Set> traitSetsToCreate = new List<Set>();
            Console.WriteLine("Generating all possible combinations!");
            while (traitSetsToCreate.Count < nftsToCreate)
            {
                var traitSetDictionary = new Dictionary<string, int>();
                foreach (var trait in traitList)
                {
                    if (trait.PresenceChance != null)
                        if (RandomGenerator.Next(0, 100) > trait.PresenceChance)
                            continue;

                    var randomIndex = GetRandomTraitIndex(trait.NumOfElements, trait.RarityWeights);
                    traitSetDictionary.Add(trait.Name, randomIndex);
                }

                var set = new Set(traitSetDictionary, traitSetsToCreate.Count);
                if (!traitSetsToCreate.Any(x => x.Equals(set)))
                    traitSetsToCreate.Add(set);

                var progress = $"Trait Set Generation Progress: {traitSetsToCreate.Count}/{nftsToCreate}";
                Console.SetCursorPosition(0, consoleCursorTop);
                Console.Write($"{progress}{new string(' ', Console.WindowWidth - progress.Length)}");
            }

            return traitSetsToCreate;
        }

        private void GenerateImages(List<Set> traitSetsToCreate, List<TraitImages> traitImageList, string outputPath)
        {
            var imageWidth = Config.GetSection(Constants.Configuration.ImageWidthKey).Get<int>();
            var imageHeight = Config.GetSection(Constants.Configuration.ImageHeightKey).Get<int>();
            var counter = 0;
            var dateTime = DateTime.Now;
            var metaDataTemplate = new Dictionary<string, string>
            {
                {Constants.Configuration.ImageNameKey, string.Empty }
            };

            foreach (var trait in traitList)
                metaDataTemplate.Add(trait.Name, string.Empty);

            var metaData = new ConcurrentBag<MetadataRecord>();

            var currentCursorLine = Console.CursorTop;
            var consoleSemaphore = new object();
            Parallel.ForEach(traitSetsToCreate, nft =>
            {
                counter++;
                var canvas = new Image<Rgba32>(imageWidth, imageHeight);
                var metaDataEntry = new Dictionary<string, string>();
                metaDataEntry = (Dictionary<string,string>)metaDataTemplate.DeepClone();

                canvas.Mutate(ImageContext => {

                    foreach (var layerInfo in nft.SelectedTraits)
                    {
                        var layerImage = traitImageList.First(x => x.TraitName == layerInfo.Key).Images[layerInfo.Value];
                        ImageContext.DrawImage(layerImage.Image, new Point(0, 0), 1);
                        metaDataEntry[layerInfo.Key] = layerImage.GetStrippedFileName();
                    }
                });

                canvas.Save(System.IO.Path.Combine(outputPath,Constants.Configuration.ImagesPath, 
                    $"{nft.Id}{Config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>()}"));

                metaDataEntry[Constants.Configuration.ImageNameKey] = $"{nft.Id}";
                metaData.Add(new MetadataRecord { Id = nft.Id, Data = metaDataEntry });

                if (counter % Constants.Configuration.Precentage == 0)
                {
                    lock (consoleSemaphore)
                    {
                        var progress = $"Image Generation Progress: {counter}/{traitSetsToCreate.Count}";
                        Console.SetCursorPosition(0, currentCursorLine);
                        Console.Write($"{progress}{new string(' ', Console.WindowWidth - progress.Length)}");
                    }
                }
            });

            Console.WriteLine();
            Extensions.WriteColoredLine("!!!DO NOT CLOSE THIS CONSOLE!!!", ConsoleColor.Black, ConsoleColor.Red);
            Extensions.WriteColoredLine($"Finished Generating Images in {(DateTime.Now - dateTime).TotalMinutes:##.##} minutes.", ConsoleColor.Black, ConsoleColor.Green);
            Console.Write("If you want to curate the generated images, you can simply delete the unwanted images from ");
            Extensions.WriteColored($"{Path.Combine(outputPath, Constants.Configuration.ImagesPath)}", ConsoleColor.Black, ConsoleColor.Yellow);
            Console.WriteLine(" and continue the process in this console.");
            
            var orderedMetaData = metaData.ToList().OrderBy(x => x.Id);

            File.WriteAllText(
                System.IO.Path.Combine(outputPath, $"{Constants.Configuration.MetadataFile}{Constants.Configuration.MetadataFileExtension}"),
                JsonSerializer.Serialize(orderedMetaData));
        }

        public void PrintUploadInstructions(){
            var collectionPath = System.IO.Path.Combine(baseDirectory, Constants.Configuration.OutputPath, collectionName);
            Console.Write($"Upload ");
            Extensions.WriteColored($"{Path.Combine(collectionPath, Constants.Configuration.ImagesPath)}", ConsoleColor.Black, ConsoleColor.Yellow);
            Console.WriteLine(" to IPFS through Pinata Gateway.");
            Console.WriteLine("When the upload is complete, copy the CID from Pinata and paste it this console when prompted.");
        }

        public int TotalCombinatons(List<Trait> traitList)
        {
            return traitList.Select(x => x.NumOfElements).Aggregate((product, element) => product * element);
        }

        public void CurateMetadata()
        {
            var metadata =  ReadMetadata();
            var tempMetadata = new List<MetadataRecord>();
            var curatedMetadata = new List<MetadataRecord>();
            var imagesPath = Path.Combine(baseDirectory, Constants.Configuration.OutputPath, collectionName, Constants.Configuration.ImagesPath);
            var imageExtension = Config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>();
            
            foreach(var record in metadata){
                if(File.Exists(Path.Combine(imagesPath, $"{record.Data[Constants.Configuration.ImageNameKey]}{imageExtension}")))
                    tempMetadata.Add(record);
            }

            var counter = 0;
            var images = Directory.GetFiles(imagesPath, $"*{Config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>()}")
                    .OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x)));

            foreach(var image in images){
                var imageInfo = new FileInfo(image);
                imageInfo.MoveTo(Path.Combine(imagesPath, $"{counter}{imageExtension}"));
                var imageId = int.Parse(Path.GetFileNameWithoutExtension(image));
                var metadataRecord = tempMetadata.Find(x => x.Id == imageId);
                metadataRecord.Id = counter;
                metadataRecord.Data[Constants.Configuration.ImageNameKey] = counter.ToString();
                curatedMetadata.Add(metadataRecord);
                counter++;
            }

            File.WriteAllText(
                System.IO.Path.Combine(
                baseDirectory,
                Constants.Configuration.OutputPath,
                collectionName,
                $"{Constants.Configuration.MetadataFile}{Constants.Configuration.MetadataFileExtension}"),
                JsonSerializer.Serialize(curatedMetadata));    

        }
        
        public void GenerateJsonFiles(string ipfsCid)
        {
            var metadata =  ReadMetadata();
            var metadataDirectory = System.IO.Path.Combine(baseDirectory, Constants.Configuration.OutputPath, collectionName, Constants.Configuration.MetadataPath);

            if (!Directory.Exists(metadataDirectory))
                Directory.CreateDirectory(metadataDirectory);

            foreach (var record in metadata)
            {
                var openseaMetadata = new OpenseaMetadata
                {
                    Name = Config.GetSection(Constants.Configuration.NftNameKey).Get<string>().Replace("#", $"#{record.Id}"),
                    Description = Config.GetSection(Constants.Configuration.DescriptionKey).Get<string>(),
                    Image = $"ipfs://{ipfsCid}/{record.Data[Constants.Configuration.ImageNameKey]}{Config.GetSection(Constants.Configuration.ImageFormatKey).Get<string>()}",
                    Attributes = record.Data.Where(x => x.Key != Constants.Configuration.ImageNameKey && x.Value != string.Empty)
                    .Select(x => new OpenseaAttributeDto { trait_type = x.Key, value = x.Value }).ToList()
                };
                File.WriteAllText(System.IO.Path.Combine(metadataDirectory, $"{record.Id}{Constants.Configuration.MetadataFileExtension}"), JsonSerializer.Serialize(openseaMetadata));
            }

            Extensions.WriteColoredLine("Json files generated successfully", ConsoleColor.Black, ConsoleColor.Green);
            Console.Write($"Don't forget to upload ");
            Extensions.WriteColored($"{metadataDirectory}", ConsoleColor.Black, ConsoleColor.Yellow);
            Console.WriteLine(" to IPFS through Pinata Gateway!");
        }

        public List<MetadataRecord> ReadMetadata(){
            var metadataJson = File.ReadAllText(System.IO.Path.Combine(
                baseDirectory,
                Constants.Configuration.OutputPath,
                collectionName,
                $"{Constants.Configuration.MetadataFile}{Constants.Configuration.MetadataFileExtension}"));

            return JsonSerializer.Deserialize<List<MetadataRecord>>(metadataJson);
        }

    }
}