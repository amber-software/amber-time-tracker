using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTracking.Models;

namespace TimeTracking.Services.Sprints
{
    public interface ISprintsService
    {
        Task<IList<Sprint>> GetAllSprints();

        Task<Sprint> GetTargetSprintOrCurrentSprint(int? sprintId);
    }
}