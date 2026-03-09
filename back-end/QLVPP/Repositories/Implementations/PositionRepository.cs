using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(AppDbContext context)
            : base(context) { }
    }
}
