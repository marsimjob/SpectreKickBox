USE KickBoxingClubDB
GO

-- ADMINISTRATORY VIEWS (For KickboxingAdmin)
-- View för totala insättning av alla fakturor
CREATE VIEW vw_TotalInvoiceRevenue
AS
SELECT 
SUM(pl.Amount) AS Revenue,
COUNT(i.InvoiceID) AS [Number of Invoices]
FROM Invoice i
JOIN MembershipPlan mp ON i.MembershipPlanID = mp.MembershipPlanID
JOIN PriceList pl ON mp.PriceID = pl.PriceID;
GO
-- View för att kolla hur många sessioner tränare har
CREATE VIEW vw_TotalSessionsPerTrainer
AS
SELECT 
    s.TrainerID,
    u.FirstName + ' ' + u.LastName AS TrainerName,
    COUNT(*) AS TotalSessions
FROM [Session] s
JOIN Account a ON s.TrainerID = a.AccountID
JOIN AppUser u ON a.UserID = u.UserID
GROUP BY s.TrainerID, u.FirstName, u.LastName;
GO
-- Kollar alla registrerade konton för vanliga medlemmar
CREATE VIEW vw_MemberRole
AS
SELECT
    a.AccountID,
    u.FirstName + ' ' + u.LastName AS MemberName,
    r.Title AS RoleTitle
FROM [Account] a
JOIN AppUser u ON a.UserID = u.UserID
JOIN [Role] r ON a.RoleID = r.RoleID
WHERE a.RoleID = 3;  -- Member ID status
GO