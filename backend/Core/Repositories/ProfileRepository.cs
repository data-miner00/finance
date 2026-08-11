using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Data;

namespace Core.Repositories
{
    public sealed class ProfileRepository : IProfileRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public ProfileRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Profile?> GetAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "SELECT TOP (1) * FROM [dbo].[Profiles];";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstOrDefaultAsync<ProfileDto>(query);

            return dto?.ToModel();
        }

        public async Task<Profile> SaveAsync(Profile entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = this.connectionFactory.CreateConnection();

            var existingId = await connection.QueryFirstOrDefaultAsync<Guid?>(
                "SELECT TOP (1) [Id] FROM [dbo].[Profiles];");

            var parameters = new DynamicParameters();
            parameters.Add("Username", entity.Username);
            parameters.Add("FirstName", entity.FirstName);
            parameters.Add("LastName", entity.LastName);
            parameters.Add("Email", entity.Email);
            parameters.Add("Bio", entity.Bio);
            parameters.Add("CompanyName", entity.CompanyName);
            parameters.Add("WebsiteUrl", entity.WebsiteUrl);
            parameters.Add("AvatarImage", entity.AvatarImage);

            ProfileDto? savedProfile;

            if (existingId is null)
            {
                savedProfile = await connection.QuerySingleOrDefaultAsync<ProfileDto>(
                    SpNames.AddProfile,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                parameters.Add("Id", existingId.Value, DbType.Guid);

                savedProfile = await connection.QuerySingleOrDefaultAsync<ProfileDto>(
                    SpNames.UpdateProfile,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return savedProfile!.ToModel();
        }
    }
}
