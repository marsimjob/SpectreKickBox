USE KickBoxingClubDB
GO
-- Create role
--CREATE ROLE KickBoxingAdminReader;
--GO
-- Grant SELECT only on views
--GRANT SELECT ON vw_TotalInvoiceRevenue TO KickBoxingAdminReader;
--GRANT SELECT ON vw_TotalSessionsPerTrainer TO KickBoxingAdminReader;
--GRANT SELECT ON vw_MemberRole TO KickBoxingAdminReader;
--GO
-- Create SQL user (example)

--CREATE LOGIN KickboxingAdmin WITH PASSWORD = 'Kickbox.com';
--GO



CREATE USER KickBoxingAdmin FOR LOGIN KickboxingAdmin;
GO

-- Assign role
ALTER ROLE KickBoxingAdminReader
ADD MEMBER KickBoxingAdmin;
GO

