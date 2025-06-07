In a new and empty folder

```powershell
> dotnet new console
```

Note that the container image name is derived from the name of the project, but it can
be something else if you want (not sure what the limitations are).

Make sure you have [Docker](https://www.docker.com/) running on your computer.

Run docker build to create the image an publish it to your local container registry.

```
docker build --pull --rm -f "Dockerfile" -t helloworldcontainer2:latest "." 
```
