USE KickBoxingClubDB;
GO
-- FRONT PAGE QUERIES:
-- This is on the front page newsletter tha shows the latest news on top
SELECT
nl.NewsTitle AS Topic,
nl.NewsContent AS Contents,
u.FirstName + ' ' + u.LastName AS 'Posted By',
nl.PostWeek,
nl.PostYear
FROM Newsletter nl
JOIN [Account] AS a ON nl.PostedByAccountID = a.AccountID
JOIN AppUser AS u ON a.UserID = u.UserID
WHERE IsActive = 1
ORDER BY nl.PostYear DESC,nl.PostWeek DESC;
-- Lets you check your personal page with your payment plan
SELECT
    u.FirstName,
    u.LastName,
    mp.BillingPeriod,
    pl.Label,
    pl.Amount
FROM Membership m
JOIN AppUser u ON m.UserID = u.UserID
JOIN MembershipPlan mp ON m.MembershipPlanID = mp.MembershipPlanID
JOIN PriceList pl ON mp.PriceID = pl.PriceID
WHERE u.FirstName = 'Noah' AND u.LastName = 'Jones' -- In the console app we can use this as paremter to search for it and display information
AND m.IsActive = 1 -- Not a parameter but we can also check if they are still active
