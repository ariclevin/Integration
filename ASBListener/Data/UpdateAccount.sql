USE [AzureStaging]
GO

CREATE PROCEDURE UpdateAccount
(
	@AccountId uniqueidentifier,
	@AccountName nvarchar(160),
	@AccountNumber nvarchar (20),
	@Description nvarchar(max),
	@WebsiteUrl nvarchar(200),
	@EmailAddress1 nvarchar(100),
	@Telephone1 nvarchar(50),
	@ModifiedById uniqueidentifier,
	@OwnerId uniqueidentifier,
	@StateCode int,
	@StatusCode int
)
AS

BEGIN

declare @UpdatedDateTime datetime
SET @UpdatedDateTime = getdate()

UPDATE [dbo].[Accounts] SET
	[Name] = @AccountName,
    [AccountNumber] = @AccountNumber,
    [Description] = @Description,
    [WebSiteURL] = @WebsiteUrl,
    [EMailAddress1] = @EmailAddress1,
    [Telephone1] = @Telephone1,
    [ModifiedOn] = @UpdatedDateTime,
    [ModifiedBy] = @ModifiedById,
    [StateCode] = @StateCode,
    [StatusCode] = @StatusCode,
    [OwnerId] = @OwnerId
WHERE [AccountId] = @AccountId

END
GO


