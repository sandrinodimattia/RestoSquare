using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class MetadataEntity<TTranslatedType>
        where TTranslatedType : ITranslationMetadataEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id
        {
            get;
            set;
        }

        [Required]
        public string Name
        {
            get;
            set;
        }
        public ICollection<TTranslatedType> Translations
        {
            get;
            set;
        }
    }
}