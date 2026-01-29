USE KickBoxingClubDB
GO

-- Drop users and roles
DROP USER IF EXISTS KickBoxingAppUser;
DROP ROLE IF EXISTS KickBoxingReader;

-- Drop views
DROP VIEW IF EXISTS vw_PublicUsers;
DROP VIEW IF EXISTS vw_MembershipReport;

-- Drop tables in correct FK order
DROP TABLE IF EXISTS Newsletter;
DROP TABLE IF EXISTS Invoice;
DROP TABLE IF EXISTS [Session];
DROP TABLE IF EXISTS SessionType;
DROP TABLE IF EXISTS Membership;
DROP TABLE IF EXISTS MembershipPlan;
DROP TABLE IF EXISTS PriceList;
DROP TABLE IF EXISTS Account;
DROP TABLE IF EXISTS [WeekDay];
DROP TABLE IF EXISTS [Role];
DROP TABLE IF EXISTS AppUser;

-- Drop role
DROP ROLE KickBoxingAdminReader;
GO

-- Drop views
DROP VIEW IF EXISTS vw_TotalInvoiceRevenue;
DROP VIEW IF EXISTS vw_TotalSessionsPerTrainer;
DROP VIEW IF EXISTS vw_MemberRole;
GO