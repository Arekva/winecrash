![alt text](https://github.com/breaks-and-continues/winecrash/raw/master/src/previews/banner.png "In-Game main menu")

# Winecrash, The unstable french cubic experience.

Hey, welcome to the repos of Winecrash!

## ⚠️ DISCLAMER
__Winecrash__ and __breaks-and-continues__ are both **unaffiliated** to __Minecraft__ nor __Mojang Studios__.
This is a fan project I enjoy making in my free time to train my C#. However feel free to contribute! 😉

If any legal issue would come up, please contact me on [arekva@protonmail.com](mailto:arekva@protonmail.com "Send a mail to arekva@protonmail.com").

## Run the game

#### 🎮 Play the game as a non developper
If you simply want to play the game, get the last available version from the [release tab of the project](https://github.com/breaks-and-continues/winecrash/releases "Releases of the game"). 

For now (07/09/2020) the last release is the Predev 0.1 which is **very old** compared to the current project version, however I will try to frequently (**__during my free time__**) update the game as soon the first alpha will be released (not ETA yet). 

###### For now I would advise to make a build of the last commited version (see below).

#### 🛠️ Build and play the game by your own
If you want to edit or build the last version of the project, you first may download or [clone](https://docs.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository "How to clone a repository").

###### Windows
* Download the last version of [Visual Studio 2019](https://visualstudio.microsoft.com/fr/vs/ "Visual Studio Homepage") (Community is free for everyone).
* Into the installer, select **.NET Desktop Development** then go into __Individual Componants__ and check **.NET Framework 4.8 Target Pack** and finally install Visual Studio 2019.
* Once installed (this can take some time), open __Winecrash.sln__ (situated at src/Winecrash/Winecrash.sln). If you did follow the steps until now, the project should load with no error.
* Then go into the **Solution Explorer** window (default at the __right__ side of Visual Studio) and do right click onto **'Winecrash' Solution** and then press **Build Solution**.
* It should either have created a folder into __src/Winecrash/Winecrash.Client/bin/x64/(Debug **OR** Release)__ **OR** __src/Winecrash/Winecrash.Client/bin/(Debug **OR** Release)__. Search the folder where the **Winecrash.exe** and copy the **asset/** folder next to the **Winecrash.exe**.
* Run **Winecrash.exe** and have fun!

###### Linux
* Install [Mono](https://www.mono-project.com/).
* From your package manager, install the `msbuild` package.
* You may need to install [.NET Core](https://dotnet.microsoft.com/download/dotnet-core) to install NuGet packages.
* Clone the repository
* Navigate to src/Winecrash
* `nuget restore`
* `msbuild Winecrash.sln`
* Navigate to Winecrash.Client/bin/Debug
* Copy the assets folder into it
* `mono Winecrash.exe`







 [Minecraft texture pack](https://cdn.discordapp.com/attachments/731567608071192595/786713560071929896/Winecrash.zip)