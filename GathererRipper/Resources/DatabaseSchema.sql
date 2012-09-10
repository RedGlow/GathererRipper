CREATE TABLE IF NOT EXISTS CardType (
	CardBaseMultiverseId INTEGER NOT NULL,
	CardPart             VARCHAR(128) NOT NULL,
	CardVariant          CHARACTER(1),
	CardLanguageId       INTEGER NOT NULL,
	Type                 VARCHAR(64),

	PRIMARY KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId, Type),
	FOREIGN KEY(CardBaseMultiverseId) REFERENCES Card(BaseMultiverseId),
	FOREIGN KEY(CardPart) REFERENCES Card(Part),
	FOREIGN KEY(CardVariant) REFERENCES Card(Variant),
	FOREIGN KEY(CardLanguageId) REFERENCES Card(LanguageId)
);

CREATE TABLE IF NOT EXISTS CardSubtype (
	CardBaseMultiverseId INTEGER NOT NULL,
	CardPart             VARCHAR(128) NOT NULL,
	CardVariant          CHARACTER(1),
	CardLanguageId       INTEGER NOT NULL,
	Subtype              VARCHAR(64),

	PRIMARY KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId, Subtype),
	FOREIGN KEY(CardBaseMultiverseId) REFERENCES Card(BaseMultiverseId),
	FOREIGN KEY(CardPart) REFERENCES Card(Part),
	FOREIGN KEY(CardVariant) REFERENCES Card(Variant),
	FOREIGN KEY(CardLanguageId) REFERENCES Card(LanguageId)
);

CREATE TABLE IF NOT EXISTS Rarity (
	Id   INTEGER PRIMARY KEY,
	Name VARCHAR(64) NOT NULL
);

CREATE TABLE IF NOT EXISTS Language (
	Id   INTEGER PRIMARY KEY,
	Name VARCHAR(64) NOT NULL
);

CREATE INDEX IF NOT EXISTS LanguageName ON Language(Name);

CREATE TABLE IF NOT EXISTS Set_ (
	Name          VARCHAR(128) PRIMARY KEY,
	NumberOfCards INTEGER
);

CREATE TABLE IF NOT EXISTS Card (
	MultiverseId           INTEGER NOT NULL,
	BaseMultiverseId       INTEGER NOT NULL,
	Part                   VARCHAR(128) NOT NULL,
	Name                   VARCHAR(128) NOT NULL,
	ManaCost               VARCHAR(20),
	ConvertedManaCost      TINYINT NOT NULL,
	SetName                VARCHAR(128) NOT NULL,
	Text                   TEXT NOT NULL,
	FlavorText             TEXT NOT NULL,
	Power                  VARCHAR(3),
	Toughness              VARCHAR(3),
	RarityId               INTEGER NOT NULL,
	Number                 INTEGER,
	Variant                CHARACTER(1),
	Artist                 VARCHAR(128) NOT NULL,
	LanguageId             INTEGER NOT NULL,

	PRIMARY KEY(BaseMultiverseId, Part, Variant, LanguageId),

	UNIQUE(MultiverseId, Variant, LanguageId),

	FOREIGN KEY(SetName)    REFERENCES Set_(Name),
	FOREIGN KEY(RarityId)   REFERENCES Rarity(Id),
	FOREIGN KEY(LanguageId) REFERENCES Language(Id)
);

CREATE INDEX CardSetLanguage ON Card(SetName, LanguageId);