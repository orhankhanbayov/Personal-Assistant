import requests
import uuid
import urllib3

# Disable warnings for self-signed certificates (if using localhost with self-signed certs)
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Base URL of the API
base_url = "https://localhost:6443"

# Generate a random CallSid to be used across all requests
call_sid = str(uuid.uuid4())
from_number = "+447874158451"
to_number = "+1234567890"  # any consistent number

def send_request(endpoint, payload):
    url = f"{base_url}{endpoint}"
    response = requests.post(url, files=payload, verify=False)
    print(f"Request to {endpoint} returned {response.status_code}: {response.text}")
    return response

initial_payload = {
    "CallSid": (None, call_sid),
    "From": (None, from_number),
    "To": (None, to_number),
    "CallStatus": (None, "ringing"),
    "SpeechResult": (None, "")
}

initial_response = send_request("/IncomingCalls/InitialCall", initial_payload)

callback_payload_in_progress = {
    "CallSid": (None, call_sid),
    "From": (None, from_number),
    "To": (None, to_number),
    "CallStatus": (None, "in-progress"),
    "SpeechResult": (None, "create a task called not forget to not forget") 

}

if initial_response.status_code == 200:
    callback_response_in_progress = send_request("/IncomingCalls/Callback", callback_payload_in_progress)

callback_payload_completed = {
    "CallSid": (None, call_sid),
    "From": (None, from_number),
    "To": (None, to_number),
    "CallStatus": (None, "completed"),
    "SpeechResult": (None, "")
}

print("\nSending completed callback request...")
if callback_response_in_progress.status_code == 200:
    callback_response_completed = send_request("/IncomingCalls/Callback", callback_payload_completed)



