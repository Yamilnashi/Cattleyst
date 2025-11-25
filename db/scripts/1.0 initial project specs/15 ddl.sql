insert [dbo].[Location]
(
	LocationName
)
values
	('East Idaho')
	,('Northwest Farms')
	,('Washington')
	,('Oregon Farms')
;

insert
	[dbo].[CattleType]
(
	CattleTypeCode
	,CattleTypeName
)
values
	(1, 'Aberdeen Angus')
	,(2, 'Japanese Black')
	,(3, 'Japanese Brown')
	,(4, 'Japanese Shorthorn')
	,(5, 'Japanese Polled')
	,(6, 'American Brahman')
	,(7, 'Red Angus')
	,(8, 'Hereford')
	,(9, 'Charolais')
	,(10, 'Limousin')
	,(11, 'Black Hereford')
	,(12, 'Lincoln Red')
;

insert
	[dbo].[CattleStatus]
(
	CattleStatusCode
	,CattleStatusName
)
values
	(1, 'Received')
	,(2, 'Processing')
	,(3, 'Processed')
	,(4, 'Under Observation')
	,(5, 'Quarantine')
;

insert [dbo].[ProductType]
(
	ProductTypeCode
	,ProductTypeName
)
values
	(1, 'Chuck')
	,(2, 'Rib')
	,(3, 'Brisket')
	,(4, 'Plate')
	,(5, 'Short Loin')
	,(6, 'Flank')
	,(7, 'Sirloin')
	,(8, 'Tenderloin')
	,(9, 'Top Sirloin')
	,(10, 'Bottom Sirloin')
	,(11, 'Round')
	,(12, 'Shank')
	,(13, 'Head')
;

insert [dbo].[ProductStatus]
(
	ProductStatusCode
	,ProductStatusName
)
values
	(1, 'Holding Pen')
	,(2, 'Ante-Mortem')
	,(3, 'Euthanized')
	,(4, 'Stunned')
	,(5, 'Bleeding')
	,(6, 'Inspection')
	,(7, 'Hide Removal')
	,(8, 'Evisceration')
	,(9, 'Splitting')
	,(10, 'Washing')
	,(11, 'Chilling')
	,(12, 'Grading')
	,(13, 'Butchering')
	,(14, 'Packaging')
	,(15, 'Ready To Ship')
;
	
insert [dbo].[Brand]
(
	BrandCode
	,BrandName
)
values
	(1, 'Snake River Farms')
	,(2, 'Double Range Northwest Beef')
	,(3, 'St. Helens Beef')
	,(4, 'Rancho El Oro Beef')
;

insert [dbo].[Customer]
(
	CustomerName
	,SavedDate
)
values
	('Ridley Supermarkets', sysutcdatetime())
	,('Fred Meyers', sysutcdatetime())
	,('Ruth''s Chris', sysutcdatetime())
	,('OZ Korean BBQ', sysutcdatetime())
;

insert [dbo].[OrderStatus]
(
	OrderStatusCode
	,OrderStatusName
)
values
	(1, 'Submitted')
	,(2, 'Preparing Order')
	,(3, 'Shipping')
	,(4, 'Shipped')
	,(5, 'Delivered')
; 

insert [idpy].[RequestState]
(
	RequestStateCode
	,RequestStateName
)	
values
	(1, 'Pending')
	,(2, 'Processing')
	,(3, 'Completed')
	,(4, 'Failed')
	,(5, 'TimedOut')
	,(6, 'Cancelled')
	,(7, 'Validated')
	,(8, 'Retried')
	,(9, 'Aborted')
	,(10, 'Archived')
;

insert [idpy].[EventType]
(
	EventTypeCode
	,EventTypeName
)
values
	(1, 'Cattle Received')
	,(2, 'Cattle Status Updated')
	,(3, 'Cattle Processed')
	,(4, 'Product Created')
	,(5, 'Product Status Updated')
	,(6, 'Stock Updated')
	,(7, 'Order Placed')
	,(8, 'Order Status Updated')
	,(9, 'Shipping Updated')
	,(10, 'Error Ocurred')
	,(11, 'Inventory Low')
	,(12, 'Compliance Alert')
;