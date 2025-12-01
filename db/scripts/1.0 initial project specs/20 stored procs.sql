drop procedure if exists [dbo].[CattleList];
GO

create procedure [dbo].[CattleList]
	@LocationIds varchar(max) = null
as
begin
	set nocount on;
	
	;with cteLocations as (
		select
			cast(value as int) as [LocationId]
		from
			string_split(@LocationIds, ',')
	)
	
	select
		CattleId
		,c.LocationId
		,CattleTypeCode
		,Birthdate
		,SavedDate
	from
		[dbo].[Cattle] c
	left join
		cteLocations l
		on l.LocationId = c.LocationId
	where
		@LocationIds is null 
		or l.LocationId is not null
	;
end
GO