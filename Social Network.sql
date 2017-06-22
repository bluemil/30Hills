--Friends

select u.firstName, u.surname 
from Users as u
join User_Friend as uf
on u.ID=uf.IDfr 
where uf.ID=5
union
select u.firstName, u.surname 
from Users as u
join User_Friend as uf
on u.ID=uf.ID
where uf.IDfr=5;


--Friends of friends

select u.ID, u.firstName, u.surname 
from Users as u
join User_Friend as uf
on u.ID=uf.IDfr and uf.Idfr<>5 and uf.IDfr not in (select * from fn_FriendsOfSelected(5))--zato sto necemo da nam vrati zajednicke prijatelje
where uf.ID in (select * from fn_FriendsOfSelected(5)) 
union 
select u.ID, u.firstName, u.surname  
from Users as u
join User_Friend as uf
on u.ID=uf.ID and uf.ID<>5 and uf.ID not in (select * from fn_FriendsOfSelected(5))
where uf.IDfr in (select * from fn_FriendsOfSelected(5)); 


--Suggested friends 

select u.ID, u.firstName, u.surname
from
(select f1.IDfr from fn_FriendsOfFriends(5) as f1
where  exists (select numidfr
				from (select count (*) as numidfr 
						from (select IDfr from fn_FriendsOfSelected(f1.IDfr) as ffof
								where ffof.IDfr in (select * from fn_FriendsOfSelected(5))) as derived) as d2
				where numidfr>=2 )) as sugfr
join Users as u 
on sugfr.IDfr =u.ID


--fn_FriendsOfSelected

create function [dbo].[fn_FriendsOfSelected] (@id as int)
returns table
as
return
	(select IDfr
	from User_Friend
	where ID=@id
	union
	select ID 
	from User_Friend
	where IDfr=@id)
GO


--fn_FriendsOfFriends

create function [dbo].[fn_FriendsOfFriends] (@id as int)
returns table
as
return
	(select IDfr 
	from User_Friend
	where Idfr<>@id and IDfr not in (select * from fn_FriendsOfSelected(@id))
	and ID in (select * from fn_FriendsOfSelected(@id)) 
	union 
	select ID 
	from User_Friend
	where ID<>@id and ID not in (select * from fn_FriendsOfSelected(@id))
	and IDfr in (select * from fn_FriendsOfSelected(@id)))
GO