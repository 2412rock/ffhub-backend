﻿using FFhub_backend.Database;
using FFhub_backend.Database.Models;
using FFhub_backend.HttpReponse;
using Microsoft.EntityFrameworkCore;

namespace FFhub_backend.Services
{
    public class AccessLogService
    {
        private readonly FFDbContext _dbContext;
        public AccessLogService(FFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddLog(string ipAddress)
        {
            var existingLog = await _dbContext.AccessLogs.FirstOrDefaultAsync(e => e.IpAddress == ipAddress);
            if (existingLog == null)
            {
                await _dbContext.AccessLogs.AddAsync(new DBAccessLog() { AccessTime = DateTime.Now, IpAddress = ipAddress });

            }
            else
            {
                existingLog.AccessTime = DateTime.Now;
                _dbContext.AccessLogs.Update(existingLog);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Maybe<List<DBAccessLog>>> GetLogs()
        {
            var list = await _dbContext.AccessLogs.ToListAsync();
            var maybe = new Maybe<List<DBAccessLog>>();
            maybe.SetSuccess(list);
            return maybe;
        }
    }
}
