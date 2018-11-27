create table IISLog
(
	[File] nvarchar(100),
	[Line] int,
	[date-time] datetime,
	[date] date,
	[time] time,
	[s-ip] nvarchar(15),
	[cs-method] nvarchar(10),
	[cs-uri-stem] nvarchar(256),
	[cs-uri-query] nvarchar(2000), 
	[s-port] nvarchar(5),
	[cs-username] nvarchar(100),
	[c-ip] nvarchar(15),
	[cs(User-Agent)] nvarchar(2000),
	[cs(Referer)] nvarchar(2000),
	[sc-status] int,
	[sc-substatus] int,
	[sc-win32-status] bigint,
	[time-taken] int
)