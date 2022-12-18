/* STEP 1: CREATE TABLES
create table [User](
	UserId int,
	Username nvarchar(50),
	[Password] nvarchar(50),
	[Name] varchar(50),

	primary key (UserId)
);

create table ShoppingSession(
	SessionId uniqueidentifier,
	UserId int,

	primary key (SessionId), 
	foreign key (UserId) references [User](UserId)
);

create table PurchaseHistory(
	PurchaseId int,
	UserId int, 
	PurchaseDate date, 

	primary key (PurchaseId), 
	foreign key (UserId) references [User](UserId)
);

create table Product(
	ProductId int, 
	ProductName varchar(50),
	ProductDescription varchar(500),
	Price int,
	ProductImage nvarchar(200),

	primary key (ProductId)
);

create table PurchaseDetails(
	PurchaseId int,
	ProductId int,
	ActivationCode uniqueidentifier,

	primary key (ActivationCode),
	foreign key (PurchaseId) references PurchaseHistory(PurchaseId),
	foreign key (ProductId) references Product(ProductId)
);

create table ProductRating(
	ProductId int, 
	UserId int,
	Rating int,

	primary key (ProductId, UserId)
);

create table Cart (
	UserId int,
	ProductId int,
	Qty int
);
*/

/* STEP 2: POPULATE PRODUCT TABLE AND USER TABLE 
//PRODUCT
insert into Product
values (1, '.NET Charts', 'Brings charting capabilities to your .NET applications.', 99);

insert into Product
values (2, '.NET Paypal', 'Integrate your .NET apps with Paypal the easy way!', 69);

insert into Product
values (3, '.NET ML', 'Supercharge your .Net machine learning libraries.', 299);

insert into Product
values (4, '.NET Analytics', 'Performs data mining and analytics easily in .NET.', 299);

insert into Product
values (5, '.NET Logger', 'Logs and aggregates events easily in your .NET apps.', 49);

insert into Product
values (6, '.NET Numerics', 'Powerful numerical methods for your .NET simulations.', 99);

//USER
insert into [User]
values (1, 'mary10', 'mary10', 'Mary Ann')

insert into [User]
values (2, 'glendon99', 'glendon99', 'Glendon Tan')
*/

/* STEP 3 (UPDATE): UPDATE PRODUCT TABLE 
Use ShoppingCartCA
GO

Update Product set ProductImage = 'Slide1.Jpeg' where ProductId = 1
Update Product set ProductImage = 'Slide2.Jpeg' where ProductId = 2
Update Product set ProductImage = 'Slide3.Jpeg' where ProductId = 3
Update Product set ProductImage = 'Slide4.Jpeg' where ProductId = 4
Update Product set ProductImage = 'Slide5.Jpeg' where ProductId = 5
Update Product set ProductImage = 'Slide6.Jpeg' where ProductId = 6
*/