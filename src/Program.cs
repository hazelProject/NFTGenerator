using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Drawing;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HazelProject.NFTGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new Generator();
            Console.Write("Insert Collection Name: ");
            try{
                generator.Generate(Console.ReadLine());
            }catch(Exception e){
                Extensions.WriteError(e.Message);
                throw;
            }
            
            Console.Write("Did you delete any unwanted images? (y/n): ");
            var didDeleteImages = string.Empty;
            
            while(!new [] { "y", "n" }.Contains(didDeleteImages))
                didDeleteImages = Console.ReadLine();

            if(didDeleteImages == "y"){
                try{
                    generator.CurateMetadata();
                }catch(Exception e){
                    Extensions.WriteError(e.Message);
                    throw;
                }
            }
            generator.PrintUploadInstructions();
            Console.Write("Insert IPFS CID: ");
            try{
                generator.GenerateJsonFiles(Console.ReadLine());
            }catch(Exception e){
                Extensions.WriteError(e.Message);
                throw;
            }   
        }
    }
}
