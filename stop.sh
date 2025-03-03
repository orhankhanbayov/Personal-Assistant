#!/bin/bash
# stop-services.sh
# Run this script from the solution root directory

PID_FILE="./service_pids.txt"

if [[ ! -f "$PID_FILE" ]]; then
  echo "PID file not found. Are the services running?"
  exit 1
fi

while IFS= read -r pid; do
  if kill -0 "$pid" 2>/dev/null; then
    echo "Stopping service with PID $pid..."
    kill "$pid"
    # Optionally, wait a moment and force kill if still running:
    sleep 2
    if kill -0 "$pid" 2>/dev/null; then
      echo "Service $pid did not stop gracefully; force killing..."
      kill -9 "$pid"
    fi
  else
    echo "No running process with PID $pid found."
  fi
done < "$PID_FILE"

# Clean up PID file
rm "$PID_FILE"
echo "All services stopped."
