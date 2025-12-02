# Smart-Meter-System

University Smart Meter System.

## How to run

There are 2 main ways to run this project.  
The easiest way to do this is by using the preconfigured docker compose.  
The other way is by individually running each project and configuring all the env files to match.  
You will need to have all programs from the requirements installed.

### Docker Compose

To run the project via docker compose start by cloning the repository.

```bash
git clone https://github.com/illuminat3/Smart-Meter-System.git
cd Smart-Meter-System
```

Next you will have to set up the env file.  
To do this simply copy the .env.example file

```bash
copy .env.example .env
```

Alternatively, you can manually copy paste it.  
If you wish you can update the `JWT__SECRET` value.  
If you do this please ensure that it is at least 256 bits long otherwise the meter api will return 500 errors.  
You can generate a 256-bit hash with this [online hash generator](https://tools.keycdn.com/sha256-online-generator)  
An example value of this env file should look something like

```txt
672eb0ec6b5ee7827704e2e1b4fe72a52e2a13b9c59b0acad18201b938d3cd88
```

At this point, if you do not have docker installed, you will need to install it.  
You can find the link to download this in the [requirements](#requirements)  
Once you have docker installed you will have to run the following commands

```bash
docker compose pull
docker compose up -d
```

This should run the entire project.  
You will be able to access it in your browser at [http://localhost:4173](http://localhost:4173)  
You should use the login credentials of `Client1` and `password_client_1`

### Individually

If you have run the project previously via the docker compose in the root folder you will have to remove the container group from docker desktop.  
If you do not do this there will be clashes.  

To run the project individually start by cloning the repository,

```bash
git clone https://github.com/illuminat3/Smart-Meter-System.git
cd Smart-Meter-System
```

To start with we will set up the mock-database.  

```bash
cd mock-database
docker compose pull
docker compose up -d
```

This should now run the mock-database container.  
To verify that this is the case you should be able to access it at [http://localhost:3030](http://localhost:3030) in your browser.  

Once this is done return to the root folder.

```bash
cd ..
```

Now we will set up the meter api.

```bash
cd meter-api/meter-api
copy .env.example .env
```

For this you should have visual studio and dotnet 8.0 installed.  
You should now head to the `Smart-Meter-System\meter-api` folder in your file explorer.  
This should be the folder that contains the `.sln` file.  
Open `meter-api.sln` in Visual Studio.  
Once this is open make sure to press the green run button.  

<img width="351" height="74" alt="{7ED9FCC6-52E8-4A90-B43D-F578102BA10F}" src="https://github.com/user-attachments/assets/4fe424bf-79e8-47fc-b5dd-e58a80993656" />

You can confirm this is running by opening the [swagger page](http://localhost:5234/swagger/index.html) or you can make a GET request to [http://localhost:5234/health](http://localhost:5234/health).  

Now that the meter api is running, we will setup the meter agent.

```bash
cd ../../
cd meter-agent/meter-agent
copy .env.example .env
```

Again you should have visual studio and dotnet 8.0 installed.  
You should now head to the `Smart-Meter-System\meter-agent` folder in your file explorer.  
This should be the folder that contains the `.sln` file.  
Open `meter-agent.sln` in Visual Studio.  
Once this is open make sure to press the green run button.  

<img width="434" height="87" alt="{DA63017C-0F04-433D-AD51-60042D6D03AF}" src="https://github.com/user-attachments/assets/24384c5c-6883-4af3-98b5-a74525825a0a" />

This will then run the meter-agent.  
By default the environment is set up with an 80% chance to fail.  
To change this locate the `ERROR_CHANCE` in the `.env` file.  

Finally, we will set up the frontend.  
For this you should have node.js installed 

```bash
cd ../../
cd frontend
copy .env.example .env
npm install
npm run dev
```

This should then run the frontend at [http://localhost:5173](http://localhost:5173)  
You should use the login credentials of `Client1` and `password_client_1`

If you need help with any of these steps you can watch tis [helpful video guide](https://www.youtube.com/watch?v=gDNOK6Na-mE)

## Group Project

This is a group project with:

- Matthew Rawson C3060480 ([illuminat3](https://github.com/illuminat3), [matthewrawsoninfotrack](https://github.com/matthewrawsoninfotrack))
- William Pearson C3067280 ([MARSPEARSWIL](https://github.com/MARSPEARSWIL))
- Jacob Allmedinger C3018245 ([jallmen](https://github.com/jallmen))

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
Here is a list of all [repository packages](https://github.com/illuminat3?tab=packages&repo_name=Smart-Meter-System).

## AI Usage

AI is used to automatically review code.  
It is also used to help decide on technologies for approach.  
Outside of this no AI is used.  
This project has an AITS Score of 2
