CREATE TABLE Pacient(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Nume NVARCHAR(25) NOT NULL,
	Prenume NVARCHAR(50) NOT NULL,
	CNP NVARCHAR(13) NOT NULL,
	Data_Nastere DATETIME,
	Telefon NVARCHAR(10),
	Email NVARCHAR(50)
)

CREATE TABLE Adresa(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Tip_Adresa INT NOT NULL,
	Linie_Adresa NVARCHAR(50) NOT NULL,
	Localitate NVARCHAR(35) NOT NULL,
	Judet NVARCHAR(35) NOT NULL,
	Cod_Postal NVARCHAR(6),
	ID_Pacient INT NOT NULL FOREIGN KEY REFERENCES Pacient(ID)
)

CREATE TABLE Medicament(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Denumire NVARCHAR(75) NOT NULL,
	Forma VARCHAR(4) NOT NULL,
	Pret DECIMAL NOT NULL,
	Descriere NVARCHAR(MAX),
)

CREATE TABLE Lot(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Data_Expirare DATETIME NOT NULL,
	Cantitate int NOT NULL,
	ID_Medicament INT NOT NULL FOREIGN KEY REFERENCES Medicament(ID)
)

CREATE TABLE Comanda(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Status INT NOT NULL,
	Data DATETIME NOT NULL,
	Pret DECIMAL NOT NULL,
	ID_Pacient INT NOT NULL FOREIGN KEY REFERENCES Pacient(ID)
)

CREATE TABLE ComandaMedicament(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	ID_Comanda INT NOT NULL FOREIGN KEY REFERENCES Comanda(ID),
	ID_Medicament INT NOT NULL FOREIGN KEY REFERENCES Medicament(ID),
	Cantitate INT NOT NULL
)

CREATE TABLE Utilizator(
	ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Email NVARCHAR(50) NOT NULL,
	Parola NVARCHAR(MAX) NOT NULL,
	Tip VARCHAR(4) NOT NULL,
	ID_Pacient INT NOT NULL FOREIGN KEY REFERENCES Pacient(ID)
)