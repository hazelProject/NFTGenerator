# NFTGenerator

## Overview

This is a small dotnet console application for NFT assets generation. 
The output of the program consists of both images and their metadata.
The metadata is compliant with the OpenSea standard defined at https://docs.opensea.io/docs/metadata-standards.
If you would like to read a more comprehensive article about NFTs generation, development, deployment and minting make sure to check out our's at https://medium.com/@HazelProject/so-you-want-to-make-an-nft-5b6d07838647

## Getting started

Make sure to have dotnet installed. You can verify by running `dotnet --version` command in Window CMD or macOS Terminal. If an error is displayed instead of a version number, you can get dotnet SDK from https://dotnet.microsoft.com/en-us/download. 

If you choose to run the latest release version you can simply navigate to the **release/** directory and run **HazelProject.NFTGenerator.exe**. 
If you are using macOS, you will have to use Terminal to navigate to the **release/** directory and use the `dotnet run` command.

Before running the tool you will need to save your files inside a parent directory and to edit the `config.json` from the **release/** directory if you run the release version.
If you choose to build the project yourself, you will need to edit `config.json` from **src/**.

## Modifying config.json

```
{

  "BaseDirectory": "/Users/HazelProject/NFT/",
  "NftsToCreate": 100,
  "ImageFormat": ".png",
  "ImageWidth": 3000,
  "ImageHeight": 3000,
  "NftName": "Hazel #",
  "Description": "Example descripton for Hazel Project",

  "Traits": [
    {
      "id": 1,
      "name": "background",
      "directory": "background",
      "presenceChance": 100,
      "rarityWeights": [ 10, 20, 1 ]
    },
    {
      "id": 2,
      "name": "body",
      "directory": "body",
      "presenceChance": 100
    }
  ]
}
```

1. `BaseDirectory`: should point to a directory that contains a sub directory named **assets/**
2. `NftsToCreate`: should contain the number of NFT images to be generated
3. `ImageFormat`: file extension for generated images. Note: currently it supports only `.png` and `.jpeg`. We recommend using `.png`.
4. `ImageWidth`: output image width in pixels. It should  be the same width as the input files.
5. `ImageHeight`: output image height in pixels. It should  be the same height as the input files.
6. `NftName`: Name of each token that will be generated. If the name contains `#`, its index will be appended after it.
7. `Description`: The description of the collection that will be displayed on OpenSea.
8. `Traits`: A list of traits used for the composition of the output images (e.g. body, eyes, mouth etc.):
   - `id`: unique number for each trait. It will be used to order layers, 1 will be the bottom layer while, the biggest number will be the topmost layer
   - `name`: the name of the trait that will be dispalyed in OpenSea
   - `directory`: name of the folder in which the trait images are located. (the folder needs to exist inside **assets/**)
   - `presenceChance`: the chance of the trait to appear in the collection, expressed as percentage. It has to be number between 0 and 100. This is an optional property, if you leave this field empty the program will treat it as 100
   - `rarityWeights`: the rarity distribution of traits, it is an array of numbers where each number represents the weight of the corresponding image inside the trait subfolder. This is an optional property, if you leave this field empty the program will give an equal chance to every image within that layer
   (e.g. background has the following rarityWeights: [ 10, 20, 0 ]. This means that the first background image will appear two times less than the second one but 10 times more than the third image)


## Tool Usage

On Windows, run **HazelProject.NFTGenerator.exe** from **release/**.
On macOS, navigate to **release/** using Terminal and use `dotnet run` command.

The first input required from the user is the name of the collection. This name will be used to create a directory under **output/** in which all output data will be saved.
The program will generate all images as defined in `config.json`.

Next, the user can choose to delete unwanted images from **output/{collectionName}/images/**. After deletion is completed, the user will need to indicate if any images were removed by inputting **y** or **n** inside the console.

If any images were removed, after indicating **y**, the program will rename them accordingly and will indicate that the user has to upload **output/{collectionName}/images/** directory to IPFS through Pinata Gateway.

After upload is complete, the user has to copy the CID from Pinata and paste it in the console. After pasting and pressing `Enter` key, the program will generate the OpenSea compliant metadata. 

The program will indicate that the user has to upload **output/{collectionName}/metadata/** directory to IPFS through Pinata Gateway.

Congratz!

## Output

After the tool finishes execution, the output will be present inside the **output/** sub-directory of the `BaseDirectory` path from `config.json`.
The structure of the **output/** directory is the following:
1. collectionName:
   - images: contains all NFT images
   - metadata: contains one `json` file for each image (OpenSea standard compliant)
2. metadata.json (file containing all properties of all images in json format)
