-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates the profile row by Id and returns it.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateProfile]
    @Id UNIQUEIDENTIFIER,
    @Username NVARCHAR(50),
    @FirstName NVARCHAR(50) = NULL,
    @LastName NVARCHAR(50) = NULL,
    @Email NVARCHAR(255) = NULL,
    @Bio NVARCHAR(500) = NULL,
    @CompanyName NVARCHAR(100) = NULL,
    @WebsiteUrl NVARCHAR(255) = NULL,
    @AvatarImage NVARCHAR(500) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Profiles]
    SET
        [Username] = @Username,
        [FirstName] = @FirstName,
        [LastName] = @LastName,
        [Email] = @Email,
        [Bio] = @Bio,
        [CompanyName] = @CompanyName,
        [WebsiteUrl] = @WebsiteUrl,
        [AvatarImage] = @AvatarImage
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        p.[Id],
        p.[Username],
        p.[FirstName],
        p.[LastName],
        p.[Email],
        p.[Bio],
        p.[CompanyName],
        p.[WebsiteUrl],
        p.[AvatarImage],
        p.[CreatedAt],
        p.[UpdatedAt]
    FROM [dbo].[Profiles] p
    JOIN @OutputTable r ON p.Id = r.Id;
END
