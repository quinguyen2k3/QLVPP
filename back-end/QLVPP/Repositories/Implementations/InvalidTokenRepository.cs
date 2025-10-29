using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class InvalidTokenRepository : BaseRepository<InvalidToken>, IInvalidTokenRepository
    {
        private readonly AppDbContext _context;

        public InvalidTokenRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> Exists(string jti)
        {
            return await _context.InvalidTokens.AnyAsync(t => t.Jti == jti);
        }

        public async Task<InvalidToken?> GetById(string jti)
        {
            return await _context.InvalidTokens.FirstOrDefaultAsync(i => i.Jti == jti);
        }
    }
}
