USE KickBoxingClubDB;
GO

/* Q1 (JOIN + WHERE + ORDER BY)
   Aktiva nyheter, senaste först
*/
SELECT
    nl.NewsTitle AS Topic,
    nl.NewsContent AS Contents,
    u.FirstName + ' ' + u.LastName AS PostedBy,
    nl.PostWeek,
    nl.PostYear,
    nl.CreatedAt
FROM Newsletter nl
JOIN Account a ON nl.PostedByAccountID = a.AccountID
JOIN AppUser u ON a.UserID = u.UserID
WHERE nl.IsActive = 1
ORDER BY nl.PostYear DESC, nl.PostWeek DESC, nl.CreatedAt DESC;
GO

/* Q2 (JOIN + WHERE + ORDER BY)
   Aktivt medlemskap för en medlem (exempel med Noah Jones)
*/
SELECT
    u.FirstName,
    u.LastName,
    m.IsActive,
    m.StartDate,
    m.EndDate,
    mp.BillingPeriod,
    pl.Label,
    pl.Amount
FROM Membership m
JOIN AppUser u ON m.UserID = u.UserID
JOIN MembershipPlan mp ON m.MembershipPlanID = mp.MembershipPlanID
JOIN PriceList pl ON mp.PriceID = pl.PriceID
WHERE u.FirstName = 'Noah'
  AND u.LastName  = 'Jones'
  AND m.IsActive = 1
ORDER BY m.EndDate ASC;
GO

/* Q3 (REPORT: Top entities) (JOIN + GROUP BY + COUNT)
   Konton med flest fakturor
*/
SELECT TOP 10
    a.AccountID,
    a.Email,
    u.FirstName + ' ' + u.LastName AS MemberName,
    COUNT(i.InvoiceID) AS TotalInvoices
FROM Account a
JOIN AppUser u ON a.UserID = u.UserID
LEFT JOIN Invoice i ON a.AccountID = i.AccountID
GROUP BY a.AccountID, a.Email, u.FirstName, u.LastName
ORDER BY TotalInvoices DESC;
GO

/* Q4 (SUMMARY per kategori) (JOIN + GROUP BY + SUM)
   Total intäkt per BillingPeriod
*/
SELECT
    mp.BillingPeriod,
    COUNT(i.InvoiceID) AS InvoicesCount,
    SUM(COALESCE(pl.Amount, 0)) AS TotalRevenue
FROM Invoice i
JOIN MembershipPlan mp
    ON i.MembershipPlanID = mp.MembershipPlanID
JOIN PriceList pl
    ON mp.PriceID = pl.PriceID
GROUP BY mp.BillingPeriod
ORDER BY TotalRevenue DESC;
GO