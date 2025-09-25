# Smart-Meter-System
University Smart Meter System.

## Group Project

This is a group project with:

- Matthew Rawson C3060480
- William Pearson C3067280
- Jacob Allmedinger C3018245
- Aran Bansal C4037436

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

## AI Usage

AI is used to automatically review code.  
It is also used to help decide on technologies for approach.  
Outside of this no AI is used.  
This project has an AITS Score of 2
