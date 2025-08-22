# File Monolith

File Monolith is a set of .NET 8 WinForms file management tools for Metal Gear Solid V. The goal of these tools is to improve accessibility for MGSV modding.

## Tools Overview

* **Archive Unpacker**: The user can select Archive (.dat) files, which the tool will unpack into a target directory. Then, the tool will unpack all .fkp, .fpkd, .pftxs, and .sbp files within the directory.
* **Mass Texture Converter**: The user can select a directory and the tool will attempt to convert all texture files from .ftex(s) to .dds. The resulting .dds files are sent to a target directory.
* **File Proliferator**: The user selects any number of files, of any type. The tool will search for these files in the MGSV file structure. If these filenames are found in the game files, the tool creates a directory structure to mirror MGSV's and then copies the user's files into the structure.
* **Filename Updater**: The user can select any files with hashed filenames, and the tool will attempt to update their names and filepaths using the latest qar_dictionary. Any updated files are copied to the target directory.
* **Archive Transferrer**: The user selects their Ground Zeroes and Phantom Pain executable files, and the tool will attempt to automatically transfer the game archives from Ground Zeroes and Metal Gear Online into The Phantom Pain.
* **Texture Aggregator**: The user can select a Packed Fox Textures (.pftxs) file, and the tool will attempt to automatically complete the textures partially contained in the .pftxs, and then convert the textures to DirectDraw Surface (.dds) format.
* **Regex File Copier**: The user can copy files and their directory structures to another folder using regular expressions.
  
## Archive Unpacker

The Archive Unpacker is a simple tool which unpacks user-specified .dat files, and then unpacks all .fpk, .fpkd, .pftxs and .sbp files which resided in the .dat files into one single directory structure. 
This tool has three purposes:
1. To provide the user with a "monolithic" view of MGSV's files, assuming the user chooses to unpack all .dat files.
2. To provide Mass Texture Converter with .ftex(s) files.
3. To provide File Proliferator with file lists (TppMasterFileList.txt is pre-included in the download) and a texture directory (for pulling vanilla .ftex(s) files into _pftxs folders).

Notes:
* Unpacking all .dat files will take a while. 
* The tool will unpack in reverse-alphabetical order, starting with texture4.dat and ending with 0/00.dat. This allows for any updated files to overwrite their older versions. For this reason, I do not recommend unpacking modded .dats into your target folders.
* The unpacked (Condensed) directory structure requires ~25GB of space, excluding MGO/SDD Archives.
* The "Condensed Directory Structure" option will take the files contained within the .pftxs, .fpk, .fpkd and .sbp and pull them to the head of the directory structure.
  * (Ex: \Assets\tpp\SomePack_fpk\Assets\SomeFile.fmdl -> \Assets\SomeFile.fmdl). 
  * Otherwise, the files will remain in their unpacked folders (Ex: \Assets\tpp\SomePack_fpk\Assets\SomeFile.fmdl). 
* The "Condensed Directory Structure" checkbox must be checked in order for Mass Texture Converter to convert all MGSV textures.

## Mass Texture Converter

The Mass Texture Converter enables the user to convert any given amount of .ftex/.ftexs files into .dds files. The user simply selects the input folder, the output folder, and whether to include subdirectories for conversion.
This tool has one purpose:
1. To provide the user with an easy and safe way of converting a large number of .ftex/.ftexs into .dds files.

Notes:
* Unpacking all of MGSV's textures will take a while.
* In order to convert a .ftex file, all of the .ftexs files for the texture must be contained in the same folder. 
  *For this reason, the Archive Unpacker must use the "Condensed Directory Structure" option (Some .ftex and .1.ftexs are contained only in .pftxs files, so they must be pulled into the same folder as the remaining .ftexs).
* If "Convert Subfolders" is selected, the tool will also search through the subfolders of the input directory. The output directory will retain the directory structure. 
  * (Ex: Input\Assets\SomeTexture.ftex -> Output\Assets\SomeTexture.dds).
* I recommend using the SageThumbs Plugin in order to preview the .dds files in their thumbnails without opening them in GIMP or Photoshop.
  * https://www.cherubicsoft.com/en/projects/sagethumbs
* Excluding MGO/SDD textures, the unpacked .dds files require ~21GB of space.

## File Proliferator

The File Proliferator tool is the most powerful and feature-rich tool of the three. Although it has a number of use-cases and functions, its primary role is to create MakeBite-ready file directories for MGSV. From the files input by the user, this tool will search through the entirety of MGSV's files (including the contents of .fpk, .fpkd, .pftxs and .sbp files) for a matching filename. Upon finding a match, the tool will create a mimic of the file's directory structure in the target directory, and then place a copy of the user's file into that directory. In essence, this "proliferates" the user's files into the MakeBite structure, ready to be packed into a .mgsv file.

This tool has a number options, but only one purpose:
1. To help create a MakeBite-ready file structure from the user's input files.

Notes:
* **TppMasterFileList.txt**: The tool will look up files from a text document, TppMasterFileList.txt, which lists all of the file paths (including files contained in .fpk, .fpkd, .pftxs and .sbp files), excluding those from MGO/SDD Archives.
  * TppGeneratedFileList.txt is built automatically after using the Archive Unpacker. The user can promote TppGeneratedFileList.txt to TppMasterFileList.txt by simply changing the filename and replacing the old TppMasterFileList.txt.
  * Users can manually add file path entries into TppMasterFileList.txt, and File Proliferator will accept these paths as true MGSV file paths. 
    * This may be useful if the user intends to use custom texture or script names for their mod.
* **Check for filename updates**: If this box is checked, the tool will check the input filenames for dictionary updates. If an update is found, the tool will create a copy of the file with an updated name, and continue the proliferation process using the updated filename.
    * Having up-to-date filenames is important, because File Proliferator is using TppMasterFileList.txt to match filenames with directories. If the name of a file is outdated, the tool won't be able to proliferate it, because it cannot locate it within TppMasterFileList.txt.
* **Reference File**: The user can choose to select a "Reference File" for their directory structure. File Proliferator will search the MGSV file paths for folders that contain the Reference File, and then create a directory structure to mimic wherever that file was found. All of the user's input files are then copied to those folders.
  * Using a Reference File is very situational, especially if the "Set in Root of Packs" checkbox is unchecked.
  * If the "Set in Root of Packs" checkbox is checked, File Proliferator will pull the user's file to the root directory of the _fpk, _fpkd, _pftxs and _sbp folders where the Reference File was found. This may be useful if the user is adding hashed files to the folder of a Reference File. 
    * (Ex: User input: 1aa64302cee42.ftex, Reference File: SomeFile.ftex, | 1aa64302cee42.ftex -> \Assets\SomePack_pftxs\1aa64302cee42.ftex)
* **Texture Options**: File Proliferator comes with three options for texture management when creating the MakeBite directory structure.
  * **Pull Vanilla .ftex | .ftexs to _pftxs**: Selecting this option will prompt the user to set a Texture Directory, where File proliferator can "autofill" the generated _pftxs folders by grabbing vanilla .ftex and .ftexs files and copying them to the _pftxs folders (in order to reflect the vanilla .pftxs file).
    * This option is intended to utilize the Archive Unpacker's Output Directory as a texture library.
    * This process may take a while depending on the number of generated _pftxs and their vanilla filesizes.
    * This function also utilizes the TppMasterFileList in order to find .ftex/.ftexs files that belong to a given pftxs path.
  * **Convert .dds to .ftex**: With this option enabled (on by default), The user can use .dds files as Input Files. The tool will attempt to convert these files to .ftex/.ftexs files before proliferating them to the MakeBite file structure.
    * If this option is not selected, using .dds files will not be converted or copied to the MakeBite directory structure (unless the user manually adds the .dds file paths to the TppMasterFileList.txt).
  * **Pack _pftxs**: Upon creating the MakeBite Directory Structure, this option will automatically pack any _pftxs folders to .pftxs files using BobDoleOwndU's AutoPftxsTool.
    * Since MakeBite does not automatically repack _pftxs folders like it does with _fpk or _fpkd, the user will need to either manually pack _pftxs folders, or check the "Pack _pftxs" checkbox.
    * After packing the .pftxs files, File Proliferator will ask the user whether to delete the leftover _pftxs folders. In either case, the user should not include _pftxs folders in their final MakeBite file.

## Filename Updater

The Filename Updater allows users to update old, hashed filenames to their unhashed filepaths. Assuming that the files were originally unpacked using an outdated version of the qar_dictionary.txt, this tool will copy and rename the input files to the target directory.

NOTICE:
* This tool's functionality has since been integrated into SnakeBite. The mod manager will automatically seek out filename updates during mod installation. For the sake of convention, It is still recommended that you keep your filenames up to date.
* File Proliferator has also integrated this filename updating feature. Refer to the File Proliferator overview for details.

This tool has one purpose:
1. To provide users with support when updating old mods.

Notes:
* Upon finding an update for a filename, the tool will create a copy of the hashed file. The copy is then renamed and moved to the output directory.
  * (Ex: 3cb5fc5a6e14d.2.ftexs -> \targetDirectory\\am10_main0_def_c00_bsm.2.ftexs)
* When a filename is updated, the tool can include the file's full filepath by checking the "Include Directory Structure" checkbox. The copy of the updated file will be sent into the directory structure.
  * (Ex: 3cb5fc5a6e14d.2.ftexs -> \targetDirectory\Assets\tpp\weapon\amo\Pictures\am10_main0_def_c00_bsm.2.ftexs)

## Archive Transferrer

The Archive Transferrer allows for the user to automatically import Ground Zeroes and Metal Gear Online data into The Phantom Pain. This tool will reformat and copy the files as necessary, without modifying the original data.

This tool has one purpose:
1. To provide the user with a streamlined method of pulling data archives from Ground Zeroes and Metal Gear Online into The Phantom Pain.

Notes:
* Currently the tool is only designed to transfer the "texture" archives from Ground Zeroes / Metal Gear Online. A future update should allow the user to transfer the "chunk" archives.
* This tool utilizes my [fork of GzsTool](https://github.com/JosephZoeller/GzsTool) in order to reformat the Ground Zeroes archive.
* Transferring the archives may take a moment.
* In order to read these foreign archives, SnakeBite makes the necessary edits to the foxfs.dat upon its initial setup, independently from the Archive Transferrer.
  * There is no order to setting up SnakeBite and importing foreign archives, but both steps are necessary in order to utilize the other games' data.

## Texture Aggregator

The Texture Aggregator allows for the user to automatically build textures from a Packed Fox Textures (.pftxs) file. Given a .pftxs, the tool will unpack the file's partial textures, pull the additional texture files from the user's unpacked archives (from the Archive Unpacker's output), and then (optionally) convert all fox textures to DirectDraw Surface (.dds) files.

This tool has one purpose:
1. To provide the user with a streamlined method of collecting all textures associated with a specified .pftxs file.

Notes:
* This tool expects the user to have unpacked the contents of all of TPP's texture archives to a directory using the Archive Unpacker tool.
* The aggregation process may take a moment, depending on the contents of the .pftxs file.
* **TppMasterFileList.txt**: Similar to the File Proliferator tool, Texture Aggregator references TppMasterFileList.txt to determine the additional textures which need to be pulled from the unpacked texture archives.
* I recommend using the SageThumbs Plugin in order to preview the .dds files in their thumbnails without opening them in GIMP or Photoshop.
  * https://www.cherubicsoft.com/en/projects/sagethumbs

 ## Regex File Copier

 More for researching, exploring and referencing files than creating mods. Given a regular expression (try https://regexr.com/), the tool will copy files that match the regular expression from one specified folder into another, retaining the subdirectory nesting of the copied files.
