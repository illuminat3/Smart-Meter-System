# Smart-Meter-System
University Smart Meter System.

## Group Project

This is a group project with:

- Matthew Rawson C3060480 ([illuminat3](https://github.com/illuminat3), [matthewrawsoninfotrack](https://github.com/matthewrawsoninfotrack))
- William Pearson C3067280 ([MARSPEARSWIL](https://github.com/MARSPEARSWIL))
- Jacob Allmedinger C3018245 ([jallmen](https://github.com/jallmen))
- Aran Bansal C4037436 ([Aranb4](https://github.com/aranb4))

## Project Structure

- `frontend/` - Frontend dashboard that connects with a signalr socket to the meter-api  
- `meter-api/` - The API that handles smart meter connections and has a SignalR socket to the frontend with the live status of the meter  
- `meter-agent/` - The agent that connects to the meter-api and tells it its current state  
- `mock-database/` - The mock database connection as a docker compose and relevant accompanying files  

## Requirements
  
- [Docker](https://www.docker.com/products/docker-desktop/)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)  
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/download)
- [Node.js](https://nodejs.org/en/download)

## Docker Images

Here is a quick guide on how to pull docker images since it is not straightforward.

### Packages

We are using ghcr.io for the packages.  
All packages will be on a repository level.  
You can access all repository packages [here](https://github.com/illuminat3?tab=packages&repo_name=Smart-Meter-System).

### PAT

To access the packages you will need to create a Personal Access Token.  
To do this go to [classic token in developer settings](https://github.com/settings/tokens)  
Create a new classic token. Do not create a fine grained token.  
Call the token Docker Access and make sure to set the expiry to 90 days.  
You will then be given a bunch of options.  Make sure to select `write:packages`  
This will also tick a bunch of other options. This is all you need to do.  
Generate your token and make sure to save it somewhere so you don't lose it.  

### Docker commands

Now for the final step.  
Firstly, make sure that you have docker installed.  
If you do not, make sure to install it from the requirements section as it is required.  
With docker desktop open and running, open a new command prompt window.  
In here run the command `docker login ghcr.io -u Username`.  
For example, for me this command is `docker login ghcr.io -u illuminat3`.  
It will then ask you for a password, you should paste in the value of the PAT token from earlier in here.  
If all went well you should now have a successful login.  
At this point you will be able to pull docker images.  
This is done automatically when running `docker-compose up -d` for the first time on a container.  
You can also find specific pull commands in the packages section.

## AI Usage

AI is used to automatically review code.  
It is also used to help decide on technologies for approach.  
Outside of this no AI is used.  
This project has an AITS Score of 2
