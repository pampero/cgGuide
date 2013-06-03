using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Migrations
{
    internal sealed partial class Configuration
    {
        private static void SeedSQLProcedures(Model.AppDbContext appDbContext)
        {
            appDbContext.Database.ExecuteSqlCommand(@"
CREATE PROCEDURE sp_GetCategories

	@business_id as integer

AS

	SET NOCOUNT ON

SELECT Name as Categories
FROM Category 
WHERE Id IN (SELECT Category_ID FROM Categorybusiness WHERE Business_ID = @business_id)");


            appDbContext.Database.ExecuteSqlCommand(@"
CREATE PROCEDURE sp_GetPaths

	@business_id as integer

AS

	SET NOCOUNT ON

declare @T table(ID int, Name varchar(500), Parent_Id int);

;WITH Cat
AS
(
    SELECT Id AS StartingId, Id, Parent_Id, Name
    FROM Category

    UNION ALL

    SELECT Cat.StartingId, C.Id, C.Parent_Id, C.Name
    FROM Category C INNER JOIN Cat ON C.Id = Cat.Parent_Id
)
INSERT INTO @T (ID, Name, Parent_Id)
SELECT Id, Name, Parent_Id
FROM Cat 
WHERE Cat.StartingId IN (SELECT Category_ID FROM CategoryBusiness WHERE Business_ID = @business_id)

;with C as
(
  select ID,
         Name,
         Parent_Id,
         cast('' as varchar(max)) as ParentNames,
         0 As Generation
  from @T
  where Parent_Id is null
  union all
  select T.ID,
         T.Name,
         T.Parent_Id,
         C.ParentNames + '/' + C.Name,
         Generation + 1 As Generation
  from @T as T         
    inner join C
      on C.ID = T.Parent_Id
)      
select ID,
       Name as Categories,
       CASE WHEN ParentNames = '' THEN
			'0/' + Name
       ELSE
			Convert(varchar, Generation) + '/' + stuff(ParentNames, 1, 1, '') + '/' + Name 
       END as Paths
from C;");

            appDbContext.SaveChanges();
        }
    }
}
