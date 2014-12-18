using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSquare.Data
{
    public class TranslationMetadataEntity<TEntity> : ITranslationMetadataEntity
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public int ParentId
        {
            get;
            set;
        }

        [ForeignKey("ParentId")]
        public TEntity Parent
        {
            get;
            set;
        }

        [Required]
        public string Language
        {
            get;
            set;
        }

        [Required]
        public string Title
        {
            get;
            set;
        }
    }
}