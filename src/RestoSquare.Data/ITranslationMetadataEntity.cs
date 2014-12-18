using System.ComponentModel.DataAnnotations;

namespace RestoSquare.Data
{
    public interface ITranslationMetadataEntity
    {
        [Required]
        string Language
        {
            get;
            set;
        }

        [Required]
        string Title
        {
            get;
            set;
        }
    }
}