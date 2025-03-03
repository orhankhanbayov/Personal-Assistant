#!/bin/bash
set -e


# Path to the mounted certificate directory (adjust if needed)
CERT_MOUNT_DIR="/https"
TARGET_CERT_DIR="/usr/local/share/ca-certificates"

# If the certificate exists in the mounted folder, copy it
CERT_FILES=("AssistantService.crt")

for cert_file in "${CERT_FILES[@]}"; do
    if [ -f "${CERT_MOUNT_DIR}/${cert_file}" ]; then
        echo "Certificate ${cert_file} found. Copying to CA certificates directory..."
        cp "${CERT_MOUNT_DIR}/${cert_file}" "${TARGET_CERT_DIR}/aspnetapp${cert_file}"
    else
        echo "Certificate ${cert_file} not found in ${CERT_MOUNT_DIR}."
    fi
done

# Update CA certificates
update-ca-certificates

service ssh start


# Execute the CMD passed to the container (e.g., starting your application)
exec "$@"

