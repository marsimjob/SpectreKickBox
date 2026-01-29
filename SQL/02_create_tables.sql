USE KickBoxingClubDB
GO

-- Our Independent tables (no FK)
CREATE TABLE AppUser (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(30) NOT NULL,
    LastName VARCHAR(30) NOT NULL,
    DateOfBirth DATE NOT NULL
);

CREATE TABLE [Role] (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    Title VARCHAR(40) NOT NULL
);

CREATE TABLE [WeekDay] (
    DayID INT PRIMARY KEY,
    DayName VARCHAR(10) NOT NULL
);

CREATE TABLE PriceList (
    PriceID INT IDENTITY(1,1) PRIMARY KEY,
    Label VARCHAR(100) NOT NULL,  
    Amount DECIMAL(10,2) NOT NULL 
);

-- Tables with Foreign Keys
CREATE TABLE [Account] (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    Email VARCHAR(50) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
    UserID INT NOT NULL,
    RoleID INT NOT NULL,
    CONSTRAINT FK_Account_User FOREIGN KEY (UserID) REFERENCES AppUser(UserID),
    CONSTRAINT FK_Account_Role FOREIGN KEY (RoleID) REFERENCES [Role](RoleID)
);

CREATE TABLE MembershipPlan (
    MembershipPlanID INT IDENTITY(1,1) PRIMARY KEY,
    RoleID INT NOT NULL,
    PriceID INT NOT NULL,
    BillingPeriod VARCHAR(20) NOT NULL,
    CONSTRAINT FK_MembershipPlan_Role FOREIGN KEY (RoleID) REFERENCES [Role](RoleID),
    CONSTRAINT FK_MembershipPlan_PriceList FOREIGN KEY (PriceID) REFERENCES PriceList(PriceID)
);

CREATE TABLE Membership (
    MembershipID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    StartDate DATETIME NOT NULL DEFAULT GETDATE(),
    EndDate DATETIME NOT NULL,
    MembershipPlanID INT NOT NULL,
    CONSTRAINT FK_Membership_PlanID FOREIGN KEY (MembershipPlanID) REFERENCES MembershipPlan(MembershipPlanID),
    CONSTRAINT FK_Membership_User FOREIGN KEY (UserID) REFERENCES AppUser(UserID),
    CONSTRAINT CK_Membership_DateTime CHECK (EndDate > StartDate)
);

CREATE TABLE SessionType
(
SessionTypeID INT IDENTITY (1,1) PRIMARY KEY,
TypeTitle VARCHAR(30) NOT NULL
);

CREATE TABLE [Session] (
SessionID INT IDENTITY(1,1) PRIMARY KEY,
TrainerID INT NOT NULL,
SessionTypeID INT NOT NULL,
StartTime TIME NOT NULL,
Duration INT NOT NULL,
Capacity INT NOT NULL,
Focus VARCHAR(100),
DayID INT NOT NULL,
SessionWeek INT NOT NULL,
CONSTRAINT FK_Session_DayOfWeek FOREIGN KEY (DayID) REFERENCES [WeekDay](DayID),
CONSTRAINT FK_Session_SessionTypeID FOREIGN KEY (SessionTypeID) REFERENCES SessionType(SessionTypeID),
CONSTRAINT FK_Session_TrainerID FOREIGN KEY (TrainerID) REFERENCES [Account](AccountID),
CONSTRAINT CK_Session_Capacity CHECK (Capacity > 0 AND Capacity <= 30),
CONSTRAINT CK_Session_Week CHECK (SessionWeek BETWEEN 1 AND 53)
);

CREATE TABLE Invoice (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
    AccountID INT NOT NULL,
    MembershipPlanID INT NOT NULL,
    InvoiceDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Invoice_Account FOREIGN KEY (AccountID) REFERENCES Account(AccountID),
    CONSTRAINT FK_Invoice_Plan FOREIGN KEY (MembershipPlanID) REFERENCES MembershipPlan(MembershipPlanID)
);

CREATE TABLE Newsletter (
NewsletterID INT IDENTITY(1,1) PRIMARY KEY,
PostedByAccountID INT NOT NULL,
NewsTitle VARCHAR(200) NOT NULL,
NewsContent VARCHAR(2000) NOT NULL,
PostWeek INT NOT NULL,
PostYear INT NOT NULL,
NewsType VARCHAR(30) NOT NULL,
IsActive BIT NOT NULL DEFAULT 1,
CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
CONSTRAINT FK_Newsletter_PostedBy FOREIGN KEY (PostedByAccountID) REFERENCES Account(AccountID),
CONSTRAINT CK_Newsletter_PostWeek CHECK (PostWeek BETWEEN 1 AND 53)
);