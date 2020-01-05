USE [AzureStaging]
GO

CREATE PROCEDURE CreateAccount
(
	@AccountId uniqueidentifier,
	@AccountName nvarchar(160),
	@AccountNumber nvarchar (20),
	@Description nvarchar(max),
	@WebsiteUrl nvarchar(200),
	@EmailAddress1 nvarchar(100),
	@Telephone1 nvarchar(50),
	@CreatedById uniqueidentifier,
	@OwnerId uniqueidentifier,
	@StateCode int,
	@StatusCode int
)
AS

BEGIN

declare @CreatedDateTime datetime
SET @CreatedDateTime = getdate()

INSERT INTO [dbo].[Accounts]
           ([AccountId]
           ,[Name]
           ,[AccountNumber]
           ,[Description]
           ,[WebSiteURL]
           ,[EMailAddress1]
           ,[Telephone1]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[StateCode]
           ,[StatusCode]
           ,[OwnerId])
     VALUES
           (
			@AccountId,
			@AccountName,
			@AccountNumber,
			@Description,
			@WebsiteUrl,
			@EmailAddress1,
			@Telephone1,
			@CreatedDateTime,
			@CreatedById,
			@CreatedDateTime,
			@CreatedById,
			@StateCode,
			@StatusCode,
			@OwnerId
		   )
END
GO


