# Smart-Meter-System
University Smart Meter System.

## How to run

There are 2 main ways to run this project.  
The easiest way to do this is by using the preconfigured docker compose.  
The other way is by individually running each project and configuring all the env files to match.

### Docker Compose

To run the project via docker compose start by cloning the respoitory.

```bash
git clone https://github.com/illuminat3/Smart-Meter-System.git
cd Smart-Meter-System
```

If you do not have docker installed, you will need to install it.  
You can find the link to download this in the [requirements](#requirements)  
Once you have docker installed you will have to run the following commands

```bash
docker compose pull
docker compose up -d
```

This should run the entire project.

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

## Packages

We are using ghcr.io for the packages.  
All packages have the same visibility as the repository.  
You can access all repository packages [here](https://github.com/illuminat3?tab=packages&repo_name=Smart-Meter-System).

## AI Usage

AI is used to automatically review code.  
It is also used to help decide on technologies for approach.  
Outside of this no AI is used.  
This project has an AITS Score of 2
