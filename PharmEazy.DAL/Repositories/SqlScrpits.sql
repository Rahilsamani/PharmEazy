
alter Proc spGetCategoriesByQueryAndPagination
@PageNumber int,
@PageSize int,
@SearchQuery nvarchar(150)
as
begin
	select * From Category
	where isdeleted=0 and (isnull(@SearchQuery,'')='' or name like @SearchQuery+'%') 
	order by id
	offset (@pageNumber-1)*@Pagesize rows
	fetch next @pagesize rows only
end
GO

ALTER proc spGetCategoriesCountOnSearch
@SearchQuery nvarchar(150)
as
begin
	select count(id) as count From Category
	where isdeleted=0 and (isnull(@SearchQuery,'')='' or name like @SearchQuery+'%') 
end

Go

alter procedure spGetMedicineAndStockDetails
as
begin
select m.Id ,m.Name, m.Description,m.ImageUrl, c.Name as CategoryName, 
u.Name as SellerName, 
s.Id, s.Quantity, s.ExpiryDate, s.Price 
from Medicine as m 
inner join Stock as s
on m.Id = s.MedicineId
inner join Category as c
on m.CategoryId=c.Id
inner join [User] as u
on m.SellerId=u.Id
end

Go

alter Procedure spGetMedicine
@medicineId int
as
begin
select m.*, s.* from
(select * from medicine as m where id = @medicineId) as m
inner join stock as s on s.medicineId = m.id
end

Go

alter proc spGetMedicinesCountOnSearch
@SearchQuery nvarchar(150)
as
begin
	select count(id) as count From Medicine
	where IsDeleted=0 and (isnull(@SearchQuery,'')='' or name like @SearchQuery+'%') 
end

Go

alter Proc spGetMedicinesByQueryAndPagination
@PageNumber int,
@PageSize int,
@SearchQuery nvarchar(150)
as
begin
	select m.*, s.* from
	(select * From Medicine
	where isdeleted=0 and (isnull(@SearchQuery,'')='' or name like @SearchQuery+'%')
	order by Id
	offset (@pageNumber-1)*@Pagesize rows
	fetch next @pagesize rows only) as m
	inner join stock as s on s.MedicineId = m.Id
end
GO

alter proc spGetCartItems
@userId nvarchar(100)
as
begin
	select c.id, c.Quantity,
	m.Name, m.Description, m.ImageUrl, m.Id as MedicineId, m.SellerId,
	s.Price, s.ExpiryDate, s.Id as StockId from
	(select * from CartItem 
	where UserId like @userId) as c
	inner join Medicine as m
	on c.MedicineId=m.Id and m.IsDeleted=0
	inner join Stock as s
	on c.stockId = s.id
end

Go

alter proc spGetAllUsers
@pagesize int,
@pageNumber int,
@searchQuery nvarchar(150)
as
begin
	Select U.Id, 
	R.Name as RoleName, 
	U.Name, U.Email, U.PhoneNumber, U.Address, U.GstNumber from (Select * From User_Role order by UserId offset (@pageNumber-1)*@Pagesize rows
	fetch next @pagesize rows only) as UR
	inner join Role as R
	on UR.RoleId = R.Id
	inner join (select * from [PharmEazy].[dbo].[User]
	where IsDeleted=0 and (isnull(@searchQuery,'')='' or name like @searchQuery+'%')
	) as U
	on U.Id = UR.UserId
end

Go

alter proc spGetAllUsersCountOnSearch
@searchQuery nvarchar(150)
as
begin
	select count(id) as count from [PharmEazy].[dbo].[User]
	where IsDeleted=0 and (isnull(@searchQuery,'')='' or name like @searchQuery+'%')
end

go

alter proc spGetUserDetails
@userId nvarchar(150)
as
begin
	select Id, Name, Email, PhoneNumber, Address, DateOfBirth, 
	GstNumber from [PharmEazy].[dbo].[User]
	where id like @userId
end

Go

Alter Proc spGetUserInvoices
@userId nvarchar(150)
as
begin
	select I.Id,I.TotalAmount,I.Status,I.CreatedOn, U.Name as SellerName, U.GstNumber from
	(select Id, TotalAmount, Status, CreatedOn, SellerId from Invoice
	where UserId = @userId) as I
	inner join [PharmEazy].[dbo].[User] as U
	on I.SellerId = U.Id
end

Go


alter Proc spGetInvoiceDeatail
@invoiceId int
as
begin
	select I.Id, I.TotalAmount, I.Status, I.CreatedOn, 
	U.Name, U.GstNumber, U.PhoneNumber,
	M.Id, M.Name, M.Description, M.ImageUrl,
	ID.Quantity, ID.Price,
	S.ExpiryDate
	from
	(select Id, TotalAmount, Status, CreatedOn, SellerId from Invoice where Id = @invoiceId) as I
	inner join InvoiceDetails as ID
	on I.Id = ID.InvoiceId
	inner join [PharmEazy].[dbo].[User] as U
	on I.SellerId = U.Id
	inner join Medicine as M
	on ID.MedicineId = M.Id
	inner join Stock as S
	on S.id = ID.StockId
end

Go

alter proc spCreateMedicine
@Name nvarchar(100),
@Description nvarchar(300),
@CategoryId int,
@ImageUrl nvarchar(200),
@SellerId nvarchar(200),
@CreatedOn DateTime
as
begin
	Insert Into Medicine 
	(Name, Description, CategoryId,ImageUrl, SellerId, CreatedOn, IsDeleted)
	Values (@Name, @Description, @CategoryId, @ImageUrl, @SellerId, @CreatedOn, 0)
	Select SCOPE_IDENTITY()
end

Go

alter proc spCreateStock
@MedicineId int,
@Quantity int,
@ExpiryDate DateTime,
@Price int,
@CreatedOn DateTime
as
begin
	Insert Into Stock 
	(MedicineId, Quantity, ExpiryDate,Price, CreatedOn, IsDeleted)
	Values (@MedicineId, @Quantity, @ExpiryDate, @Price, @CreatedOn, 0)
end

Go

alter proc spDeleteMedicine
@MedicineId int
as
begin
	Update Medicine Set IsDeleted = 1, DeletedOn = GetDate()
	where Id = @MedicineId
	select Id from stock where MedicineId = @MedicineId
end

go

alter proc spDeleteStock
@StockId int
as
begin
	Update Stock Set IsDeleted = 1, DeletedOn = GetDate()
	where Id = @StockId
end

GO

alter proc spCreateInvoice
@UserId nvarchar(200),
@TotalAmount Decimal(20, 5),
@SellerId nvarchar(200)
as
begin
	Insert Into Invoice 
	(Userid, TotalAmount, Status, CreatedOn, SellerId)
	Values (@UserId, @TotalAmount, 'Pending', GetDate(), @SellerId)
	Select SCOPE_IDENTITY()
end

Go

alter proc spCreateInvoiceDetail
@MedicineId int,
@Quantity int,
@InvoiceId int,
@Price Decimal(20,4),
@StockId int
as
begin
	Insert Into InvoiceDetails 
	(MedicineId, Quantity, InvoiceId,Price, StockId)
	Values (@MedicineId, @Quantity, @InvoiceId, @Price, @StockId)

	Update Stock Set Quantity = Quantity - @Quantity
	where Id = @StockId
end

Go


alter Proc spEditMedicine
@medicineId int,
@description nvarchar(300),
@name nvarchar(100),
@imageUrl nvarchar(300)
as
begin
	update Medicine set Description = @description, 
	Name = @name, ImageUrl = ISNULL(@imageUrl, ImageUrl),
	UpdatedOn = GetDate()
	where Id = @medicineId
end

go

alter Proc spEditStock
@stockId int,
@ExpiryDate Datetime,
@Price Decimal(20,5),
@Quantity int
as
begin
	update Stock set ExpiryDate = @ExpiryDate, 
	Price = @Price, Quantity = @Quantity,
	UpdatedOn = GetDate()
	where Id = @stockId
end

go

alter proc spGetCategoryNameAndId
as
begin
	Select Id, Name from Category
	where isDeleted=0
end

