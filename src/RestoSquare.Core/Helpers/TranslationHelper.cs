using System;
using System.Collections.Generic;
using System.Linq;
using RestoSquare.Data;

namespace RestoSquare.Core.Helpers
{
    public static class TranslationHelper
    {
        public static IReadOnlyCollection<string> TryGet<TEntity, TTranslation>(this IEnumerable<TEntity> metadataItems, string language)
            where TEntity : MetadataEntity<TTranslation>
            where TTranslation : class, ITranslationMetadataEntity
        {
            var items = new List<string>();

            foreach (var metadataItem in metadataItems)
            {
                var translation = metadataItem.TryGet(language);
                if (!String.IsNullOrEmpty(translation))
                    items.Add(translation);

            }

            return items;
        }

        public static string TryGet<TTranslation>(this MetadataEntity<TTranslation> entity, string language)
            where TTranslation : class, ITranslationMetadataEntity
        {
            var translation = entity.Translations.FirstOrDefault(t => t.Language == language) ?? entity.Translations.FirstOrDefault(t => t.Language == "en");
            if (translation != null)
                return translation.Title;
            return String.Empty;
        }

        public static string TryGet(this Restaurant entity, Func<RestaurantTranslation, string> getter, string language)
        {
            var text = "";

            var translation = entity.Translations.FirstOrDefault(t => t.Language == language);
            if (translation != null)
                text = getter(translation);
            if (!String.IsNullOrEmpty(text))
                return text;

            translation = entity.Translations.FirstOrDefault(t => t.Language == "en");
            if (translation != null)
                text = getter(translation);
            if (!String.IsNullOrEmpty(text))
                return text;

            return String.Empty;
        }
    }
}
