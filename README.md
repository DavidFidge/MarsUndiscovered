# Mars Undiscovered

Mars Undiscovered is a roguelike game currently in development. It is developed using C#, .NET and MonoGame. It uses several customised third party libraries including GoRogue and GeonBit UI.

The website for the game is here: [https://marsundiscovered.azurewebsites.net](https://marsundiscovered.azurewebsites.net)

It does not currently use nuget packages for several third party libraries. After cloning this repository you must clone DavidFidge/FrigidRogue, DavidFidge/GoRogue, DavidFidge/GeonBit.UI and DavidFidge/BehaviourTree. You can then dotnet build the MarsUndiscovered.sln which will restore and build all projects. If this project nears completion or gains more developers I will build nuget packages, but right now it is easier being able to make changes to all projects at once in one solution.

## Copyright

This project is currently visible to the public. However all source code in this repository is copyright David Fidge. The 'core' libary that I have developed for this project, DavidFidge/FrigidRogue, is available for use under the MIT License.
