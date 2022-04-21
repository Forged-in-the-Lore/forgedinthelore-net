all : clean restore build

clean:
	dotnet clean

restore:
	dotnet restore

build: 
	dotnet build

up:
	dotnet watch run

migration: $(migration)
	dotnet ef migrations add $(m)

db-update:
	 dotnet ef database update
 
db-reset:
	dotnet ef database drop
	make db-update

seed:
	make db-reset
	dotnet run seed