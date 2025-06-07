In a new and empty folder.

```powershell
> dotnet new console
> dotnet add package Microsoft.NET.Build.Containers
```

Edit the `.csproj` file and add this property in the first `<PropertyGroup>`:

```
<ContainerImageName>$(MSBuildProjectName)-image</ContainerImageName>
```

Note that the container image name is derived from the name of the project, but it can
be something else if you want (not sure what the limitations are).

Make sure you have [Docker](https://www.docker.com/) running on your computer.

Publish the solution and publish the container to the local docker container registry

```
dotnet publish --os linux --arch x64 -t:PublishContainer -c Release
```
