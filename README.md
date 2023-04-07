# Mars Undiscovered

Mars Undiscovered is a roguelike game currently in development. It is developed using C#, .NET and MonoGame. It uses several customised third party libraries including GoRogue and GeonBit UI.

You can find out more about the game at the game's website: [Mars Undiscovered hosted on Azure](https://marsundiscovered.azurewebsites.net).  Navigate to the [Releases](https://github.com/DavidFidge/MarsUndiscovered/releases) section for current and past releases.

This project does not currently use nuget packages for a number of third party libraries as I've forked them to make my own changes. After cloning this repository you must clone the following GitHub projects to the parent folder of DavidFidge/MarsUndiscovered: DavidFidge/FrigidRogue, DavidFidge/GoRogue (checkout FrigidRogue branch), DavidFidge/GeonBit.UI (checkout FrigidRogue branch), DavidFidge/BehaviourTree and DavidFidge/MonoGame.Extended (checkout FrigidRogue branch). You can then dotnet build the MarsUndiscovered.sln which will restore and build all projects.

## Copyright

This project is currently visible to the public. However all source code in this repository is copyright David Fidge and thus is not open source. The 'core' libary that I have developed for this project, DavidFidge/FrigidRogue, is open source and available for use under the MIT License.
