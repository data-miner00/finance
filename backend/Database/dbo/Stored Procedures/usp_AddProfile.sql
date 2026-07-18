-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Inserts the profile row and returns it. Only ever called
--              when the Profiles table is empty (0-or-1-row invariant).
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddProfile]
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

    INSERT INTO [dbo].[Profiles]
	(
		[Username],
		[FirstName],
		[LastName],
		[Email],
		[Bio],
		[CompanyName],
		[WebsiteUrl],
		[AvatarImage]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Username,
		@FirstName,
		@LastName,
		@Email,
		@Bio,
		@CompanyName,
		@WebsiteUrl,
		@AvatarImage
	);

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
	JOIN @OutputTable r
	ON p.Id = r.Id;
END
