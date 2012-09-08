using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;
using System.Data.SQLite;
using MagicRipper;

namespace GathererRipper
{
    public class DbManager
    {
        private Dictionary<Rarity, int> rarityToId
            = new Dictionary<Rarity, int>();

        private Dictionary<Language, int> languageToId
            = new Dictionary<Language, int>();

        public DbManager()
        {
            using (var command = connection.CreateCommand())
            {
                fillEnumTable<Rarity>(command, rarityToId);
                fillEnumTable<Language>(command, languageToId);
            }
        }

        private void fillEnumTable<T>(DbCommand command,
            Dictionary<T, int> enumToId)
            where T: struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException(
                    string.Format("{0} is not an enum.", type));
            var typeName = type.Name;
            var getEnumTableId = string.Format(
                SQLResources.GetEnumTableId,
                typeName);
            var addEnumTable = string.Format(
                SQLResources.AddEnumTable,
                typeName);

            foreach (var entry in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, entry);

                command.CommandText = getEnumTableId;
                command.setParameters(name);

                var id = command.ExecuteScalar();

                if (id == null)
                {
                    command.CommandText = addEnumTable;
                    command.setParameters(name);
                    command.ExecuteNonQuery();

                    command.CommandText = getEnumTableId;
                    command.setParameters(name);
                    id = command.ExecuteScalar();
                }

                enumToId[(T)entry] = (int)(long)id;
            }
        }

        private DbConnection cachedConnection = null;

        private DbConnection connection
        {
            get
            {
                openConnection();
                return cachedConnection;
            }
        }

        public static string BasePath
        {
            get
            {
                var basePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GathererRipper");
                if (!File.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                return basePath;
            }
        }

        public static string DatabasePath
        {
            get
            {
                var basePath = BasePath;
                return System.IO.Path.Combine(
                    basePath,
                    "GathererRipper.sqlite");
            }
        }

        private void openConnection()
        {
            if (cachedConnection == null)
            {
                var databaseExists = File.Exists(DatabasePath);
                cachedConnection = new SQLiteConnection(
                    string.Format("Data Source={0}; Version=3;", DatabasePath));
                cachedConnection.Open();
                if (!databaseExists)
                    createDatabaseIfNotExists(cachedConnection);
            }
        }

        private void createDatabaseIfNotExists(DbConnection c)
        {
            using (var command = c.CreateCommand())
            {
                command.CommandText = SQLResources.DatabaseSchema;
                command.ExecuteNonQuery();
            }
        }

        public DbTransaction Transaction()
        {
            return connection.BeginTransaction();
        }

        public void AddCard(Card card)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.AddCard;
                command.setParameters(
                    card.MultiverseId,
                    card.BaseMultiverseId,
                    card.Part,
                    card.Name,
                    card.ManaCost,
                    card.ConvertedManaCost,
                    card.Set.Name,
                    card.Text,
                    card.FlavorText,
                    card.Power,
                    card.Toughness,
                    rarityToId[card.Rarity],
                    card.Number,
                    card.Variant,
                    card.Artist,
                    languageToId[card.Language]
                    );
                command.ExecuteNonQuery();

                addCardTags(command, card.MultiverseId,
                    card.Part, card.Variant, card.Language,
                    card.Types, "Type");

                addCardTags(command, card.MultiverseId,
                    card.Part, card.Variant, card.Language,
                    card.Subtypes, "Subtype");
            }
        }

        private void addCardTags(DbCommand command,
            int multiverseId, string part, char? variant, Language language,
            ICollection<string> tags, string tagName)
        {
            var addCardTag = string.Format(
                SQLResources.AddCardTag,
                tagName);
            foreach (var tag in tags)
            {
                command.CommandText = addCardTag;
                command.setParameters(multiverseId, part,
                    variant, languageToId[language],
                    tag);
                command.ExecuteNonQuery();
            }
        }

        public bool CardExistsByBase(int baseMultiverseId, string part)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.IsCardFullyDownloaded;
                command.setParameters(baseMultiverseId, part);
                var result = command.ExecuteScalar();
                return result != null;
            }
        }

        public bool CardExists(int baseMultiverseId, string part, Language language)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.GetCardWithoutVariant;
                command.setParameters(baseMultiverseId, part, languageToId[language]);
                var result = command.ExecuteScalar();
                return result != null;
            }
        }

        public bool SetExists(string setName)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.SetExists;
                command.setParameters(setName);
                var result = command.ExecuteScalar();
                return result != null;
            }
        }

        public void AddSet(string setName, int numberOfCards)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.AddSet;
                command.setParameters(
                    setName, numberOfCards);
                command.ExecuteNonQuery();
            }
        }

        public bool IsSetDownloaded(string setName)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = SQLResources.GetTotalAndDownloadedSetCardsNumber;
                command.setParameters(setName, setName);
                using (var resultReader = command.ExecuteReader())
                {
                    if (!resultReader.Read())
                        return false;
                    return resultReader.GetInt32(0) == resultReader.GetInt32(1);
                }
            }
        }
    }
}
