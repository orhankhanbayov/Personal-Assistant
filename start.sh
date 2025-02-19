#!/bin/bash
# run-services.sh
# Make sure you run this script from the directory containing your service folders

# Array of .NET service directories
services=(
    "AssistantService"
    "DatabaseService"
    "CacheService"
    "SchedulerService"
    "ChatGPTService"
    "TwilioService"
)

# Loop through each service and start it
for service in "${services[@]}"; do
    echo "Starting ${service}..."
    (
        cd "$service" || { echo "Directory $service not found!"; exit 1; }
        dotnet run
    ) &
done

# Wait for all background processes to finish
wait
