# Mars Undiscovered

Mars Undiscovered is a rogue-like game currently in development. It is developed using C#, .NET and MonoGame. It uses several customised third party libraries including GoRogue and GeonBit UI.

It does not currently use nuget packages for several third party libraries. After cloning this repository you must clone DavidFidge/FrigidRogue, DavidFidge/GoRogue, DavidFidge/GeonBit.UI and Eraclys/BehaviourTree. You can then dotnet build the MarsUndiscovered.sln which will restore and build all projects. If this project nears completion or gains more developers I will build nuget packages, but right now it is easier being able to make changes to all projects at once in one solution.
