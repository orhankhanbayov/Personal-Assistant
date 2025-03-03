#!/bin/bash
# Start the SSH service
set -e

service ssh start

# Start the .NET application
exec "$@"
