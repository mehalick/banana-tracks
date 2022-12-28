using BananaTracks.Api.Shared.Protos;
using Grpc.Core;

namespace BananaTracks.Api.Services;

public class RoutineService : BananaTracks.Api.Shared.Protos.RoutineService.RoutineServiceBase
{
	public override async Task<GetRoutinesReply> GetRoutines(GetRoutinesRequest request, ServerCallContext context)
	{
		var reply = new GetRoutinesReply();
		reply.Routines.Add(new GetRoutinesItem
		{
			RoutineId = "123",
			Name = "Test"
		});

		await Task.Delay(1);

		return reply;
	}
}
