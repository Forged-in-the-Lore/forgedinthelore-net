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

create-dev-db:
	docker run --name forgedinthelore-postgres -e POSTGRES_USER=appuser -e POSTGRES_PASSWORD=Passw0rd! -p 5432:5432  -d postgres:latest

start-db:
	docker start forgedinthelore-postgres

stop-db:
	docker stop forgedinthelore-postgres