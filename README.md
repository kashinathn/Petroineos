# Power Position Service

A Windows service for generating intra-day power position reports from trade data, built with .NET 6.0.

## 📋 Overview

This service automatically generates CSV files containing aggregated power trade positions:
- Runs as a Windows service
- Aggregates trades hourly according to London timezone
- Generates CSV files at configurable intervals
- SOLID design pricniples applied

## 📥 Installation

### Prerequisites
- .NET 6.0 Runtime
- Windows Server 2012+/Windows 10+
- PowerService.dll (included)

### Steps
1. Clone repository
   ```bash
   git clone https://github.com/kashinathn/Petroineos

2. Restore Dependencies
  dotnet restore

3. Build Service
   dotnet publish -c Release -o ./publish

4. Install Service
   Open the command prompt in administration mode and run the below commands
   (change bin path accordingly)
   sc create "PowerPositionService" binPath="D:\Petroineos\PetroineosCodingChallenge\publish\PetroineosCodingChallenge.exe" start=auto
   sc start PowerPositionService

5. If the above step is not working then open .NET solution in Visual Studio and run the application

6. Configuration
   Change settings in appsettings.json





