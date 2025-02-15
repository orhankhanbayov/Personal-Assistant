using AutoMapper;
using DBService;
using DBService.Models;
using DBService.Services;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class TaskService : Tasks.TasksBase
{
	private readonly AppDbContext _dbContext;
	private readonly IMapper _mapper;

	public TaskService(AppDbContext dbContext, IMapper mapper)
	{
		_dbContext = dbContext;
		_mapper = mapper;
	}

	public override async Task<GetTaskByTaskIDReply> GetTaskByTaskID(
		GetTaskByTaskIDRequest request,
		ServerCallContext context
	)
	{
		var task = await _dbContext
			.Tasks.Where(t => t.TaskID == request.TaskID)
			.FirstOrDefaultAsync();

		if (task == null)
			throw new RpcException(new Status(StatusCode.NotFound, "Task not found"));

		return new GetTaskByTaskIDReply { TaskDetailsEntity = _mapper.Map<TaskDetailRecord>(task) };
	}

	public override async Task<AddTaskReply> AddTask(
		AddTaskRequest request,
		ServerCallContext context
	)
	{
		var taskDetails = _mapper.Map<TaskDetails>(request.TaskDetailsEntity);

		await _dbContext.Tasks.AddAsync(taskDetails);
		var saveResult = await _dbContext.SaveChangesAsync();

		return new AddTaskReply { Success = saveResult };
	}
}
