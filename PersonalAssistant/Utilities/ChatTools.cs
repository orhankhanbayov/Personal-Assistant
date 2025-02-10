using OpenAI.Chat;

namespace PersonalAssistant.Utilities;

public static class ChatTools
{
	public static ChatTool ReadFromCalendarTodayTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.ReadFromCalendarTodayAsync),
		"Get the user's events from their calendar for today"
	);

	public static ChatTool AddToCalendarTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.AddToCalendarAsync),
		"Add an event to the calendar for the future",
		BinaryData.FromString(
			"""
			{
			    "type": "object",
			    "properties": {
			        "datetime": {
			            "type": "string",
			            "format": "date-time",
			            "description": "Date and time of the event. Must be a valid ISO 8601 date-time string."
			        },
			        "name": {
			            "type": "string",
			            "description": "Name of the event. A brief, descriptive title."
			        },
			        "location": {
			            "type": "string",
			            "description": "Location of the event. Can be an address or virtual meeting link."
			        },
			        "notes": {
			            "type": "string",
			            "description": "Additional notes or details about the event."
			        },
			        "notification": {
			            "type": "object",
			            "properties": {
			                "Message": {
			                    "type": "string",
			                    "description": "Notification message to be sent."
			                },
			                "SentAt": {
			                    "type": "string",
			                    "format": "date-time",
			                    "description": "Timestamp when the notification should be sent. Required and must be a valid ISO 8601 date-time string."
			                }
			            },
			            "required": ["Message", "SentAt"]
			        }
			    },
			    "required": ["datetime", "name", "notes"]
			}
			"""
		)
	);

	public static ChatTool RemoveFromCalendarTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.RemoveFromCalendarAsync),
		"Remove an event from the calendar",
		BinaryData.FromString(
			"""
			{
			    "type": "object",
			    "properties": {
			        "eventId": {
			            "type": "string",
			            "description": "ID of the event to be removed."
			        }
			    }
			}
			"""
		)
	);

	public static ChatTool UpdateCalendarEventTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.UpdateCalendarEventAsync),
		"Update an existing event in the calendar",
		BinaryData.FromString(
			"""
			{
			    "type": "object",
			    "properties": {
			        "eventId": {
			            "type": "string",
			            "description": "ID of the event to be updated."
			        },
			        "datetime": {
			            "type": "string",
			            "format": "date-time",
			            "description": "New date and time of the event."
			        },
			        "name": {
			            "type": "string",
			            "description": "New name of the event."
			        }
			    }
			}
			"""
		)
	);

	public static ChatTool GetEventDetailsTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.GetEventDetailsAsync),
		"Get details of an event from the calendar",
		BinaryData.FromString(
			"""
			{
			    "type": "object",
			    "properties": {
			        "eventId": {
			            "type": "string",
			            "description": "ID of the event to get details for."
			        }
			    }
			}
			"""
		)
	);
	public static ChatTool CreateTaskTool = ChatTool.CreateFunctionTool(
		nameof(ChatToolsFunctions.CreateTaskAsync),
		"Add a task to the list for the future",
		BinaryData.FromString(
			"""
			{
			    "type": "object",
			    "properties": {
			        "name": {
			            "type": "string",
			            "description": "Title of the task."
			        },
			        "notes": {
			            "type": "string",
			            "description": "Additional notes or description for the task."
			        },
			        "priority": {
			            "type": "string",
			            "description": "Priority level of the task."
			        },
					"dueDate": {
			            "type": "string",
			            "description": "Due date of the task. Must be a valid ISO 8601 date-time string."
			        },
					  "notification": {
			            "type": "object",
			            "properties": {
			                "Message": {
			                    "type": "string",
			                    "description": "Notification message to be sent."
			                },
			                "SentAt": {
			                    "type": "string",
			                    "format": "date-time",
			                    "description": "Timestamp when the notification should be sent. Required and must be a valid ISO 8601 date-time string."
			                }
			            },
			            "required": ["Message", "SentAt"]
			        }
			    },
			    "required": ["name", "notes"]
			}
			"""
		)
	);
}
