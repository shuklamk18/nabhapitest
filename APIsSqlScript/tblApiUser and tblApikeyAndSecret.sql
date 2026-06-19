create table ApiUser(
ApiUserId int not null identity primary key,
emailId  varchar(100) not null unique,
mobileNumber varchar(12) not null unique,
orgName varchar(100) not null,
CreatedOn datetime default getdate(),
ModifyOn datetime default getdate()
)

CREATE TABLE tblApiKey
(
    ApiKeyId INT IDENTITY(1,1) PRIMARY KEY,

    ApiUserId INT NOT NULL,

    KeyName VARCHAR(100) NOT NULL,

    ApiKey VARCHAR(100) NOT NULL UNIQUE,

    ApiSecretHash VARCHAR(500) NOT NULL,

    IsRevoked BIT NOT NULL DEFAULT(0),

    ExpiresOn DATETIME NULL,

    LastUsedOn DATETIME NULL,

    CreatedOn DATETIME NOT NULL DEFAULT(GETDATE()),

    CONSTRAINT FK_tblApiKey_ApiUser
        FOREIGN KEY(ApiUserId)
        REFERENCES ApiUser(ApiUserId)
        ON DELETE CASCADE
)
GO

CREATE TABLE tblApiRequestLog
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    ApiUserId INT NULL,

    ApiKeyId INT NULL,

    Endpoint VARCHAR(500) NULL,

    HttpMethod VARCHAR(20) NULL,

    IpAddress VARCHAR(100) NULL,

    StatusCode INT NULL,

    RequestDate DATETIME NOT NULL DEFAULT(GETDATE())

	CONSTRAINT FK_tblApiRequestLog_ApiUser
        FOREIGN KEY(ApiUserId)
        REFERENCES ApiUser(ApiUserId)
        ON DELETE CASCADE
)
GO