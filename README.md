# Mars Undiscovered

Mars Undiscovered is a roguelike game currently in development. It is developed using C#, .NET and MonoGame. It uses several customised third party libraries including GoRogue and GeonBit UI.

The website for the game is here: [Mars Undiscovered hosted on Azure](https://marsundiscovered.azurewebsites.net)

It does not currently use nuget packages for several third party libraries. After cloning this repository you must clone DavidFidge/FrigidRogue, DavidFidge/GoRogue (checkout FrigidRogue branch), DavidFidge/GeonBit.UI (checkout FrigidRogue branch), DavidFidge/BehaviourTree and DavidFidge/MonoGame.Extended (checkout FrigidRogue branch) to the parent folder of the cloned MarsUndiscovered. You can then dotnet build the MarsUndiscovered.sln which will restore and build all projects. If this project nears completion or gains more developers I will build nuget packages, but right now it is easier being able to make changes to all projects at once in one solution.

## Copyright

This project is currently visible to the public. However all source code in this repository is copyright David Fidge and thus is not open source. The 'core' libary that I have developed for this project, DavidFidge/FrigidRogue, is open source and available for use under the MIT License.
