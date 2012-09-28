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
	SetName                VARCHAR(128) NOT NULL REFERENCES Set_(Name) ON DELETE CASCADE,
	Text                   TEXT NOT NULL,
	FlavorText             TEXT NOT NULL,
	Power                  VARCHAR(3),
	Toughness              VARCHAR(3),
	RarityId               INTEGER NOT NULL REFERENCES Rarity(Id) ON DELETE CASCADE,
	Number                 INTEGER,
	Variant                CHARACTER(1) NOT NULL,
	Artist                 VARCHAR(128) NOT NULL,
	LanguageId             INTEGER NOT NULL REFERENCES Language(Id) ON DELETE CASCADE,

	PRIMARY KEY(BaseMultiverseId, Part, Variant, LanguageId),

	UNIQUE(MultiverseId, Part, Variant, LanguageId)
);

CREATE INDEX CardSetLanguage ON Card(SetName, LanguageId);

CREATE TABLE IF NOT EXISTS CardType (
	CardBaseMultiverseId INTEGER NOT NULL,
	CardPart             VARCHAR(128) NOT NULL,
	CardVariant          CHARACTER(1) NOT NULL,
	CardLanguageId       INTEGER NOT NULL,
	Type                 VARCHAR(64),

	FOREIGN KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId) REFERENCES Card(BaseMultiverseId, Part, Variant, LanguageId) ON DELETE CASCADE,

	PRIMARY KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId, Type)
);

CREATE TABLE IF NOT EXISTS CardSubtype (
	CardBaseMultiverseId INTEGER NOT NULL,
	CardPart             VARCHAR(128) NOT NULL,
	CardVariant          CHARACTER(1) NOT NULL,
	CardLanguageId       INTEGER NOT NULL,
	Subtype              VARCHAR(64),

	FOREIGN KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId) REFERENCES Card(BaseMultiverseId, Part, Variant, LanguageId) ON DELETE CASCADE,

	PRIMARY KEY(CardBaseMultiverseId, CardPart, CardVariant, CardLanguageId, Subtype)
);